using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem
    {
        private ICompositeCondition _canApplyDamage;

        private ReactiveEvent<DamageData> _damageRequest;
        private ReactiveEvent<DamageData> _damageEvent;

        private ReactiveVariable<float> _health;

        private IDisposable _requestDisposable;

        public void OnInit(Entity entity)
        {
            /*
            
            _damageRequest = entity.TakeDamageRequest;
            _damageEvent = entity.TakeDamageEvent;
            _health = entity.CurrentHealth;
            _canApplyDamage = entity.CanApplyDamage;
            _cooldownTimer = entity.DamageCooldownTimer;

            _requestDisposable = _damageRequest.Subscribe(OnDamageRequest);
            */
        }

        private void OnDamageRequest(DamageData damage)
        {
            if (_canApplyDamage.Evaluate() == false)
                return;

            _health.Value = MathF.Max(_health.Value - damage.Amount, 0);

            _damageEvent.Invoke(damage);
        }

        public void OnDispose() => _requestDisposable.Dispose();
    }
}