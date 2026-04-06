using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class IsOnSlope : IEntityComponent { public ReactiveVariable<bool> Value = new(); }
    public class SlopeAccumSpeed : IEntityComponent { public ReactiveVariable<float> Value = new(0f); }
    public class SlopeMask : IEntityComponent { public LayerMask Value; }
    public class SlopeJumpForce : IEntityComponent { public ReactiveVariable<Vector2> Value; }

    // Параметры из конфига
    public class SlopeMinAngle : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeMaxAngle : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeDownhillBaseForce : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeBoostMultiplier : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeMagnetForce : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeMaxAccumSpeed : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeAccumGainRate : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeAccumDecayRate : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeSlideOffDelay : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeMinEjectVelocity : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeEjectForceMultiplier : IEntityComponent { public ReactiveVariable<float> Value; }
    public class SlopeAutoSlidePush : IEntityComponent { public ReactiveVariable<float> Value; }
}