using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    [CreateAssetMenu(fileName = "ShurikenConfig", menuName = "Configs/Throwable/Shuriken")]
    public class ShurikenConfig : ThrowableConfig
    {
        [field: SerializeField] public float Damage { get; private set; } = 20f;
        [field: SerializeField] public bool PenetrateEnemies { get; private set; } = false;
    }
}