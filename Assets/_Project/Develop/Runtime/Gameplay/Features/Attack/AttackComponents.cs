using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    // Событие для View, чтобы проиграть звук и эффект попадания
    public class SuccessfulHitEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    // Настройки HitStop (замедление времени)
    public class AttackHitStopScale : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class AttackHitStopDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    // Настройки отскока (Bounce)
    public class AttackHitBounceForce : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class GroundHitBounceModifiers : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new();
    }

    public class AirHitBounceModifiers : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new();
    }

    // Слой врагов для системы попадания
    public class AttackEnemyMask : IEntityComponent
    {
        public ReactiveVariable<LayerMask> Value = new();
    }

    public class AttackInvulnerabilityDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackInvulnerabilityTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsAttackInvulnerable : IEntityComponent
    {
        public ReactiveVariable<bool> Value = new();
    }

    public class StartAttackRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class StartAttackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class CanStartAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class EndAttackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class AttackProcessInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackProcessCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InAttackProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class AttackRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDelayTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDelayEndEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class InstantAttackDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ShootPoint : IEntityComponent
    {
        public Transform Value;
    }

    public class MustCancelAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class AttackCanceledEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class AttackCooldownInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackCooldownCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InAttackCooldown : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}

