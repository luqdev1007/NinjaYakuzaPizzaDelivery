using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "SlimeConfig", menuName = "Configs/Gameplay/Entities/Enemies/New Slime Config")]
    public class SlimeConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Enemies/Slime";

        [Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 2f;
        [field: SerializeField] public float LinearDrag { get; private set; } = 2f;
        [field: SerializeField] public float DirectionChangeCooldown { get; private set; } = 2f;


        [Header("Life Cycle")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
        [field: SerializeField] public float DamageCooldown { get; private set; } = 0.1f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 0.5f;

        [Header("Combat")]
        [field: SerializeField] public float ContactDamage { get; private set; } = 1f;

        [Header("Loot")]
        [field: SerializeField] public LootTableConfig LootTable { get; private set; }

        [Header("Grab Target Settings")]
        [field: SerializeField] public float MaxRange { get; private set; } = 10f;
        [field: SerializeField] public float GrabTime { get; private set; } = 2f;
        [field: SerializeField][Range(0f, 1f)] public float ReduceGrabTimePerHitPercent { get; private set; } = 1f;
        [field: SerializeField] public float GrabAttackSpeed { get; private set; } = 5f;
        [field: SerializeField] public float GrabBackSpeed { get; private set; } = 3f;
        // чета еще
    }
}