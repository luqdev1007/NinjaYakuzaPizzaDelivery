using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "GhostConfig", menuName = "Configs/Gameplay/Entities/Enemies/New Ghost Config")]
    public class GhostConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Enemies/Ghost";

        [Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 2f;
        [field: SerializeField] public float LinearDrag { get; private set; } = 2f;
        [field: SerializeField] public float AngularDrag { get; private set; } = 2f;
        [field: SerializeField] public float DirectionChangeCooldown { get; private set; } = 2f;


        [Header("Life Cycle")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
        [field: SerializeField] public float DamageCooldown { get; private set; } = 0.1f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 0.5f;

        [Header("Combat")]
        [field: SerializeField] public float ContactDamage { get; private set; } = 1f;
        [field: SerializeField] public LayerMask ContactLayerMask { get; private set; }

        [Header("Loot")]
        [field: SerializeField] public LootTableConfig LootTable { get; private set; }
    }
}