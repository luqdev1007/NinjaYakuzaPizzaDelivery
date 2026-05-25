using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles
{
    [CreateAssetMenu(fileName = "New Sleep Dart Config", menuName = "Configs/Gameplay/Projectiles/SleepDart")]
    public class SleepDartConfig : ThrowableItemConfig
    {
        [field: SerializeField] public float SleepDuration { get; private set; } = 3f;
    }
}