using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce
{
    /// <summary>
    /// Применяет отскок от внешнего источника к rigidbody своей сущности.
    /// </summary>
    /// <remarks>
    /// Тикового канала нет намеренно — применяем прямо в колбэке запроса, как это
    /// делает DamageKnockbackSystem. Источник (трамплин) ловит коллизию в
    /// OnCollisionEnter2D, а тот стреляет внутри Physics2D.Simulate, т.е. уже в
    /// физ-контексте шага. Гонки нет: системы движения к этому моменту своё
    /// OnFixedUpdate уже отработали. Откладывать применение до следующего
    /// OnFixedUpdate означало бы +1 физ-шаг (20 мс) задержки перед стартом —
    /// изменение фила, которого этот рефактор избегает.
    ///
    /// Смысл правки: раньше трамплин писал velocity напрямую в чужой rigidbody мимо
    /// арбитража, и ни одна ECS-система об отскоке не знала. Теперь пишет система
    /// самой сущности — нарушение single-writer снято.
    ///
    /// Восходящая Y отскока переживает арбитраж так же, как прыжок: на следующем
    /// тике база либо сохраняет Y (вне склона), либо защищает её Y-guard'ом
    /// (на склоне, коммит 6012ab95) — guard читает живую velocity и ему всё равно,
    /// кто её записал.
    /// </remarks>
    public class BounceSystem : IInitializableSystem, IDisposableSystem
    {
        private Rigidbody2D _rigidbody;

        private IDisposable _requestDisposable;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;

            _requestDisposable = entity.BounceImpulseRequest.Subscribe(OnBounceRequest);
        }

        private void OnBounceRequest(BounceImpulseData bounce)
        {
            if (_rigidbody == null)
                return;

            Vector2 upAxis = bounce.UpAxis;

            // Гасим только компоненту вдоль оси отскока — тангенциальная скорость
            // сохраняется (та же семантика, что была в Trampoline).
            float currentUpVelocity = Vector2.Dot(_rigidbody.linearVelocity, upAxis);
            Vector2 tangentialVelocity = _rigidbody.linearVelocity - upAxis * currentUpVelocity;

            _rigidbody.linearVelocity = tangentialVelocity + upAxis * bounce.LaunchVelocity;
        }

        public void OnDispose()
        {
            _requestDisposable?.Dispose();
        }
    }
}
