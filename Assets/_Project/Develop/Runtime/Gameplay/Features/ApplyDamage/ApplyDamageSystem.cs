using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private IDisposable _requestSubscription;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _requestSubscription = _entity.TakeDamageRequest.Subscribe(OnDamageRequest);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.DamageCooldownTimer.Value > 0)
            {
                _entity.DamageCooldownTimer.Value -= deltaTime;
            }
        }

        private void OnDamageRequest(DamageData damage)
        {
            if (_entity.CanApplyDamage.Evaluate() == false)
                return;

            _entity.CurrentHealth.Value = MathF.Max(_entity.CurrentHealth.Value - damage.Amount, 0);

            _entity.DamageCooldownTimer.Value = _entity.DamageCooldown.Value;

            _entity.TakeDamageEvent.Invoke(damage);

            // new
            if (damage.SourcePosition != Vector2.zero)
            {
                var force = new Vector2(
                    _entity.DamageKnockbackForceX.Value * damage.SourcePosition.x,
                    _entity.DamageKnockbackForceY.Value
                );
                _entity.Rigidbody.AddForce(force, ForceMode2D.Impulse);
            }
        }

        public void OnDispose() => _requestSubscription?.Dispose();
    }
}