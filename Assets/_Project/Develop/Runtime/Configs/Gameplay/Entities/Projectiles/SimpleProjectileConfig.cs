using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles
{
    [CreateAssetMenu(fileName = "SimpleProjectileConfig", menuName = "Configs/Gameplay/Projectiles/New Simple Projectile Config")]
    public class SimpleProjectileConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Projectiles/SimpleProjectile";
        [field: SerializeField] public float GravityScale { get; set; } = 10;
    }
}
