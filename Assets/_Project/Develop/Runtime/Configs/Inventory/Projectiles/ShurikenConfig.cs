using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "New Shuriken Config", menuName = "Configs/Gameplay/Projectiles/Shuriken")]
    public class ShurikenConfig : ThrowableItemConfig
    {
        [field: SerializeField] public int Damage { get; private set; } = 10;
    }
}