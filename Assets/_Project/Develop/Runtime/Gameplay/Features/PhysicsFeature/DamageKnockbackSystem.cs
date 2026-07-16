using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DamageKnockbackSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _takeDamageEvent;
        private ReactiveVariable<float> _knockbackElapsedTime;

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _knockbackElapsedTime = entity.KnockbackElapsedTime;

            _takeDamageEvent = entity.TakeDamageEvent.Subscribe(OnTakeDamage);
        }

        private void OnTakeDamage(DamageData damage)
        {
            float forceMagnitude = damage.KnockbackForce.magnitude;

            // Knockback-окно открывается только от РЕАЛЬНОГО отбрасывания (удар меча).
            // Контактный урон приходит с KnockbackForce = Vector2.zero — он не должен
            // ни гасить velocity, ни останавливать движение призрака вообще.
            if (forceMagnitude <= 0f)
            {
                return;
            }

            if (_entity.Rigidbody == null)
            {
                return;
            }

            _entity.Rigidbody.linearVelocity = Vector2.zero;

            float pushDirectionX = _entity.Transform.position.x >= damage.SourcePosition.x ? 1f : -1f;
            Vector2 arcDirection = new Vector2(pushDirectionX * 1.2f, 1.0f).normalized;

            Vector2 finalArcForce = arcDirection * forceMagnitude;

            // Обнуление elapsed открывает окно: пока elapsed < duration, canMove = false
            // и арка отбрасывания оседает, не перетираясь движением.
            _knockbackElapsedTime.Value = 0;
            _entity.Rigidbody.AddForce(finalArcForce, ForceMode2D.Impulse);
        }

        public void OnDispose() => _takeDamageEvent?.Dispose();
    }
}
