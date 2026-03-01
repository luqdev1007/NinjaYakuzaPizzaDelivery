using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Mines
{
    [CreateAssetMenu(fileName = "WaterMineConfig", menuName = "Configs/Gameplay/Entities/Mines/WaterMineConfig")]
    public class WaterMineConfig : ScriptableObject
    {
        [field: SerializeField] public string GhostPrefabPath { get; private set; }
        [field: SerializeField] public float MaxPlacementDistance { get; private set; } = 20f;
        [field: SerializeField] public float OverlapCheckRadius { get; private set; } = 1.5f;
    }
}