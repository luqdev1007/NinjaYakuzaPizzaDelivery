using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Hero/Abilities/Movement")]
    public class MovementAbilityConfig : AbilityConfig
    {
        [field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField, Min(0)] public float MoveSpeedMin { get; private set; } = 3f;
        [field: SerializeField, Min(0)] public float Acceleration { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float Deceleration { get; private set; } = 15f;

        [Header("Slope Physics")]
        [field: SerializeField] public LayerMask SlopeMask { get; private set; }
        [field: SerializeField, Range(0, 90)] public float MaxSlopeAngle { get; private set; } = 45f;
        [field: SerializeField, Range(0, 90)] public float MinSlopeAngle { get; private set; } = 45f;
    }
}