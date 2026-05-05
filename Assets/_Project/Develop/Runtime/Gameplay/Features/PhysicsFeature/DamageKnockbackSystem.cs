using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DamageKnockbackSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _eventSubscription;

        private const float MaxForceLimit = 60f;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _eventSubscription = _entity.TakeDamageEvent.Subscribe(OnTakeDamage);
        }

        private void OnTakeDamage(DamageData damage)
        {
            if (_entity.Rigidbody == null)
                return;

            float pushDirectionX;

            if (damage.SourcePosition != Vector2.zero)
            {
                pushDirectionX = damage.SourcePosition.x > _entity.Transform.position.x ? -1f : 1f;
            }
            else
            {
                pushDirectionX = -Mathf.Sign(_entity.Transform.localScale.x);
            }

            float baseForceX = _entity.DamageKnockbackForceX.Value;
            float baseForceY = _entity.DamageKnockbackForceY.Value;

            float finalForceX = baseForceX + (damage.Amount * 1.2f);
            float finalForceY = baseForceY + (damage.Amount * 0.5f);

            finalForceX = Mathf.Min(finalForceX, MaxForceLimit);

            _entity.Rigidbody.linearVelocity = Vector2.zero;

            Vector2 impulse = new Vector2(pushDirectionX * finalForceX, finalForceY);
            _entity.Rigidbody.AddForce(impulse, ForceMode2D.Impulse);

            Debug.Log($"[Knockback] Applied force: {impulse}. Damage source: {damage.SourcePosition}");
        }

        public void OnDispose() => _eventSubscription?.Dispose();
    }
}