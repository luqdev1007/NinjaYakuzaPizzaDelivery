using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class DashForceMin : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashForceMax : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashChargeTime : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashCooldown : IEntityComponent { public ReactiveVariable<float> Value; }
    public class IsDashing : IEntityComponent { public ReactiveVariable<bool> Value; }
    public class DashDuration : IEntityComponent { public ReactiveVariable<float> Value; }
}

