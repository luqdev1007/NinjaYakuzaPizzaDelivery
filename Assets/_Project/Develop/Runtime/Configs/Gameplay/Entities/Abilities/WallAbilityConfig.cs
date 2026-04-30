using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Hero/Abilities/Wall Management")]
    public class WallAbilityConfig : AbilityConfig
    {
        [Header("Wall Hang")]
        [field: SerializeField] public LayerMask WallLayer { get; private set; }
        [field: SerializeField] public float SlideSpeed { get; private set; } = 1f;

        [Header("Wall Jump")]
        [field: SerializeField] public float VelocityYAbs { get; private set; } = 5f;
        [field: SerializeField] public Vector2 JumpForce { get; private set; } = new Vector2(12f, 15f);
        [field: SerializeField] public float ControlLockDuration { get; private set; } = 0.2f;
        [field: SerializeField] public float WallCheckDistance { get; private set; } = 0.1f;
    }
}