using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHero
{
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "Configs/Gameplay/Entities/Main Hero/New Attack Config")]
    public class AttackConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float ProcessTime { get; private set; } = 1.5f;
        [field: SerializeField, Min(0)] public float DelayTime { get; private set; } = 0.75f;
        [field: SerializeField, Min(0)] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField, Min(0)] public float Damage { get; private set; } = 50f;
        [field: SerializeField] public float Range { get; private set; } = 1.5f;
        [field: SerializeField] public float HitBounceForce { get; private set; } = 8f;
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
    }
}