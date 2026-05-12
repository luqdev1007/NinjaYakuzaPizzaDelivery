using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DamageKnockbackSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _takeDamageEvent;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            // _takeDamageEvent = entity.TakeDamageEvent.Subscribe(OnTakeDamage);
        }

        private void OnTakeDamage(DamageData damage)
        {
            /*
            if (_entity.Rigidbody == null)
                return;

            // Определяем направление (противоположное взгляду)
            float lookDirection = Mathf.Sign(_entity.Transform.localScale.x);
            float pushDirectionX = -lookDirection;

            // Рассчитываем силу на основе урона
            // Итоговая сила = База + (Урон * Множитель)
            float finalForceX = BaseKnockbackX + (damage.Amount * DamageMultiplier);
            float finalForceY = BaseKnockbackY + (damage.Amount * 0.5f); // По Y добавляем чуть меньше, чтобы не подлетать до потолка

            // Ограничиваем максимальный импульс
            finalForceX = Mathf.Min(finalForceX, MaxForce);

            _entity.Rigidbody.linearVelocity = Vector2.zero;

            Vector2 impulse = new Vector2(pushDirectionX * finalForceX, finalForceY);
            _entity.Rigidbody.AddForce(impulse, ForceMode2D.Impulse);
            */
        }

        public void OnDispose() => _takeDamageEvent?.Dispose();
    }
}