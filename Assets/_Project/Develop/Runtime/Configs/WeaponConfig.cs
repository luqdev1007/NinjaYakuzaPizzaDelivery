using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Weapons/New Weapon Config", order = 54)]
public class WeaponConfig : ScriptableObject
{
    [field: SerializeField][Min(0)] public float Damage { get; private set; } = 10;
    [field: SerializeField][Min(0)] public float ProjectileSpeed { get; private set; } = 10;
    [field: SerializeField][Min(0)] public float ShootsPerSecond { get; private set; } = 5;
    [field: SerializeField] public WeaponView ViewPrefab { get; private set; }
    [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
}


