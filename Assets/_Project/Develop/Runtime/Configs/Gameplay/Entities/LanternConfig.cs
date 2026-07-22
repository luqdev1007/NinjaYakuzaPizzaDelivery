using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    /// <summary>
    /// Тётин-обакэ — стационарный ритмичный стрелок и точка зацепа крюком.
    /// Наследует EntityConfig НАПРЯМУЮ (как SlimeConfig): базового EnemyConfig в
    /// проекте нет, общий набор полей продублирован осознанно. Побочный плюс —
    /// не попадает в ловушку порядка веток pattern matching в EnemiesFactory.Create.
    ///
    /// БЕЗ ДВИЖЕНИЯ И БЕЗ RIGIDBODY. Фонарь стоит/висит на месте — ни патруля, ни
    /// мозга, ни движ-систем, ни Rigidbody2D. Точку зацепа крюком даёт слой
    /// Enemies(9) бесплатно (маска крюка его включает), отдельного флага не нужно.
    ///
    /// НАПРАВЛЕНИЕ ВЫСТРЕЛА per-instance задаётся В СЦЕНЕ через LanternAimAuthoring,
    /// а НЕ в конфиге: одинаковые фонари смотрят в разные стороны.
    /// </summary>
    [CreateAssetMenu(fileName = "LanternConfig", menuName = "Configs/Gameplay/Entities/Enemies/New Lantern Config")]
    public class LanternConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Enemies/Lantern";

        [Header("Life Cycle")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
        [field: SerializeField] public float DamageCooldown { get; private set; } = 0.1f;

        [Tooltip("Окно смерти, сек: сколько живёт труп, пока играется анимация сгорания " +
                 "бумажного фонаря и VFX. Как у слайма — без него труп исчез бы мгновенно " +
                 "(mustSelfRelease сработал бы в тот же тик, что и смерть)")]
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 0.6f;

        [Header("Combat")]
        [field: SerializeField] public float ContactDamage { get; private set; } = 1f;

        [Tooltip("Кого задевает телом. По образцу слайма/призрака — Characters(7)")]
        [field: SerializeField] public LayerMask ContactLayerMask { get; private set; }

        // KnockbackDuration НЕ заведено намеренно: knockback-систем у фонаря нет
        // (DamageKnockbackSystem упал бы на entity.Rigidbody без RB, а стационарному
        // врагу knockback и не нужен). Подробнее — в EntitiesFactory.CreateLantern.

        [Header("Loot")]
        [field: SerializeField] public LootTableConfig LootTable { get; private set; }

        [Header("Fire — ритм стрельбы")]
        [Tooltip("Период между выстрелами, сек. Метроном: фонарь стреляет БЕЗУСЛОВНО, " +
                 "без прицеливания, sight-check и поиска героя")]
        [field: SerializeField] public float FireCooldown { get; private set; } = 2f;

        [Tooltip("Длительность телеграфа перед выстрелом, сек. Окно на реакцию. Ведёт " +
                 "общий флаг IsTelegraphing — та же вьюха (TelegraphView), что у языка слайма")]
        [field: SerializeField] public float TelegraphDuration { get; private set; } = 0.3f;

        [Header("Projectile — снаряд")]
        [field: SerializeField] public string ProjectilePrefabPath { get; private set; } = "Entities/Projectiles/LanternProjectile";
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 6f;
        [field: SerializeField] public float ProjectileLifeTime { get; private set; } = 3f;
        [field: SerializeField] public float ProjectileContactDamage { get; private set; } = 1f;

        [Tooltip("На кого снаряд наносит контактный урон телом — Characters(7)")]
        [field: SerializeField] public LayerMask ProjectileTargetMask { get; private set; }

        // ProjectileSightBlockMask убрано: снаряд фонаря летит СКВОЗЬ СТЕНЫ,
        // деспавн об геометрию снят (см. LanternProjectileSystem). Ограничитель
        // дальности — только ProjectileLifeTime.
    }
}
