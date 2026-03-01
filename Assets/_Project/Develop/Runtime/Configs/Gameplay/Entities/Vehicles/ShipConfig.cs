using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes
{
    [CreateAssetMenu(fileName = "SmallShipConfig", menuName = "Configs/Gameplay/Entities/New Small Ship Config")]
    public class SmallShipConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Vehicles/SmallShip";
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 2;
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 10;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 3;
        [field: SerializeField, Min(0)] public float BodyContactDamage { get; private set; } = 3;
        [field: SerializeField, Min(0)] public float RotationSpeed { get; private set; } = 30;
        [field: SerializeField, Min(0)] public float MoveSinkSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)] public float RotationSinkSpeed { get; private set; } = 5;
    }
}
