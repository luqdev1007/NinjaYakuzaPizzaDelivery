using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/New Coin Loot Config", fileName = "New Coin Loot Config")]
    public class CoinLootConfig : LootConfig
    {
        [field: SerializeField] public float BaseAmount { get; private set; }
    }
}