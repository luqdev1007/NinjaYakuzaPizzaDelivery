using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class MoveSpeedStatSynchronizerSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _baseMoveSpeed;
        private StatModifiersList _modifiers;
        private ReactiveVariable<float> _moveSpeed;

        private IDisposable _baseChangedDisposable;

        public void OnInit(Entity entity)
        {
            _baseMoveSpeed = entity.BaseMoveSpeed;
            _modifiers = entity.MoveSpeedModifiers;
            _moveSpeed = entity.MoveSpeed;

            _modifiers.Changed += Recalculate;
            _baseChangedDisposable = _baseMoveSpeed.Subscribe(OnBaseChanged);

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
            float result = _baseMoveSpeed.Value;

            foreach (IStatModifier modifier in _modifiers.Elements)
            {
                result = modifier.Apply(result);
            }

            if (result < 0f)
            {
                result = 0f;
            }

            _moveSpeed.Value = result;
        }
    }
}