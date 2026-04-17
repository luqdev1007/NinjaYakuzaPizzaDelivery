using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/New Soul Shard Loot Config", fileName = "SoulShardLootConfig")]
    public class SoulShardLootConfig : LootConfig
    {
        [field: SerializeField] public float ExperienceAmount { get; private set; }
    }
}