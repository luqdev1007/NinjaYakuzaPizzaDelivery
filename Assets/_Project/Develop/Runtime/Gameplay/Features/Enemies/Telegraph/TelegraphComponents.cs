using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Telegraph
{
    // Конвенция EntityAPIGenerator: РОВНО ОДНО public-поле с именем Value.
    // Нарушишь — генератор выдаст урезанный API (без свойства-геттера и без
    // TryGet), и это не будет ошибкой компиляции, просто молча пропадут методы.

    /// <summary>
    /// Идёт windup перед атакой: окно, за которое игрок успевает среагировать.
    /// Читатель — <c>TelegraphView</c> на префабе врага (DOTween-сжатие визуала).
    ///
    /// ОБЩИЙ КОМПОНЕНТ, не привязан к конкретному врагу. Раньше жил в
    /// TongueComponents под слайм-специфичным именем и обслуживал только язык
    /// слайма; вынесен в нейтральную локацию, когда телеграф понадобился фонарю
    /// (Lantern) — тот же флаг, та же вьюха.
    ///
    /// SINGLE-WRITER — но per-entity: на КАЖДОЙ сущности писатель ровно один.
    /// На слайме это TongueSystem, на фонаре — LanternFireSystem. Инвариант не
    /// нарушен: две разные сущности, два разных единственных писателя.
    ///
    /// Заведён отдельным флагом, а не выведен из фазового состояния стрелка,
    /// осознанно: это отдельное семантическое состояние, склейка дала бы тихий
    /// orphaning (правило single-writer из CLAUDE.md).
    /// </summary>
    public class IsTelegraphing : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
