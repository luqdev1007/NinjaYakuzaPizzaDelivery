using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Plunge
{
    public class CanPlunge : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class PlungeAccelerationMultiplier : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class MinPlungeImpactSpeedThreshold : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeImpactEvent : IEntityComponent
    {
        public ReactiveEvent<float> Value;
    }

    public class IsPlunging : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class PlungeSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactKnockbackForceMin : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class PlungeLandImpactHitMask : IEntityComponent
    {
        public LayerMask Value;
    }
}