using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/New Soul Shard Loot Config", fileName = "New Soul Shard Loot Config")]
    public class SoulShardLootConfig : LootConfig
    {
        [field: SerializeField] public int ExperienceAmount { get; private set; }
    }
}