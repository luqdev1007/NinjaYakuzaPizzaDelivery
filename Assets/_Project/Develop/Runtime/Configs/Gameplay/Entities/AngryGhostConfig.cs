using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    /// <summary>
    /// Призрак-камикадзе. Наследует GhostConfig целиком: базовая сборка сущности
    /// в EntitiesFactory.CreateAngryGhost переиспользует CreateGhost, а фаза
    /// блуждания — существующий CreateRandomMovementStateMachine, которому нужны
    /// именно поля GhostConfig (тайминги фаз, джиттер, скорость, дрэг).
    ///
    /// ВНИМАНИЕ на порядок веток в EnemiesFactory.Create: из-за наследования
    /// pattern matching по GhostConfig поймает и AngryGhostConfig, поэтому
    /// case AngryGhostConfig обязан стоять выше.
    /// </summary>
    [CreateAssetMenu(fileName = "AngryGhostConfig", menuName = "Configs/Gameplay/Entities/Enemies/New Angry Ghost Config")]
    public class AngryGhostConfig : GhostConfig
    {
        // Все значения ниже — ПЛЕЙСХОЛДЕРЫ под ручную подгонку в инспекторе.

        [Header("Agro")]
        [Tooltip("Радиус, на котором призрак замечает героя и необратимо переходит в погоню")]
        [field: SerializeField] public float DetectionRadius { get; private set; } = 8f;

        [Tooltip("Скорость в фазе погони. Подменяет MoveSpeed при входе в ChaseTargetState")]
        [field: SerializeField] public float ChaseSpeed { get; private set; } = 5f;

        [Header("Arming")]
        [Tooltip("Радиус, на котором призрак замирает и начинает взводиться")]
        [field: SerializeField] public float ArmingRadius { get; private set; } = 2f;

        [Tooltip("Радиус срыва взведения. Обязан быть БОЛЬШЕ ArmingRadius — " +
                 "иначе на границе призрак задребезжит между погоней и взведением")]
        [field: SerializeField] public float DisarmRadius { get; private set; } = 3.5f;

        [Tooltip("Окно, в которое игрок успевает разорвать дистанцию или добить призрака")]
        [field: SerializeField] public float ArmingDuration { get; private set; } = 1f;

        [Header("Explosion")]
        [Tooltip("Радиус поражения. Может отличаться от ArmingRadius — взрыв обычно шире, " +
                 "чем точка, с которой он взводится")]
        [field: SerializeField] public float ExplosionRadius { get; private set; } = 3f;

        [field: SerializeField] public float ExplosionDamage { get; private set; } = 3f;

        [Tooltip("Кого задевает взрыв. Characters(7) + Enemies(9) + Props(15)")]
        [field: SerializeField] public LayerMask ExplosionLayerMask { get; private set; }

        [field: SerializeField] public float ExplosionKnockbackForce { get; private set; } = 12f;

        [Tooltip("Множитель отброса для вынужденной детонации (призрака убили до взведения). " +
                 "Урона при ней нет вообще, остаётся только импульс — держать ниже 1")]
        [field: SerializeField] public float ForcedExplosionKnockbackMultiplier { get; private set; } = 0.5f;
    }
}
