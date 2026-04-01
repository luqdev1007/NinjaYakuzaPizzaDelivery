using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/LootTable", fileName = "NewLootTable")]
    public class LootTableConfig : ScriptableObject
    {
        [SerializeField] private List<LootConfig> _possibleLoot;
        public IReadOnlyList<LootConfig> PossibleLoot => _possibleLoot;
    }
}