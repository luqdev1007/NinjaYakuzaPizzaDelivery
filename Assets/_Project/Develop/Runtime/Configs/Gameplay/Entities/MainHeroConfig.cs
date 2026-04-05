using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "MainHeroConfig", menuName = "Configs/Gameplay/Main Hero/New Main Hero Config")]
    public class MainHeroConfig : EntityConfig
    {
        [Header("Common & Physics")]
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float LootCollectRange { get; private set; } = 3f;
        [field: SerializeField] public float MinFallVelocityForAction { get; private set; } = -2f;
        [field: SerializeField] public LayerMask GroundMask { get; private set; }

        [Space(10)]
        [SerializeField] private MovementSettings _movement = new();
        [SerializeField] private JumpSettings _jump = new();
        [SerializeField] private DashSettings _dash = new();
        [SerializeField] private GlideSettings _glide = new();
        [SerializeField] private WallHangSettings _wallHang = new();
        [SerializeField] private WallJumpSettings _wallJump = new(); // Добавлено
        [SerializeField] private SlideSettings _slide = new();
        [SerializeField] private PlungeSettings _plunge = new();
        [SerializeField] private SlopeSettings _slope = new();
        [SerializeField] private AttackSettings _attack = new();
        [SerializeField] private LifeCycleSettings _lifeCycle = new();
        [SerializeField] private ThrowableSettings _throwables = new();

        // Геттеры
        public MovementSettings Movement => _movement;
        public JumpSettings Jump => _jump;
        public DashSettings Dash => _dash;
        public GlideSettings Glide => _glide;
        public WallHangSettings WallHang => _wallHang;
        public WallJumpSettings WallJump => _wallJump; // Добавлено
        public SlideSettings Slide => _slide;
        public PlungeSettings Plunge => _plunge;
        public SlopeSettings Slope => _slope;
        public AttackSettings Attack => _attack;
        public LifeCycleSettings LifeCycle => _lifeCycle;
        public ThrowableSettings Throwables => _throwables;
    }

    // --- ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ ---

    [Serializable]
    public class MovementSettings
    {
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField, Min(0)] public float MoveSpeedMin { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 15f;
    }

    [Serializable]
    public class JumpSettings
    {
        [field: SerializeField, Min(0)] public float JumpForce { get; private set; } = 12f;
        [field: SerializeField, Min(0)] public float JumpForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float JumpChargeTime { get; private set; } = 0.5f;
        [field: SerializeField, Min(1)] public int MaxJumps { get; private set; } = 1;
    }

    [Serializable]
    public class DashSettings
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.3f;
        [field: SerializeField] public float ForceMin { get; private set; } = 8f;
        [field: SerializeField] public float ForceMax { get; private set; } = 20f;
        [field: SerializeField] public float ChargeTime { get; private set; } = 0.4f;
        [field: SerializeField] public float Cooldown { get; private set; } = 0.5f;
        [field: SerializeField] public float AirMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float VerticalBoost { get; private set; } = 3f;
        [field: SerializeField] public float Damage { get; private set; } = 25f;
        [field: SerializeField] public Vector2 HitboxSize { get; private set; } = new Vector2(1f, 1f);
    }

    [Serializable]
    public class GlideSettings
    {
        [field: SerializeField] public float HorizontalDrag { get; private set; } = 3f;
        [field: SerializeField] public float MaxFallSpeed { get; private set; } = -2f;
        [field: SerializeField] public float SpeedDamping { get; private set; } = 5f;
        [field: SerializeField] public float BounceForce { get; private set; } = 4f;
        [field: SerializeField] public float MinFallVelocity { get; private set; } = -3f;
        [field: SerializeField] public float SnapSpeed { get; private set; } = 40f;
        [field: SerializeField] public float SnapDuration { get; private set; } = 0.15f;
    }

    [Serializable]
    public class WallJumpSettings
    {
        [field: SerializeField] public float MinVelocityY { get; private set; } = 5f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(12f, 15f);
        [field: SerializeField] public float ControlLockDuration { get; private set; } = 0.2f;
        [field: SerializeField] public float WallCheckDistance { get; private set; } = 0.1f;
    }

    [Serializable]
    public class WallHangSettings
    {
        [field: SerializeField] public LayerMask Layer { get; private set; }
        [field: SerializeField] public float SlideSpeed { get; private set; } = 1f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(8f, 12f);
    }

    [Serializable]
    public class SlideSettings
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.4f;
        [field: SerializeField] public float Speed { get; private set; } = 15f;
    }

    [Serializable]
    public class PlungeSettings
    {
        [field: SerializeField] public float Speed { get; private set; } = 25f;
        [field: SerializeField] public float AOERadius { get; private set; } = 3f;
        [field: SerializeField] public float AOEDamage { get; private set; } = 50f;
        [field: SerializeField] public float KnockbackForce { get; private set; } = 10f;
    }

    [Serializable]
    public class SlopeSettings
    {
        [field: SerializeField] public float BoostMultiplier { get; private set; } = 2f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(10f, 6f);
        [field: SerializeField] public LayerMask Mask { get; private set; }
    }

    [Serializable]
    public class AttackSettings
    {
        [Header("Timings")]
        [field: SerializeField] public float ProcessTime { get; private set; } = 1.5f;
        [field: SerializeField] public float DelayTime { get; private set; } = 0.75f;
        [field: SerializeField] public float Cooldown { get; private set; } = 1f;

        [Header("Combat Parameters")]
        [field: SerializeField] public float InstantDamage { get; private set; } = 50f;
        [field: SerializeField] public float Range { get; private set; } = 1.5f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
        [field: SerializeField] public float InvulnerabilityDuration { get; private set; } = 0.2f;

        [Header("Hit Bounce (Physics)")]
        [field: SerializeField] public float HitBounceForce { get; private set; } = 8f;
        [field: SerializeField] public Vector2 GroundHitBounceModifiers { get; private set; } = new Vector2(0.7f, 0.4f);
        [field: SerializeField] public Vector2 AirHitBounceModifiers { get; private set; } = new Vector2(0.7f, 0.8f);

        [Header("Hit Stop (Juice)")]
        [field: SerializeField, Range(0f, 1f)] public float HitStopScale { get; private set; } = 0.05f;
        [field: SerializeField] public float HitStopDuration { get; private set; } = 0.15f;
    }

    [Serializable]
    public class LifeCycleSettings
    {
        [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 2f;
        [field: SerializeField] public float SpawnProcessTime { get; private set; } = 2f;
    }

    [Serializable]
    public class ThrowableSettings
    {
        [field: SerializeField] public GrappleHookConfig GrappleConfig { get; private set; }
        [field: SerializeField] public ShurikenConfig ShurikenConfig { get; private set; }
        [field: SerializeField] public SleepDartConfig SleepDartConfig { get; private set; }
    }
}