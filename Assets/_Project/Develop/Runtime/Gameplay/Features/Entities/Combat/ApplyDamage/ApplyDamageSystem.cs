using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Diagnostics;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem
    {
        private ICompositeCondition _canApplyDamage;

        private ReactiveEvent<DamageData> _damageRequest;
        private ReactiveEvent<DamageData> _damageEvent;

        private ReactiveVariable<float> _currentHealth;

        private IDisposable _requestDisposable;

        public void OnInit(Entity entity)
        {
            _canApplyDamage = entity.CanApplyDamage;

            _damageRequest = entity.TakeDamageRequest;
            _damageEvent = entity.TakeDamageEvent;

            _currentHealth = entity.CurrentHealth;

            _requestDisposable = _damageRequest.Subscribe(OnDamageRequest);
        }

        private void OnDamageRequest(DamageData damage)
        {
            if (_canApplyDamage.Evaluate() == false)
                return;

            _currentHealth.Value = MathF.Max(_currentHealth.Value - damage.Amount, 0);

            _damageEvent.Invoke(damage);
        }

        public void OnDispose() => _requestDisposable.Dispose();
    }
}