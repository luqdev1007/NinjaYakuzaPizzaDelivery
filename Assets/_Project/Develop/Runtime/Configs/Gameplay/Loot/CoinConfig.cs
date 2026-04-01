using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    // Золотые монеты
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/Coin", fileName = "CoinConfig")]
    public class CoinConfig : LootConfig
    {
        [field: SerializeField] public int CoinAmount { get; private set; }
    }
}