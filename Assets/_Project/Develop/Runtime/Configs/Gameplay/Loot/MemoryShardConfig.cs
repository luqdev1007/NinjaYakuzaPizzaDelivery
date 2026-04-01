using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    // Осколки памяти (валюта прокачки)
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/MemoryShard", fileName = "MemoryShardConfig")]
    public class MemoryShardConfig : LootConfig
    {
        [field: SerializeField] public float ExperienceAmount { get; private set; }
    }
}