using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "MainHeroConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Main Hero Config")]
    public class MainHeroConfig : EntityConfig
    {
        [Header("Common")]
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float LootCollectRange { get; private set; } = 3f;
        [field: SerializeField] public float BaseBodyContactDamage { get; private set; } = 1f;
        [field: SerializeField] public float FallActionThreshold { get; private set; } = -2f;
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
        [field: SerializeField] public LayerMask ContactLayerMask { get; private set; }

        [Header("Settings")]
        [field: SerializeField] public MovementSettings Movement { get; private set; } = new();
        [field: SerializeField] public JumpSettings Jump { get; private set; } = new();
        [field: SerializeField] public AirJumpSettings AirJump { get; private set; } = new();
        [field: SerializeField] public DashSettings Dash { get; private set; } = new();
        [field: SerializeField] public GlideSettings Glide { get; private set; } = new();
        [field: SerializeField] public WallHangSettings WallHang { get; private set; } = new();
        [field: SerializeField] public WallJumpSettings WallJump { get; private set; } = new();
        [field: SerializeField] public SlideSettings Slide { get; private set; } = new();
        [field: SerializeField] public PlungeSettings Plunge { get; private set; } = new();
        [field: SerializeField] public SlopeSettings Slope { get; private set; } = new();
        [field: SerializeField] public AttackSettings Attack { get; private set; } = new();
        [field: SerializeField] public LifeCycleSettings LifeCycle { get; private set; } = new();
        [field: SerializeField] public ThrowableSettings Throwables { get; private set; } = new();
    }

    [Serializable]
    public class MovementSettings
    {
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField, Min(0)] public float MoveSpeedMin { get; private set; } = 5f;
        [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 30f;
    }

    [Serializable]
    public class AirJumpSettings
    {
        [field: SerializeField, Min(0)] public float ForceMin { get; private set; } = 12f;
        [field: SerializeField, Min(0)] public float ForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float MaxChargeTime { get; private set; } = 0.25f;
        [field: SerializeField, Min(1)] public int JumpsMaxCount { get; private set; } = 1;
    }

    [Serializable]
    public class JumpSettings
    {
        [field: SerializeField, Min(0)] public float ForceMin { get; private set; } = 12f;
        [field: SerializeField, Min(0)] public float ForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float MaxChargeTime { get; private set; } = 0.25f;
    }

    [Serializable]
    public class DashSettings
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.25f;
        [field: SerializeField] public float InputBufferTimeMax { get; private set; } = 0.15f;
        [field: SerializeField] public float ForceMin { get; private set; } = 15f;
        [field: SerializeField] public float ForceMax { get; private set; } = 30f;
        [field: SerializeField] public float MaxChargeTime { get; private set; } = 0.3f;
        [field: SerializeField] public float Cooldown { get; private set; } = 0.5f;
        [field: SerializeField] public float AirMultiplier { get; private set; } = 2f;
        [field: SerializeField] public float VerticalBoost { get; private set; } = 2f;
        [field: SerializeField] public float Damage { get; private set; } = 10f;
        [field: SerializeField] public Vector2 HitboxSize { get; private set; } = new Vector2(0.5f, 1.5f);
    }

    [Serializable]
    public class GlideSettings
    {
        [field: SerializeField, Tooltip("Сопротивление горизонтальному движению во время парения")]
        public float HorizontalDrag { get; private set; } = 3f;

        [field: SerializeField, Tooltip("Максимальная скорость падения при открытом парашюте")]
        public float MaxFallSpeed { get; private set; } = -2f;

        [field: SerializeField, Tooltip("Плавность замедления вертикальной скорости после активации")]
        public float SpeedDamping { get; private set; } = 5f;

        [field: SerializeField, Tooltip("Сила подскока вверх при закрытии парашюта")]
        public float BounceForce { get; private set; } = 5f;

        [field: SerializeField, Tooltip("Сила моментального торможения в момент раскрытия")]
        public float SnapSpeed { get; private set; } = 40f;

        [field: SerializeField, Tooltip("Длительность фазы моментального торможения")]
        public float SnapDuration { get; private set; } = 0.15f;

        [field: SerializeField, Tooltip("Множитель противодействия текущей скорости падения при раскрытии.")]
        public float GravityScale { get; private set; } = 0.8f;
    }

    [Serializable]
    public class WallJumpSettings
    {
        [field: SerializeField] public float VelocityYAbs { get; private set; } = 5f;
        [field: SerializeField] public float ControlLockDuration { get; private set; } = 0.2f;
        [field: SerializeField] public LayerMask WallMask { get; private set; }
        [field: SerializeField] public Vector2 ForceMultiplier { get; private set; } = new Vector2(0.5f, 1.2f);
    }

    [Serializable]
    public class WallHangSettings
    {
        [field: SerializeField] public LayerMask Layer { get; private set; }
        [field: SerializeField] public float SlideSpeed { get; private set; } = 1f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(8f, 12f); // from jump settings
    }

    [Serializable]
    public class SlideSettings
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.2f;
        [field: SerializeField] public float Speed { get; private set; } = 15f;
        [field: SerializeField] public float Cooldown { get; private set; } = 0.25f;
        [field: SerializeField] public Vector2 HitBoxSize { get; private set; } = new Vector2(1.5f, 0.5f); // not used!
    }

    [Serializable]
    public class PlungeSettings
    {
        [field: SerializeField] public float Speed { get; private set; } = 25f;
        [field: SerializeField] public float DamageRange { get; private set; } = 3f;
        [field: SerializeField] public float Damage { get; private set; } = 10f;
        [field: SerializeField] public float KnockbackForce { get; private set; } = 5f;
        [field: SerializeField] public float MinImpactSpeedThreshold { get; private set; } = 5f;
        [field: SerializeField] public float AccelerationMultiplier { get; private set; } = 5f;
    }

    [Serializable]
    public class SlopeSettings
    {
        [field: SerializeField, Range(0, 90)] public float MinAngle { get; private set; } = 15f;
        [field: SerializeField, Range(0, 90)] public float MaxAngle { get; private set; } = 75f;
        [field: SerializeField] public float SlipForce { get; private set; } = 15;
        [field: SerializeField] public float MaxStableAngle { get; private set; } = 45;
        [field: SerializeField] public float BaseSlideSpeed { get; private set; } = 15;
        [field: SerializeField] public float SlideAcceleration { get; private set; } = 25;
        [field: SerializeField] public float MaxSlideSpeed { get; private set; } = 40;
        [field: SerializeField] public float BaseJumpForce { get; private set; } = 15;
        [field: SerializeField] public Vector2 JumpForceModifier { get; private set; }
        [field: SerializeField] public float MinFallVelocityForAutoSlide { get; private set; } = -1f;
    }

    [Serializable]
    public class AttackSettings
    {
        [Header("Timings")]
        [field: SerializeField] public float ProcessTime { get; private set; } = 0.2f;
        [field: SerializeField] public float DelayTime { get; private set; } = 0.1f;
        [field: SerializeField] public float Cooldown { get; private set; } = 0.15f;

        [Header("Combat Parameters")]
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
        [field: SerializeField] public float Damage { get; private set; } = 3f;
        [field: SerializeField] public float Range { get; private set; } = 1.5f;
        [field: SerializeField] public float InvulnerabilityDuration { get; private set; } = 0.2f;

        [Header("Hit Bounce")]
        [field: SerializeField] public float HitBounceForce { get; private set; } = 4f;
        [field: SerializeField] public Vector2 GroundHitBounceModifiers { get; private set; } = new Vector2(1f, 1f);
        [field: SerializeField] public Vector2 AirHitBounceModifiers { get; private set; } = new Vector2(1f, 2f);

        [Header("Hit Stop")]
        [field: SerializeField, Range(0f, 1f)] public float HitStopScale { get; private set; } = 0.05f;
        [field: SerializeField, Range(0f, 0.5f)] public float HitStopDuration { get; private set; } = 0.15f;
        [field: SerializeField] public Vector2 AttackKnockback { get; private set; } = new Vector2(1f, 2f);
        [field: SerializeField] public Vector2 AerialHangForce { get; private set; } = new Vector2(1f, 4);
        [field: SerializeField] public Vector2 RecoilForce { get; private set; } = new Vector2(10f, 20f);
        [field: SerializeField, Range(0f, 100f)] public float DoubleAttackChance { get; private set; } = 45f;
        [field: SerializeField] public float DoubleAttackCooldown { get; private set; } = 0.2f;
        [field: SerializeField] public float SlashAttackChargeRequiredTime { get; private set; } = 0.2f;
    }

    [Serializable]
    public class LifeCycleSettings
    {
        [field: SerializeField] public float MaxHealth { get; private set; } = 5f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 1.5f;
        [field: SerializeField] public float SpawnProcessTime { get; private set; } = 1f;
        [field: SerializeField] public float DamageCooldown { get; private set; } = 0.5f;
    }

    [Serializable]
    public class ThrowableSettings
    {
        [field: SerializeField] public GrappleHookConfig GrappleConfig { get; private set; }
        [field: SerializeField] public ShurikenConfig ShurikenConfig { get; private set; }
        [field: SerializeField] public SleepDartConfig SleepDartConfig { get; private set; }
    }
}