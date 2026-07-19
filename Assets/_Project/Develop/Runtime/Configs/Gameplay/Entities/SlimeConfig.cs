using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    /// <summary>
    /// Наземный патрульный. Наследует EntityConfig НАПРЯМУЮ, а не GhostConfig:
    /// общий с призраком набор полей продублирован осознанно, базового EnemyConfig
    /// в проекте нет. Побочный плюс — слайм не попадает в ловушку порядка веток
    /// pattern matching в EnemiesFactory.Create (см. комментарий в AngryGhostConfig).
    ///
    /// ВАЖНО про «наземность»: слой Enemies(9) не сталкивается с Ground(8)/Wall(10)/
    /// Slope(11) в матрице коллизий проекта, поэтому слайм НЕ является физически
    /// наземным телом. Он едет «по рельсу» — рулёжкой скорости к текущей точке
    /// маршрута при gravityScale = 0, ровно как призрак. Высоту задают точки
    /// маршрута, которые дизайнер ставит на поверхность земли руками.
    /// </summary>
    [CreateAssetMenu(fileName = "SlimeConfig", menuName = "Configs/Gameplay/Entities/Enemies/New Slime Config")]
    public class SlimeConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Enemies/Slime";

        [Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 2f;
        [field: SerializeField] public float LinearDrag { get; private set; } = 2f;
        [field: SerializeField] public float DirectionChangeCooldown { get; private set; } = 2f;

        [Header("Patrol")]
        [Tooltip("Минимальная пауза на конце маршрута, сек")]
        [field: SerializeField] public float PatrolPauseMin { get; private set; } = 0.5f;

        [Tooltip("Максимальная пауза на конце маршрута, сек. Длительность каждой паузы " +
                 "тянется из засеянного IGameplayRandom — забег остаётся воспроизводимым")]
        [field: SerializeField] public float PatrolPauseMax { get; private set; } = 1.5f;

        [Tooltip("Радиус, внутри которого точка считается достигнутой. Он же — порог " +
                 "вырожденности маршрута: точки ближе этого значения считаются совпавшими")]
        [field: SerializeField] public float PatrolArriveDistance { get; private set; } = 0.15f;

        [Tooltip("Полудлина запасного маршрута [spawn.x - R, spawn.x + R] на высоте спавна. " +
                 "Работает, ТОЛЬКО если маршрут не задан в сцене через SlimePatrolRouteAuthoring")]
        [field: SerializeField] public float PatrolFallbackHalfRange { get; private set; } = 3f;

        [Header("Life Cycle")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
        [field: SerializeField] public float DamageCooldown { get; private set; } = 0.1f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 0.5f;

        [Header("Combat")]
        [field: SerializeField] public float ContactDamage { get; private set; } = 1f;

        [Tooltip("Кого задевает телом. По образцу GhostConfig — Characters(7)")]
        [field: SerializeField] public LayerMask ContactLayerMask { get; private set; }

        [Tooltip("Длительность knockback-окна после удара мечом: пока оно открыто, " +
                 "CanMove = false и арка отбрасывания оседает, не перетираясь движением")]
        [field: SerializeField] public float KnockbackDuration { get; private set; } = 1f;

        [Header("Loot")]
        [field: SerializeField] public LootTableConfig LootTable { get; private set; }

        // Поля языка. Часть из них заведена ещё на этапе 1 и переиспользуется
        // этапом 3a как есть: MaxRange — дальность языка, GrabAttackSpeed —
        // скорость полёта вперёд, GrabBackSpeed — скорость втягивания.
        //
        // GrabTime на этапе 3a НЕ ЧИТАЕТСЯ: удержание героя — это 3b.
        //
        // ReduceGrabTimePerHitPercent удалено: это была заготовка под маш-эскейп
        // (долбить атаку, чтобы вырваться), решение принято против неё — один
        // удар рвёт язык целиком.
        [Header("Grab Target Settings")]
        [field: SerializeField] public float MaxRange { get; private set; } = 10f;
        [field: SerializeField] public float GrabTime { get; private set; } = 2f;
        [field: SerializeField] public float GrabAttackSpeed { get; private set; } = 5f;
        [field: SerializeField] public float GrabBackSpeed { get; private set; } = 3f;

        [Header("Tongue")]
        [Tooltip("Препятствия для линии взгляда И для полёта языка. Выставлено " +
                 "Default(0) + Ground(8) + Wall(10) + Slope(11) + Props(15): геометрия " +
                 "Village.prefab размазана ровно по этим пяти слоям, Ground+Wall+Slope " +
                 "покрывает только 28 из 36 коллайдеров. Тот же набор — GroundMask героя.\n" +
                 "Имя поля намеренно НЕ WallMask: в Entity.cs живёт заглушка AddWallMask " +
                 "с throw new NotImplementedException рядом с генерённой перегрузкой")]
        [field: SerializeField] public LayerMask SightBlockMask { get; private set; }

        [Tooltip("На чём искать героя для выстрела языком. Characters(7)")]
        [field: SerializeField] public LayerMask TargetMask { get; private set; }

        [Tooltip("Длительность телеграфа перед плевком, сек. Окно, за которое игрок " +
                 "успевает среагировать. Условия входа в нём повторно НЕ проверяются: " +
                 "выстрел состоится даже если герой ушёл — промах это честная награда")]
        [field: SerializeField] public float TongueTelegraphDuration { get; private set; } = 0.25f;

        [Tooltip("Время отращивания языка после полного цикла, сек")]
        [field: SerializeField] public float TongueCooldown { get; private set; } = 3f;

        [Tooltip("Порог dot между направлением на героя и Vector2.up — нижняя граница арки.\n" +
                 "0 = ровно полусфера вверх. Отрицательные значения ОПУСКАЮТ нижнюю " +
                 "границу: -0.35 — примерно 20° ниже горизонта, -1 — полная сфера.\n" +
                 "Для наземного врага небольшое отрицательное значение ОБЯЗАТЕЛЬНО. " +
                 "ShootPoint стоит на высоте центра слайма, а пивот героя — у него в " +
                 "ногах, поэтому герой, стоящий на той же земле, оказывается НИЖЕ точки " +
                 "вылета (замерено: origin.y 15.96 против heroPos.y 15.35, dot = -0.085). " +
                 "При пороге 0 такой герой не проходит проверку, и слайм стреляет только " +
                 "по подпрыгнувшему")]
        [field: SerializeField] public float TongueAimArcDot { get; private set; } = -0.35f;

        [field: SerializeField] public string TonguePrefabPath { get; private set; } = "Entities/Projectiles/SlimeTongue";
    }
}
