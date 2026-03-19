using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "ShurikenConfig", menuName = "Configs/Gameplay/Projectiles/Shuriken")]
    public class ShurikenConfig : ThrowableConfig
    {
        [field: SerializeField] public float Damage { get; private set; } = 20f;
        [field: SerializeField] public bool PenetrateEnemies { get; private set; } = false;
    }
}