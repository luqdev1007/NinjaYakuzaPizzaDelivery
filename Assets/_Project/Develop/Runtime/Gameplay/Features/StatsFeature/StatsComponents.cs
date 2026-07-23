using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class BaseMoveSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class MoveSpeedModifiers : IEntityComponent
    {
        public StatModifiersList Value;
    }

    public class BaseLootCollectRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class LootCollectRangeModifiers : IEntityComponent
    {
        public StatModifiersList Value;
    }

    /// <summary>
    /// Базовый шанс уклонения В ПРОЦЕНТАХ, 0..100 — та же единица измерения, что у
    /// DoubleAttackChance, чтобы бросок читался одинаково во всех местах проекта.
    ///
    /// Итоговое значение считает EvasionChanceStatSynchronizerSystem и кладёт в
    /// компонент EvasionChance (он живёт в фиче-потребителе, рядом с
    /// ApplyDamageComponents) — ровно как BaseMoveSpeed → MoveSpeed.
    /// </summary>
    public class BaseEvasionChance : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class EvasionChanceModifiers : IEntityComponent
    {
        public StatModifiersList Value;
    }
}
