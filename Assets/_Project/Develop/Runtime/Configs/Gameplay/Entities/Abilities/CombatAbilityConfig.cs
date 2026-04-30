using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Abilities
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Hero/Abilities/Combat")]
    public class CombatAbilityConfig : AbilityConfig
    {
        [Header("Basic Attack")]
        [field: SerializeField] public float Damage { get; private set; } = 50f;
        [field: SerializeField] public float Range { get; private set; } = 1.5f;
        [field: SerializeField] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField] public float ProcessTime { get; private set; } = 1f;
        [field: SerializeField] public float DelayTime { get; private set; } = 1f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }

        [Header("Hit Settings")]
        [field: SerializeField] public float HitStopScale { get; private set; } = 0.05f;
        [field: SerializeField] public float HitStopDuration { get; private set; } = 0.15f;
        [field: SerializeField] public float HitBounceForce { get; private set; } = 8f;
        [field: SerializeField] public Vector2 GroundHitBounceModifiers { get; private set; }
        [field: SerializeField] public Vector2 AirHitBounceModifiers { get; private set; }
        [field: SerializeField] public float InvulnerabilityDuration { get; private set; } = 0.25f;

        [Header("Plunge Attack")]
        [field: SerializeField] public float PlungeSpeed { get; private set; } = 25f;
        [field: SerializeField] public float PlungeRadius { get; private set; } = 3f;
        [field: SerializeField] public float PlungeDamage { get; private set; } = 50f;
        [field: SerializeField] public float PlungeKnockbackForce { get; private set; } = 50f;
    }
}