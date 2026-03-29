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

        // Константы для настройки баланса
        private const float BaseKnockbackX = 3f;    // Минимальный толчок по X
        private const float BaseKnockbackY = 2f;     // Минимальный подброс вверх
        private const float DamageMultiplier = 1.2f; // На сколько умножаем каждый хитпоинт урона
        private const float MaxForce = 60f;          // Ограничитель, чтобы при огромном уроне не улететь за карту

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _eventSubscription = entity.TakeDamageEvent.Subscribe(OnTakeDamage);
        }

        private void OnTakeDamage(DamageData damage)
        {
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
        }

        public void OnDispose() => _eventSubscription?.Dispose();
    }
}