using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "SpringConfig", menuName = "Configs/Gameplay/Entities/Level Objects/New Spring Config")]
    public class SpringConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/LevelObjects/Spring";
        [field: SerializeField, Min(0)] public int ChargesCount { get; private set; } = 3;
        [field: SerializeField, Min(0)] public float AppliyingForcePower { get; private set; } = 10;
    }
}
