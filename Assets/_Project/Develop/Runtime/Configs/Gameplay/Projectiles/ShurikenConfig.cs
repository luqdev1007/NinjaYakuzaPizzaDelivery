using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "ShurikenConfig", menuName = "Configs/Gameplay/Projectiles/Shuriken")]
    public class ShurikenConfig : ThrowableConfig
    {
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public int PierceCount { get; private set; } = 3;
    }
}