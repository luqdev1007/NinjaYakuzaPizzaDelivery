using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
    [CreateAssetMenu(fileName = "GhostConfig", menuName = "Configs/Gameplay/Entities/Enemies/New Ghost Config")]
    public class GhostConfig : EntityConfig
    {
        [field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Enemies/Ghost";

        [Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 5.5f;
        [field: SerializeField] public float LinearDrag { get; private set; } = 2f;

        [Header("AI Behaviour")]
        // Тайминги фаз блуждания. Раньше были захардкожены в
        // BrainsFactory.CreateRandomMovementStateMachine, а DirectionChangeCooldown
        // висел в конфиге мёртвым — теперь он и есть период смены направления.
        // Значения — ПЛЕЙСХОЛДЕРЫ под ручную подгонку в инспекторе.
        [Tooltip("Как часто призрак выбирает новое случайное направление внутри фазы движения")]
        [field: SerializeField] public float DirectionChangeCooldown { get; private set; } = 0.5f;

        [Tooltip("Длительность фазы движения")]
        [field: SerializeField] public float MovementPhaseDuration { get; private set; } = 1.5f;

        [Tooltip("Длительность фазы простоя между фазами движения")]
        [field: SerializeField] public float IdlePhaseDuration { get; private set; } = 1f;

        // Разброс на экземпляр против «синхронных клонов»: все призраки спавнятся в
        // одном кадре и раньше брали из конфига идентичные длительности, из-за чего
        // фазы щёлкали в унисон. Одно поле на все три длительности — отдельные ручки
        // на каждую пока не на что опереть.
        //
        // Значения берутся из ЗАСЕЯННОГО потока (IGameplayRandom), а не из глобального
        // Random: длительности фаз определяют, когда призрак движется и где окажется,
        // то есть это реплей-чувствительный путь наравне с направлением. Десинхрон при
        // этом не страдает — разным призракам выпадают разные значения и из seeded-потока,
        // а забег остаётся воспроизводимым.
        [Tooltip("Разброс длительностей фаз на экземпляр: доля от значения (0.25 = ±25%). " +
                 "Плейсхолдер под ручную подгонку. 0 = все призраки идентичны и синхронны.")]
        [field: SerializeField, Range(0f, 0.9f)] public float PhaseDurationJitter { get; private set; } = 0.25f;

        [Header("Life Cycle")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
        [field: SerializeField] public float DamageCooldown { get; private set; } = 0.1f;
        [field: SerializeField] public float DeathProcessTime { get; private set; } = 0.5f;

        [Header("Combat")]
        [field: SerializeField] public float ContactDamage { get; private set; } = 1f;
        [field: SerializeField] public LayerMask ContactLayerMask { get; private set; }

        [Header("Loot")]
        [field: SerializeField] public LootTableConfig LootTable { get; private set; }

        // Длительность knockback-окна после удара мечом. Переименовано из KnockbackTimer:
        // имя врало — это не таймер, а константа-граница для KnockbackElapsedTime.
        // FormerlySerializedAs по имени backing-поля автосвойства, чтобы не потерять
        // выставленное в ассете значение (0.3).
        [field: SerializeField]
        [field: FormerlySerializedAs("<KnockbackTimer>k__BackingField")]
        public float KnockbackDuration { get; private set; }
    }
}
