using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "SleepDartConfig", menuName = "Configs/Gameplay/Projectiles/SleepDart")]
    public class SleepDartConfig : ThrowableConfig
    {
        [field: SerializeField] public float SleepDuration { get; private set; } = 3f;
    }
}