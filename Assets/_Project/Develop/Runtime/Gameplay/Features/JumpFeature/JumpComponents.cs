using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class CanJump : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class JumpForceMin : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpForceMax : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class JumpChargeTime : IEntityComponent
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

    public class JumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class JumpEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class DoubleJumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class DoubleJumpEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }
}