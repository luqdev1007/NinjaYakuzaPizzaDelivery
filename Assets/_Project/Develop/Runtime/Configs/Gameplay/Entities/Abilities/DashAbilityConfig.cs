using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Hero/Abilities/Dash")]
    public class DashAbilityConfig : AbilityConfig
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.3f;
        [field: SerializeField] public float ForceMin { get; private set; } = 8f;
        [field: SerializeField] public float ForceMax { get; private set; } = 20f;
        [field: SerializeField] public float ChargeTime { get; private set; } = 0.4f;
        [field: SerializeField] public float Cooldown { get; private set; } = 0.5f;
        [field: SerializeField] public float AirMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float VerticalBoost { get; private set; } = 3f;
        [field: SerializeField] public float Damage { get; private set; } = 25f;
        [field: SerializeField] public Vector2 HitboxSize { get; private set; } = new Vector2(1f, 1f);
    }
}