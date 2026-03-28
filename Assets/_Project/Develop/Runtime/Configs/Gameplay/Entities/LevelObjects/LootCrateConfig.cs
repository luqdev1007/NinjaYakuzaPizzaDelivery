using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "LootCrateConfig", menuName = "Configs/Gameplay/Entities/Level Objects/New Loot Crate Config")]
    public class LootCrateConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/LevelObjects/LootCrate";
        [field: SerializeField, Min(0)] public int Durability { get; private set; } = 3;
    }
}
