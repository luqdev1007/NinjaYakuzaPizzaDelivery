using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleAnchoredEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class GrappleHookTransform : IEntityComponent
    {
        public ReactiveVariable<Transform> Value;
    }

    public class GrappleMinDistance : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GrappleArrivalBounce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GrappleMaxDistance : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsGrappledTarget : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class CanGrapple : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsGrappling : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class GrappleSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GrappleProjectileSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GrappleAnchorPoint : IEntityComponent
    {
        public ReactiveVariable<Vector3> Value;
    }

    public class GrappleArriveDistance : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}


