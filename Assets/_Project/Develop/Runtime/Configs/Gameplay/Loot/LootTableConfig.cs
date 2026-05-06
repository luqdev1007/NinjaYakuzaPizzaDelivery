using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/LootTable", fileName = "New Loot Table Config")]
    public class LootTableConfig : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private Vector2Int _totalDropCountRange; 

        [Header("Loot List")]
        [SerializeField] private List<LootDropEntry> _lootEntries;

        public Vector2Int TotalDropCountRange => _totalDropCountRange;
        public IReadOnlyList<LootDropEntry> LootEntries => _lootEntries;
    }

    [Serializable]
    public class LootDropEntry
    {
        [field: SerializeField] public LootConfig Config { get; private set; }
        [field: SerializeField] public int Weight { get; private set; }
        [field: SerializeField] public Vector2Int CountRange { get; private set; }
    }
}