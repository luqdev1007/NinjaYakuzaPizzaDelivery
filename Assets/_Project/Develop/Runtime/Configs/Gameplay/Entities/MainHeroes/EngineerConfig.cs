using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes
{
    [CreateAssetMenu(fileName = "EngineerConfig", order = 54, menuName = "Configs/Gameplay/Entities/New Engineer Config")]
    public class EngineerConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHeroes/Engineer";
        [field: SerializeField] public float MaxHealth { get; private set; } = 2;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 2;
    }
}
