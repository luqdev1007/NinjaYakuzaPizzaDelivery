using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveEvent<DamageData> _damageRequest;
        private ReactiveEvent<DamageData> _damageEvent;

        private ReactiveVariable<float> _cooldownTimer;
        private float _defaultCooldown;

        private ReactiveVariable<float> _health;

        private ICompositeCondition _canApplyDamage;

        private IDisposable _requestDisposable;

        private string _entityName;

        public void OnInit(Entity entity)
        {
            _entityName = entity.Transform.gameObject.name;
            _damageRequest = entity.TakeDamageRequest;
            _damageEvent = entity.TakeDamageEvent;
            _health = entity.CurrentHealth;
            _canApplyDamage = entity.CanApplyDamage;
            _cooldownTimer = entity.DamageCooldownTimer;
            _defaultCooldown = entity.DamageCooldown.Value;

            _requestDisposable = _damageRequest.Subscribe(OnDamageRequest);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cooldownTimer.Value > 0)
            {
                _cooldownTimer.Value -= deltaTime;
                // Debug.Log($"Entity timer: {_cooldownTimer.Value}"); // Если этого лога нет в консоли — система не обновляется!
            }
        }

        private void OnDamageRequest(DamageData damage)
        {
            if (_canApplyDamage.Evaluate() == false)
                return;

            _health.Value = MathF.Max(_health.Value - damage.Amount, 0);
            _cooldownTimer.Value = _defaultCooldown; 
            _damageEvent.Invoke(damage);
        }

        public void OnDispose() => _requestDisposable.Dispose();
    }
}
