using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "MainHeroConfig", menuName = "Configs/Gameplay/Main Hero/New Main Hero Config")]
    public class MaiHeroConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/MainHero/MainHero";
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float MoveSpeedMin { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float Deceleration { get; private set; }
    }
}