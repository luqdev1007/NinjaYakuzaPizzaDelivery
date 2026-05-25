using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Inventory
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerInventoryConfig : ScriptableObject
    {
        [SerializeField] private InventoryItemConfig[] _startingConsumables;

        public InventoryItemConfig[] StartingConsumables => _startingConsumables;
    }
}
