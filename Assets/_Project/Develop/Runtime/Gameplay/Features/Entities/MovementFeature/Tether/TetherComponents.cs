using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Tether
{
    // Конвенция EntityAPIGenerator: РОВНО ОДНО public-поле с именем Value.
    // Нарушишь — генератор выдаст урезанный API (без свойства-геттера и без
    // TryGet), и это не будет ошибкой компиляции, просто молча пропадут методы.
    //
    // ═════════════════════════════════════════════════════════════════════
    //  КОНТРАКТ ПИСАТЕЛЬ/ЧИТАТЕЛЬ — ЗАФИКСИРОВАН, НЕ РАСШИРЯТЬ БЕЗ ПОВОДА
    // ═════════════════════════════════════════════════════════════════════
    //
    // Все четыре компонента ниже живут НА ГЕРОЕ.
    //
    //   ЕДИНСТВЕННЫЙ ПИСАТЕЛЬ TetherRequest / TetherReleaseRequest /
    //   TetherAnchorPoint  — TongueSystem (живёт на СЛАЙМЕ).
    //
    //   ЕДИНСТВЕННЫЙ ПИСАТЕЛЬ IsTethered — TetherPullSystem (живёт на ГЕРОЕ).
    //
    //   ЕДИНСТВЕННЫЙ ЧИТАТЕЛЬ всех четырёх — TetherPullSystem, плюс условия
    //   героя (CanBeTethered, CanGrapple) читают IsTethered.
    //
    // Обрати внимание на асимметрию: враг пишет ЗАПРОСЫ и ЯКОРЬ, но флаг
    // состояния IsTethered пишет только система героя. Это не случайность —
    // флаг обязан гаснуть на ЛЮБОМ пути завершения, включая тот, где слайм
    // умер и уже ничего никому не пришлёт (см. предохранитель по MaxDuration
    // в TetherPullSystem). Отдай флаг слайму — и смерть слайма оставила бы
    // героя навсегда захваченным.
    //
    // СХЕМА ЦЕЛИКОМ — по образцу BounceImpulseRequest (см. BounceComponents.cs):
    // внешний актор зовёт Invoke на компоненте-запросе, применяет — система
    // самой сущности, владеющая её rigidbody. Прямой записи в чужой rigidbody
    // мимо арбитража нет и быть не должно.

    /// <summary>
    /// Уставки одного захвата. Едут в запросе, а не компонентами на герое:
    /// это параметры КОНКРЕТНОГО языка, приезжающие из SlimeConfig, а не
    /// свойство героя. Разные слаймы могут тянуть по-разному.
    /// </summary>
    public struct TetherData
    {
        /// <summary>Кап общего модуля скорости во время притяга.</summary>
        public float PullSpeed;

        /// <summary>Ускорение к якорю, ед/с². Складывается с текущей скоростью.</summary>
        public float PullAcceleration;

        /// <summary>
        /// Предохранитель на стороне ГЕРОЯ, сек. Дублирует GrabTime слайма
        /// намеренно — см. обоснование в шапке TetherPullSystem.
        /// </summary>
        public float MaxDuration;
    }

    /// <summary>
    /// Почему захват закончился. Развилка нужна ровно в одном месте —
    /// TetherPullSystem решает по ней, отпускать С ИНЕРЦИЕЙ (Cut/Timeout/
    /// SourceDied) или отскоком (Arrived).
    /// </summary>
    public enum TetherReleaseReason
    {
        /// <summary>Язык разрублен атакой или тараном. Награда — инерция.</summary>
        Cut,

        /// <summary>Герой дотянут до слайма. Дальше отскок + контактный урон.</summary>
        Arrived,

        /// <summary>Слайм умер с активным захватом.</summary>
        SourceDied,

        /// <summary>Истёк GrabTime на стороне слайма.</summary>
        Timeout
    }

    /// <summary>
    /// Героя тянет язык. Читатели — условия CanBeTethered и CanGrapple, в
    /// перспективе вьюхи (анимация захвата).
    ///
    /// Single-writer: TetherPullSystem.
    /// </summary>
    public class IsTethered : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    /// <summary>
    /// Актуальная мировая точка притяга — позиция ShootPoint слайма.
    /// Обновляется КАЖДЫЙ ТИК, а не фиксируется на старте: слайм может быть
    /// сдвинут тараном уже после того, как язык вцепился (та же причина, по
    /// которой каждый тик переписывается TongueOriginPoint).
    ///
    /// Single-writer: TongueSystem.
    /// </summary>
    public class TetherAnchorPoint : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    /// <summary>
    /// Просьба начать захват. Приходит из fixed-системы слайма.
    ///
    /// Single-writer: TongueSystem.
    /// </summary>
    public class TetherRequest : IEntityComponent
    {
        public ReactiveEvent<TetherData> Value;
    }

    /// <summary>
    /// Просьба завершить захват с указанием причины.
    ///
    /// НЕ ЕДИНСТВЕННЫЙ путь завершения: TetherPullSystem умеет отпустить сама
    /// по своему таймеру, если этот запрос так и не пришёл. Полагаться на
    /// дисциплину слайма нельзя — он может успеть умереть.
    ///
    /// Single-writer: TongueSystem.
    /// </summary>
    public class TetherReleaseRequest : IEntityComponent
    {
        public ReactiveEvent<TetherReleaseReason> Value;
    }

    /// <summary>
    /// Можно ли зацепить героя языком прямо сейчас. Состав условия и
    /// обоснование каждой строки — в EntitiesFactory.AddHeroConditions.
    ///
    /// Читатель — TongueSystem на слайме, в момент попадания языка.
    /// </summary>
    public class CanBeTethered : IEntityComponent
    {
        public ICompositeCondition Value;
    }
}
