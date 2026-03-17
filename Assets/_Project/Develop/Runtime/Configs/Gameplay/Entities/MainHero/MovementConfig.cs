using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask TraversableLayers { get; private set; }
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField, Min(0)] public float MoveSpeedMin { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 15f;
    }
}