using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

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

public struct DamageData
{
    public float Amount;
    public Vector2 SourcePosition;
}
