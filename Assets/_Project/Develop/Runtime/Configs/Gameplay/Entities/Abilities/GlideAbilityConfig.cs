using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Hero/Abilities/Glide")]
    public class GlideAbilityConfig : AbilityConfig
    {
        [field: SerializeField] public float HorizontalDrag { get; private set; } = 3f;
        [field: SerializeField] public float MaxFallSpeed { get; private set; } = -2f;
        [field: SerializeField] public float SpeedDamping { get; private set; } = 5f;
        [field: SerializeField] public float BounceForce { get; private set; } = 4f;
        [field: SerializeField] public float MinFallVelocity { get; private set; } = -3f;
        [field: SerializeField] public float SnapSpeed { get; private set; } = 40f;
        [field: SerializeField] public float SnapDuration { get; private set; } = 0.15f;
        [field: SerializeField] public float CounterForceMultiplier { get; private set; } = 0.8f;
    }
}