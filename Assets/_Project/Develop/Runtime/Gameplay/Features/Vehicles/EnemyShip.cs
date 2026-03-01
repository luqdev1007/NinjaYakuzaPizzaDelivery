using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles
{
    public class EnemyShip : MonoBehaviour
    {
        [field: SerializeField] public LevelConfig LevelConfig { get; private set; }
    }
}
