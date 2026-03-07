using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "Configs/Gameplay/Entities/New Hero Config")]
    public class HeroConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Hero";

        [field: SerializeField] public float MinFallVelocityForAction { get; private set; } = -2f;

        [Header("Glide Settings")]
        [field: SerializeField] public float GlideMaxFallSpeed { get; private set; } = -2f;
        [field: SerializeField] public float GlideSpeedDamping { get; private set; } = 5f;
        [field: SerializeField] public float GlideBounceForce { get; private set; } = 4f;

        [Header("Dash Settings")]
        [field: SerializeField, Min(0)] public float DashDuration { get; private set; } = 0.3f;
        [field: SerializeField, Min(0)] public float DashForceMin { get; private set; } = 8f;
        [field: SerializeField, Min(0)] public float DashForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float DashChargeTime { get; private set; } = 0.4f;
        [field: SerializeField, Min(0)] public float DashCooldown { get; private set; } = 0.5f;

        [Header("Movement Settings")]
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10;
        [field: SerializeField, Min(0)] public float MoveSpeedMin { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 15f;

        [Header("Jump Settings")]
        [field: SerializeField, Min(0)] public float JumpForce { get; private set; } = 12f;
        [field: SerializeField, Min(0)] public float JumpForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float JumpChargeTime { get; private set; } = 0.5f;
        [field: SerializeField, Min(1)] public int MaxJumps { get; private set; } = 1;

        [Header("Physics")]
        [field: SerializeField] public LayerMask GroundMask { get; private set; }

        [Header("Attack Settings")]
        [field: SerializeField, Min(0)] public float AttackProcessTime { get; private set; } = 1.5f;
        [field: SerializeField, Min(0)] public float AttackDelayTime { get; private set; } = 0.75f;
        [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField, Min(0)] public float InstantAttackDamage { get; private set; } = 50;
        [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
        [field: SerializeField] public float HitBounceForce { get; private set; } = 8f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }

        [Header("Life Cycle Settings")]
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
        [field: SerializeField, Min(0)] public float SpawnProcessTime { get; private set; } = 2;
    }
}