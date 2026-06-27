using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class LootCollectRangeStatSynchronizerSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _baseRange;
        private StatModifiersList _modifiers;
        private ReactiveVariable<float> _lootCollectRange;

        private IDisposable _baseChangedDisposable;

        public void OnInit(Entity entity)
        {
            _baseRange = entity.BaseLootCollectRange;
            _modifiers = entity.LootCollectRangeModifiers;
            _lootCollectRange = entity.LootCollectRange;

            _modifiers.Changed += Recalculate;
            _baseChangedDisposable = _baseRange.Subscribe(OnBaseChanged);

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
            float result = _baseRange.Value;

            foreach (IStatModifier modifier in _modifiers.Elements)
            {
                result = modifier.Apply(result);
            }

            if (result < 0f)
            {
                result = 0f;
            }

            _lootCollectRange.Value = result;
        }
    }
}