using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    // Компонент для Героя
    public class CometDashStateComponent : IEntityComponent
    {
        public int MaxCharges = 3;
        public ReactiveVariable<int> CurrentCharges;
        public ReactiveVariable<float> CurrentMultiplier; // Изначально 1.0
        public float MultiplierDegradation = 0.6f; // Коэффициент затухания
        public float BaseCooldown = 2f;
        public float OverheatCooldown = 8f;
        public ReactiveVariable<float> CooldownTimer;
    }

    // Тэг-маркер для прожектайла (уже обсуждали)
    public class ChargedSlashProjectileTag : IEntityComponent { }

    public class DashForceMin : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashForceMax : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashChargeTime : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashCooldown : IEntityComponent { public ReactiveVariable<float> Value; }
    public class IsDashing : IEntityComponent { public ReactiveVariable<bool> Value; }
    public class DashDuration : IEntityComponent { public ReactiveVariable<float> Value; }

    public class AirDashMultiplier : IEntityComponent { public ReactiveVariable<float> Value; }
    public class AirDashVerticalBoost : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashDamage : IEntityComponent { public ReactiveVariable<float> Value; }
    public class DashHitboxSize : IEntityComponent { public ReactiveVariable<Vector2> Value; }
}

