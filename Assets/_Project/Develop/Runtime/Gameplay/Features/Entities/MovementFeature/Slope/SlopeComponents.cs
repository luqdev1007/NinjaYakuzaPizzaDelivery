using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class IsOnSlope : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class SlopeMask : IEntityComponent
    {
        public LayerMask Value;
    }

    public class MinFallVelocityForAutoSlide : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeNormal: IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class SlopeBaseSlideSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeSlideAcceleration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeMaxSlideSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeMaxAccumSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeMaxStableAngle : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeSlipForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeAccumGainRate : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeAccumSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeAngle : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeMinAngle : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class SlopeMaxAngle : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }
}