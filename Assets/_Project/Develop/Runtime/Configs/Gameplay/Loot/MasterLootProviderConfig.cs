using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Loot
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Loot/Master Loot Provider", fileName = "New Master Loot Provider Config")]
    public class MasterLootProviderConfig : ScriptableObject
    {
        [field: SerializeField] public LootTableConfig EnemyLoot { get; private set; }
        [field: SerializeField] public LootTableConfig SecretChestLoot { get; private set; }
        [field: SerializeField] public LootTableConfig PropsLoot { get; private set; }
    }
}
