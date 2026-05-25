using Assets._Project.Develop.Runtime.Configs.Inventory;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    public abstract class ThrowableItemConfig : InventoryItemConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public LayerMask HitMask { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public float MaxFlyDistance { get; private set; }
    }
}
