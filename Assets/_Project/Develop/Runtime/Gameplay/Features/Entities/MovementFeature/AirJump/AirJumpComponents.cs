using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump
{
    public class CanAirJump : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class MustRestoreAirJumpsCount : IEntityComponent
    {
        public ICompositeCondition Value;
    }


    public class AirJumpForceMin : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AirJumpForceMax : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }



    public class AirJumpChargeTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }


    public class AirJumpsMaxCount : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
    public class AirJumpsCount : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class AirJumpRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class AirJumpEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }
}
