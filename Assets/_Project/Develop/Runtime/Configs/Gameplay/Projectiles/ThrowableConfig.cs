using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    public abstract class ThrowableConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public LayerMask HitMask { get; private set; }
        [field: SerializeField] public int MaxCharges { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public float MaxFlyDistance { get; private set; }
    }
}
