using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "GhostConfig", menuName = "Configs/Gameplay/Enemies/New Ghost Config")]
    public class GhostConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Ghost";

        [Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 2f;
        [field: SerializeField] public float DirectionChangeCooldown { get; private set; } = 2f;

        [Header("Life Cycle")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 30f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 0.5f;

        [Header("Combat")]
        [field: SerializeField] public float ContactDamage { get; private set; } = 10f;
    }
}