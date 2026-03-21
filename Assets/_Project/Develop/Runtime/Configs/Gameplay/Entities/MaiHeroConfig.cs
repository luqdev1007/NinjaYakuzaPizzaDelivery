using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "MainHeroConfig", menuName = "Configs/Gameplay/Main Hero/New Main Hero Config")]
    public class MaiHeroConfig : EntityConfig
    {
        [Header("Common")]
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float MinFallVelocityForAction { get; private set; } = -2f;

        [Header("Physics")]
        [field: SerializeField] public LayerMask GroundMask { get; private set; }

        [Header("Movement")]
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField, Min(0)] public float MoveSpeedMin { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 15f;

        [Header("Jump")]
        [field: SerializeField, Min(0)] public float JumpForce { get; private set; } = 12f;
        [field: SerializeField, Min(0)] public float JumpForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float JumpChargeTime { get; private set; } = 0.5f;
        [field: SerializeField, Min(1)] public int MaxJumps { get; private set; } = 1;

        [Header("Dash")]
        [field: SerializeField, Min(0)] public float DashDuration { get; private set; } = 0.3f;
        [field: SerializeField, Min(0)] public float DashForceMin { get; private set; } = 8f;
        [field: SerializeField, Min(0)] public float DashForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float DashChargeTime { get; private set; } = 0.4f;
        [field: SerializeField, Min(0)] public float DashCooldown { get; private set; } = 0.5f;
        [field: SerializeField, Min(1)] public float AirDashMultiplier { get; private set; } = 1.5f;
        [field: SerializeField, Min(0)] public float AirDashVerticalBoost { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float DashDamage { get; private set; } = 25f;
        [field: SerializeField] public Vector2 DashHitboxSize { get; private set; } = new Vector2(1f, 1f);

        [Header("Glide")]
        [field: SerializeField] public float GlideMaxFallSpeed { get; private set; } = -2f;
        [field: SerializeField] public float GlideSpeedDamping { get; private set; } = 5f;
        [field: SerializeField] public float GlideBounceForce { get; private set; } = 4f;
        [field: SerializeField] public float GlideMinFallVelocity { get; private set; } = -3f;
        [field: SerializeField] public float GlideSnapSpeed { get; private set; } = 40f;
        [field: SerializeField] public float GlideSnapDuration { get; private set; } = 0.15f;

        [Header("Wall Hang")]
        [field: SerializeField] public LayerMask WallHangLayer { get; private set; }
        [field: SerializeField] public float WallHangSlideSpeed { get; private set; } = 1f;
        [field: SerializeField] public Vector2 WallJumpForce { get; private set; } = new Vector2(8f, 12f);

        [Header("Slide")]
        [field: SerializeField] public float SlideDuration { get; private set; } = 0.4f;
        [field: SerializeField] public float SlideSpeed { get; private set; } = 15f;

        [Header("Plunge")]
        [field: SerializeField] public float PlungeSpeed { get; private set; } = 25f;
        [field: SerializeField] public float PlungeAOERadius { get; private set; } = 3f;
        [field: SerializeField] public float PlungeAOEDamage { get; private set; } = 50f;
        [field: SerializeField] public float PlungeKnockbackForce { get; private set; } = 10f;

        [Header("Slope")]
        [field: SerializeField] public float SlopeBoostMultiplier { get; private set; } = 2f;
        [field: SerializeField] public Vector2 SlopeJumpForce { get; private set; } = new Vector2(10f, 6f);
        [field: SerializeField] public LayerMask SlopeMask { get; private set; }

        [Header("Attack")]
        [field: SerializeField, Min(0)] public float AttackProcessTime { get; private set; } = 1.5f;
        [field: SerializeField, Min(0)] public float AttackDelayTime { get; private set; } = 0.75f;
        [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField, Min(0)] public float InstantAttackDamage { get; private set; } = 50f;
        [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
        [field: SerializeField] public float HitBounceForce { get; private set; } = 8f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }

        [Header("Life Cycle")]
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100f;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2f;
        [field: SerializeField, Min(0)] public float SpawnProcessTime { get; private set; } = 2f;

        [Header("Throwables")]
        [field: SerializeField] public GrappleHookConfig GrappleConfig { get; private set; }
        [field: SerializeField] public ShurikenConfig ShurikenConfig { get; private set; }
        [field: SerializeField] public SleepDartConfig SleepDartConfig { get; private set; }
    }
}