using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    public abstract class ThrowableConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 15f;
        [field: SerializeField] public float MaxDistance { get; private set; } = 10f;
        [field: SerializeField] public float MinDistance { get; private set; } = 2f;
        [field: SerializeField] public int MaxCharges { get; private set; } = 3;
        [field: SerializeField] public LayerMask HitMask { get; private set; }
    }
}
