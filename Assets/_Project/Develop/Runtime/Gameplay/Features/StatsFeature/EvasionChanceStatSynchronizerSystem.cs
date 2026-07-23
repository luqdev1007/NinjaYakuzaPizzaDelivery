using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    /// <summary>
    /// Синхронизатор шанса уклонения. Схема — как у MoveSpeedStatSynchronizerSystem:
    /// база + фолд модификаторов, пересчёт на StatModifiersList.Changed и на смену
    /// базы, плюс разовый Recalculate в OnInit (иначе до первого баффа в итоговом
    /// компоненте лежал бы ноль).
    ///
    /// ОТЛИЧИЕ ОТ ЭТАЛОНА, И ОНО НАМЕРЕННОЕ: кламп здесь ДВУСТОРОННИЙ, 0..100.
    /// MoveSpeed и LootCollectRange клампят только низ — у скорости и радиуса нет
    /// осмысленного потолка. У шанса он есть: пара мультипликативных баффов поверх
    /// базовых 50% дала бы больше 100 и превратила уклонение в постоянную
    /// неуязвимость. Верхний кламп — балансный предохранитель, не косметика.
    ///
    /// Сам бросок при этом устроен так, что 100 читается как гарантированный
    /// уворот (ApplyDamageSystem.IsEvaded), поэтому кламп сверху не создаёт
    /// «почти всегда, но иногда нет» — он даёт ровный потолок.
    /// </summary>
    public class EvasionChanceStatSynchronizerSystem : IInitializableSystem, IDisposableSystem
    {
        private const float MinChancePercent = 0f;
        private const float MaxChancePercent = 100f;

        private ReactiveVariable<float> _baseEvasionChance;
        private StatModifiersList _modifiers;
        private ReactiveVariable<float> _evasionChance;

        private IDisposable _baseChangedDisposable;

        public void OnInit(Entity entity)
        {
            _baseEvasionChance = entity.BaseEvasionChance;
            _modifiers = entity.EvasionChanceModifiers;
            _evasionChance = entity.EvasionChance;

            _modifiers.Changed += Recalculate;
            _baseChangedDisposable = _baseEvasionChance.Subscribe(OnBaseChanged);

            Recalculate();
        }

        public void OnDispose()
        {
            _modifiers.Changed -= Recalculate;
            _baseChangedDisposable.Dispose();
        }

        private void OnBaseChanged(float previousValue, float newValue)
        {
            Recalculate();
        }

        private void Recalculate()
        {
            float result = _baseEvasionChance.Value;

            foreach (IStatModifier modifier in _modifiers.Elements)
            {
                result = modifier.Apply(result);
            }

            if (result < MinChancePercent)
            {
                result = MinChancePercent;
            }

            if (result > MaxChancePercent)
            {
                result = MaxChancePercent;
            }

            _evasionChance.Value = result;
        }
    }
}
