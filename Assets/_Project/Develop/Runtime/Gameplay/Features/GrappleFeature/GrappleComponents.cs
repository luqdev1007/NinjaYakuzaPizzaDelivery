using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleArrivalBounce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class GrappleMaxDistance : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsThrowingHook : IEntityComponent
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


