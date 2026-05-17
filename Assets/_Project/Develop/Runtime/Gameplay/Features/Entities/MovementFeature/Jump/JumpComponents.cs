using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump
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

    public class JumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class JumpEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }
}