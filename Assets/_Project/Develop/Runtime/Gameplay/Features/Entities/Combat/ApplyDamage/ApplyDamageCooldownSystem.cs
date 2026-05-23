using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageCooldownSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveEvent<DamageData> _damageEvent;

        private ReactiveVariable<float> _damageCooldown;
        private ReactiveVariable<float> _damageCooldownTimer;

        private IDisposable _eventDisposable;

        public void OnInit(Entity entity)
        {
            _damageEvent = entity.TakeDamageEvent;

            _damageCooldown = entity.DamageCooldown;
            _damageCooldownTimer = entity.DamageCooldownTimer;

            _eventDisposable = _damageEvent.Subscribe(OnDamageEvent);
        }

        private void OnDamageEvent(DamageData damage)
        {
            _damageCooldownTimer.Value = _damageCooldown.Value;
        }

        public void OnDispose() => _eventDisposable.Dispose();

        public void OnUpdate(float deltaTime)
        {
            if (_damageCooldownTimer.Value > 0)
            {
                _damageCooldownTimer.Value -= deltaTime;
            }
        }
    }
}