using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    // Компонент для Героя
    public struct CometDashData
    {
        public int MaxCharges;
        public float MultiplierDegradation;
        public float BaseCooldown;
        public float OverheatCooldown;

        // В структурах удобно держать только чистые данные (float, int, Vector2)
        // ReactiveVariable лучше оставить в самом классе компонента для удобства подписки
    }

    public class CometDashStateComponent : IEntityComponent
    {
        // Группируем конфиг
        public CometDashData Config;

        // Состояние (оставляем реактивным для UI/Систем)
        public ReactiveVariable<int> CurrentCharges;
        public ReactiveVariable<float> CurrentMultiplier;
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

