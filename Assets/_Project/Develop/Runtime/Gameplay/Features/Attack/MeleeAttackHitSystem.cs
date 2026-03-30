using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _attackDelayEndDisposable;
        private readonly ICoroutinesPerformer _coroutines;

        // Кэшируем ссылки на компоненты для удобства доступа
        private ReactiveEvent _successfulHitEvent;
        private ReactiveVariable<float> _attackRange;
        private ReactiveVariable<float> _attackDamage;
        private ReactiveVariable<LayerMask> _enemyMask;

        private ReactiveVariable<float> _hitStopScale;
        private ReactiveVariable<float> _hitStopDuration;

        private ReactiveVariable<float> _hitBounceForce;
        private ReactiveVariable<Vector2> _groundBounceModifiers;
        private ReactiveVariable<Vector2> _airBounceModifiers;

        public MeleeAttackHitSystem(ICoroutinesPerformer coroutines)
        {
            _coroutines = coroutines;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            // Инициализируем ссылки из компонентов сущности
            _successfulHitEvent = _entity.SuccessfulHitEvent;
            _attackRange = _entity.AttackRange;
            _attackDamage = _entity.AttackDamage;
            _enemyMask = _entity.AttackEnemyMask;

            _hitStopScale = _entity.AttackHitStopScale;
            _hitStopDuration = _entity.AttackHitStopDuration;

            _hitBounceForce = _entity.AttackHitBounceForce;
            _groundBounceModifiers = _entity.GroundHitBounceModifiers;
            _airBounceModifiers = _entity.AirHitBounceModifiers;

            _attackDelayEndDisposable = _entity.AttackDelayEndEvent.Subscribe(OnAttackHit);
        }

        private void OnAttackHit()
        {
            float dir = _entity.Transform.localScale.x > 0 ? 1f : -1f;
            float range = _attackRange.Value;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                (Vector2)_entity.Transform.position + Vector2.right * dir * (range * 0.5f),
                range * 0.5f,
                _enemyMask.Value);

            if (hits.Length == 0) return;

            bool hitAny = false;
            foreach (var hit in hits)
            {
                var mono = hit.GetComponentInParent<MonoEntity>();
                if (mono != null)
                {
                    ApplyDamage(mono.LinkedEntity, hit.transform.position);
                    hitAny = true;
                }
            }

            if (hitAny)
            {
                _successfulHitEvent?.Invoke();

                ApplyJuggle(dir);
                ExtendInvulnerability();
                _coroutines.StartPerform(DoHitStop());
            }
        }

        private void ApplyDamage(Entity target, Vector2 pos)
        {
            if (target.HasComponent<TakeDamageRequest>())
            {
                var damageData = new DamageData
                {
                    Amount = _attackDamage.Value,
                    SourcePosition = pos
                };
                target.TakeDamageRequest.Invoke(damageData);
            }
        }

        private void ApplyJuggle(float direction)
        {
            float baseForce = _hitBounceForce.Value;

            // Используем реактивные векторы из компонентов
            Vector2 modifiers = _entity.IsGrounded.Value
                ? _groundBounceModifiers.Value
                : _airBounceModifiers.Value;

            float horizontalImpulse = direction * baseForce * modifiers.x;
            float verticalImpulse = baseForce * modifiers.y;

            _entity.Rigidbody.linearVelocity = new Vector2(
                horizontalImpulse,
                Mathf.Max(0, _entity.Rigidbody.linearVelocity.y) + verticalImpulse
            );
        }

        private void ExtendInvulnerability()
        {
            if (_entity.HasComponent<AttackInvulnerabilityTimer>())
            {
                _entity.AttackInvulnerabilityTimer.Value = _entity.AttackInvulnerabilityDuration.Value;
                _entity.IsAttackInvulnerable.Value = true;
            }
        }

        private IEnumerator DoHitStop()
        {
            float originalScale = Time.timeScale;
            Time.timeScale = _hitStopScale.Value;

            // Используем Realtime, чтобы корутина не замерзла вместе с Time.timeScale
            yield return new WaitForSecondsRealtime(_hitStopDuration.Value);

            Time.timeScale = originalScale;
        }

        public void OnDispose() => _attackDelayEndDisposable?.Dispose();
    }
}