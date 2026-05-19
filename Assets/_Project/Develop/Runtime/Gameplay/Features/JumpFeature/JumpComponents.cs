using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpForceMax : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpChargeTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class JumpForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsGrounded : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class GravityScale : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ExtraJumpsAvailable : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class MaxExtraJumps : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class JumpEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class DoubleJumpEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class AirJumpMultiplier : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class MustRestoreExtraJumps : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class CanExtraJump : IEntityComponent
    {
        public ICompositeCondition Value;
    }
}