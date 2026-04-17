using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [Serializable]
    public class LootDropEntry
    {
        [field: SerializeField] public LootConfig Config { get; private set; }
        [field: SerializeField] public int Weight { get; private set; } = 1; 
        [field: SerializeField] public Vector2Int CountRange { get; private set; } = new Vector2Int(1, 1); 
    }

    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/LootTable", fileName = "NewLootTable")]
    public class LootTableConfig : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private Vector2Int _totalDropCount = new Vector2Int(5, 10); 

        [Header("Loot List")]
        [SerializeField] private List<LootDropEntry> _lootEntries;

        public Vector2Int TotalDropCount => _totalDropCount;
        public IReadOnlyList<LootDropEntry> LootEntries => _lootEntries;
    }
}