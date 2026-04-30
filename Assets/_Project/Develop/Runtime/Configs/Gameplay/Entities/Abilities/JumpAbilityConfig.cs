using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Hero/Abilities/Jump")]
    public class JumpAbilityConfig : AbilityConfig
    {
        [field: SerializeField, Min(0)] public float JumpForce { get; private set; } = 12f;
        [field: SerializeField, Min(0)] public float JumpForceMax { get; private set; } = 20f;
        [field: SerializeField, Min(0)] public float JumpChargeTime { get; private set; } = 0.5f;
        [field: SerializeField, Min(1)] public int MaxJumps { get; private set; } = 1;
    }
}