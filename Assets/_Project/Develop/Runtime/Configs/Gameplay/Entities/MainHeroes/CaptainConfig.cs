using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes
{
    [CreateAssetMenu(fileName = "CaptainConfig", order = 54, menuName = "Configs/Gameplay/Entities/New Captain Config")]
    public class CaptainConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Captain";
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 2;
        [field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 2;
        [field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
    }
}
