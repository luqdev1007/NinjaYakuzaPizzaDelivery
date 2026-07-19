using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Tongue
{
    // Конвенция EntityAPIGenerator: РОВНО ОДНО public-поле с именем Value.
    // Нарушишь — генератор выдаст урезанный API (без свойства-геттера и без
    // TryGet), и это не будет ошибкой компиляции, просто молча пропадут методы.
    //
    // ПРАВИЛО РЕАКТИВНОСТИ (то же, что применено к PatrolPointA/PatrolPointB):
    // ReactiveVariable только там, где есть подписчик или где значение меняется
    // в рантайме и читается по фронту. Уставки, приезжающие из SlimeConfig один
    // раз при сборке сущности, — обычные поля.
    //
    // ПРО ИМЕНОВАНИЕ МАСКИ: поле называется SightBlockMask и никогда WallMask.
    // В Entity.cs живёт заглушка AddWallMask с телом throw new
    // NotImplementedException рядом с корректной генерённой перегрузкой —
    // известная мина проекта. Слово WallMask в новых компонентах не используем.

    // ─────────────────────────────────────────────────────────────────────
    //  НА СЛАЙМЕ — реактивные
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Язык в работе: телеграф, полёт или втягивание. Читатель — условие CanMove
    /// слайма (движение гасится на весь цикл языка).
    ///
    /// Single-writer: TongueSystem.
    /// </summary>
    public class IsTongueActive : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    /// <summary>
    /// Идёт windup перед плевком. Читатель — TongueTelegraphView на префабе
    /// слайма (DOTween-сжатие узла SlimeView).
    ///
    /// Заведён отдельным флагом, а не выведен из IsTongueActive, осознанно:
    /// это разные семантические состояния, склейка их в один флаг дала бы
    /// тихий orphaning (правило single-writer из CLAUDE.md).
    ///
    /// Single-writer: TongueSystem.
    /// </summary>
    public class IsTongueTelegraphing : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    // ─────────────────────────────────────────────────────────────────────
    //  НА СЛАЙМЕ — уставки из конфига, нереактивные
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Дальность языка. Из SlimeConfig.MaxRange. Работает и как радиус поиска
    /// цели, и как предел пройденной дистанции в фазе полёта.
    /// </summary>
    public class TongueRange : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Скорость полёта кончика вперёд. Из SlimeConfig.GrabAttackSpeed.
    /// </summary>
    public class TongueSpeed : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Скорость втягивания кончика обратно. Из SlimeConfig.GrabBackSpeed.
    /// </summary>
    public class TongueRetractSpeed : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Длительность телеграфа, сек. Из SlimeConfig.TongueTelegraphDuration.
    /// </summary>
    public class TongueTelegraphDuration : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Время отращивания после полного цикла, сек. Из SlimeConfig.TongueCooldown.
    /// </summary>
    public class TongueCooldown : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Порог dot между направлением на цель и Vector2.up. 0 = ровно полусфера
    /// вверх. Из SlimeConfig.TongueAimArcDot.
    /// </summary>
    public class TongueAimArcDot : IEntityComponent
    {
        public float Value;
    }

    /// <summary>
    /// Препятствия для линии взгляда и для полёта языка. Из
    /// SlimeConfig.SightBlockMask. Про имя — см. шапку файла.
    /// </summary>
    public class SightBlockMask : IEntityComponent
    {
        public LayerMask Value;
    }

    /// <summary>
    /// На чём искать героя. Из SlimeConfig.TargetMask (Characters).
    /// </summary>
    public class TargetMask : IEntityComponent
    {
        public LayerMask Value;
    }

    /// <summary>
    /// Путь к префабу языка в Resources. Из SlimeConfig.TonguePrefabPath.
    /// </summary>
    public class TonguePrefabPath : IEntityComponent
    {
        public string Value;
    }

    // ─────────────────────────────────────────────────────────────────────
    //  НА ЯЗЫКЕ
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Мировая позиция корня языка — точки вылета на слайме. Обновляется каждый
    /// fixed-тик: слайм может быть сдвинут тараном уже после выстрела.
    ///
    /// ЖИВЁТ ТОЛЬКО НА СУЩНОСТИ ЯЗЫКА. Копии на слайме нет намеренно:
    /// единственный читатель — TongueView на префабе языка, а копия на слайме
    /// была бы вторым хранилищем той же величины без читателя, требующим
    /// ручной синхронизации.
    ///
    /// Single-writer: TongueSystem — она живёт на слайме, но пишет в этот
    /// компонент созданной ею сущности языка напрямую. Писатель по-прежнему
    /// ровно один, инвариант не нарушен.
    /// </summary>
    public class TongueOriginPoint : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    // ─────────────────────────────────────────────────────────────────────
    //  НА ГЕРОЕ
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Факт разруба языка. Инвоукается В ЛЮБОЙ ФАЗЕ — и когда язык сбит в
    /// полёте, и когда разрублен уже вцепившимся в героя.
    ///
    /// ЖИВЁТ НА ГЕРОЕ, хотя пишет его система слайма — ровно та же схема, что
    /// у TongueOriginPoint на языке: писатель по-прежнему ровно один.
    /// Компонент здесь потому, что единственный читатель — MainHeroStyleSystem,
    /// а она живёт на герое и переживает смерть любого отдельного слайма.
    ///
    /// Отдельным событием, а не веткой внутри TakeDamageEvent, осознанно:
    /// у сущности языка нет ни ApplyDamageSystem, ни TakeDamageEvent —
    /// TongueSystem подписана на её TakeDamageRequest напрямую.
    ///
    /// Single-writer: TongueSystem.
    /// </summary>
    public class TongueCutEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }
}
