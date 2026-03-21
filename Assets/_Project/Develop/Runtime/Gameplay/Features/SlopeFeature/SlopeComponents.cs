using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class IsOnSlope : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class SlopeBoostMultiplier : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeJumpForce : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class SlopeMask : IEntityComponent
    {
        public LayerMask Value;
    }
}