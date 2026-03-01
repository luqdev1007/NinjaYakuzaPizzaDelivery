using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes
{
    [CreateAssetMenu(fileName = "DriverConfig", menuName = "Configs/Gameplay/Entities/New Driver Config")]
    public class DriverConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Enemies/Driver";
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 10;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 3;
    }
}
