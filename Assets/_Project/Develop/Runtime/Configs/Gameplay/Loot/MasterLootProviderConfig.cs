using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/Master Provider", fileName = "MasterLootProvider")]
    public class MasterLootProviderConfig : ScriptableObject
    {
        [field: SerializeField] public LootTableConfig EnemyLoot { get; private set; }
        [field: SerializeField] public LootTableConfig ChestLoot { get; private set; }
        [field: SerializeField] public LootTableConfig PropsLoot { get; private set; }
    }
}
