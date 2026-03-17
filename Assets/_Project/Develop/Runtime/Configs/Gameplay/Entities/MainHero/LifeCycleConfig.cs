using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "LifeCycleConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Life Cycle Config")]
    public class LifeCycleConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100f;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2f;
        [field: SerializeField, Min(0)] public float SpawnProcessTime { get; private set; } = 2f;
    }
}
