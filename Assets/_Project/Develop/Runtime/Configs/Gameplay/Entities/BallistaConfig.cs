using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "BallistaConfig", menuName = "Configs/Gameplay/Entities/New Ballista Config")]
    public class BallistaConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Ballista/Ballista";
    }
}
