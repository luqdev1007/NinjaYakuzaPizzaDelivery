using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageCooldownSystem : IInitializableSystem, IDisposableSystem, IFixedUpdatableSystem
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

        // Окно неуязвимости при получении урона гейтит canApplyDamage, а урон от
        // контакта («скорость = урон») теперь считается на fixed — окно тикает там же,
        // чтобы совпадать с физикой контактов, а не отставать на кадр.
        public void OnFixedUpdate(float deltaTime)
        {
            if (_damageCooldownTimer.Value > 0)
            {
                _damageCooldownTimer.Value -= deltaTime;
            }
        }
    }
}
