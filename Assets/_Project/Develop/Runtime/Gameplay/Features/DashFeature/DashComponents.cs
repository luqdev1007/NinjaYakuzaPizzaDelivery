using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashForceMin : IEntityComponent 
    {
        public ReactiveVariable<float> Value;
    }

    public class DashForceMax : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashChargeTime : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashCooldown : IEntityComponent { public ReactiveVariable<float> Value; }
    public class IsDashing : IEntityComponent { public ReactiveVariable<bool> Value; }
    public class DashDuration : IEntityComponent { public ReactiveVariable<float> Value; }

    public class AirDashMultiplier : IEntityComponent { public ReactiveVariable<float> Value; }
    public class AirDashVerticalBoost : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashDamage : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashHitboxSize : IEntityComponent { public ReactiveVariable<Vector2> Value; }
    public class DashRequest : IEntityComponent { public ReactiveEvent Value; }
}

