using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes
{
    [CreateAssetMenu(fileName = "MainShipConfig", menuName = "Configs/Gameplay/Entities/New Main Ship Config")]
    public class MainShipConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Vehicles/MainShip";
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 3;
        [field: SerializeField, Min(0)] public float MoveSinkSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)] public float RotationSinkSpeed { get; private set; } = 5;
    }
}
