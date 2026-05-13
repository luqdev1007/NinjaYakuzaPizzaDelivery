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
}
