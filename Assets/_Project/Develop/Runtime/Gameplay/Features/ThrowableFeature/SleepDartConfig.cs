using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    [CreateAssetMenu(fileName = "SleepDartConfig", menuName = "Configs/Throwable/SleepDart")]
    public class SleepDartConfig : ThrowableConfig
    {
        [field: SerializeField] public float SleepDuration { get; private set; } = 3f;
    }
}