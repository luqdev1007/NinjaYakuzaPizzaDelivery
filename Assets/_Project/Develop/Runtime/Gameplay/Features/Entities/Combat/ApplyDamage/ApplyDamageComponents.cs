using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class DamageCooldown : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class DamageCooldownTimer: IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class TakeDamageRequest : IEntityComponent
    {
        public ReactiveEvent<DamageData> Value;
    }

    public class TakeDamageEvent : IEntityComponent
    {
        public ReactiveEvent<DamageData> Value;
    }

    public class CanApplyDamage : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    /// <summary>
    /// ИТОГОВЫЙ шанс уклонения в процентах (0..100) — то, что реально читает
    /// ApplyDamageSystem при броске. Единственный писатель —
    /// EvasionChanceStatSynchronizerSystem (база из конфига + модификаторы баффов).
    /// Руками сюда не писать: следующий же Recalculate затрёт значение.
    ///
    /// Лежит здесь, а не в StatsFeature, по тому же принципу, что MoveSpeed лежит в
    /// MovementComponents: итоговое значение принадлежит фиче-потребителю, а
    /// StatsFeature держит только пару База+Модификаторы.
    ///
    /// КОМПОНЕНТ OPT-IN. ApplyDamageSystem общая для героя, призрака, слайма,
    /// фонаря и пропсов; она берёт шанс через TryGetEvasionChance, поэтому
    /// отсутствие компонента означает «броска нет вообще», а не «шанс ноль».
    /// Сейчас компонент выдаётся только герою (MainHeroFactory).
    /// </summary>
    public class EvasionChance : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}
