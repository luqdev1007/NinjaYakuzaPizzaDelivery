using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "Configs/Gameplay/Entities/New Hero Config")]
    public class HeroConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Hero";

        [Header("Movement Settings")]
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10;

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

        [Header("Life Cycle Settings")]
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
        [field: SerializeField, Min(0)] public float SpawnProcessTime { get; private set; } = 2;
    }
}