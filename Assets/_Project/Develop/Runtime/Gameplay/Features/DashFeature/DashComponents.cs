using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class CanDash : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class IsDashing : IEntityComponent 
    { 
        public ReactiveVariable<bool> Value; 
    }

    public class DashRequest : IEntityComponent
    { 
        public ReactiveEvent Value; 
    }

    public class DashStartedEvent : IEntityComponent
    { 
        public ReactiveEvent Value; 
    }

    public class DashCompletedEvent : IEntityComponent
    { 
        public ReactiveEvent Value; 
    }

    public class DashForceMin : IEntityComponent 
    {
        public ReactiveVariable<float> Value; 
    }

    public class DashForceMax : IEntityComponent 
    {
        public ReactiveVariable<float> Value; 
    }

    public class DashChargeTimeMax : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class DashCooldown : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class DashDuration : IEntityComponent 
    {
        public ReactiveVariable<float> Value; 
    }

    public class AirDashMultiplier : IEntityComponent 
    { 
        public ReactiveVariable<float> Value;
    }

    public class AirDashVerticalBoost : IEntityComponent 
    { 
        public ReactiveVariable<float> Value;
    }
}

