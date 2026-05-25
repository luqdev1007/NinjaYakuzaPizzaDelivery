using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Inventory.Potions
{
    [CreateAssetMenu(fileName = "New Potion Config", menuName = "Configs/Gameplay/Potions/New Potion Config")]
    public class PotionItemConfig : InventoryItemConfig
    {
        [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float Duration { get; private set; } = 5f;
    }
}
