using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _attackDelayEndDisposable;
        private readonly LayerMask _enemyMask;
        private readonly float _hitBounceForce;
        private readonly ICoroutinesPerformer _coroutines;

        private ReactiveEvent _successfulHitEvent;

        private const float HitStopDuration = 0.15f;
        private const float HitStopScale = 0.05f;

        public MeleeAttackHitSystem(LayerMask enemyMask, float hitBounceForce, ICoroutinesPerformer coroutines)
        {
            _enemyMask = enemyMask;
            _hitBounceForce = hitBounceForce;
            _coroutines = coroutines;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _successfulHitEvent = _entity.SuccessfulHitEvent;

            _attackDelayEndDisposable = _entity.AttackDelayEndEvent.Subscribe(OnAttackHit);
        }

        private void OnAttackHit()
        {
            float dir = _entity.Transform.localScale.x > 0 ? 1f : -1f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                (Vector2)_entity.Transform.position + Vector2.right * dir * (_entity.AttackRange.Value * 0.5f),
                _entity.AttackRange.Value * 0.5f, _enemyMask);

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
                    Amount = _entity.AttackDamage.Value,
                    SourcePosition = pos
                };
                target.TakeDamageRequest.Invoke(damageData);
            }
        }

        private void ApplyJuggle(float direction)
        {
            float horizontalImpulse = direction * _hitBounceForce * 0.7f;
            float verticalImpulse = _entity.IsGrounded.Value ? _hitBounceForce * 0.4f : _hitBounceForce * 0.8f;
            _entity.Rigidbody.linearVelocity = new Vector2(horizontalImpulse, Mathf.Max(0, _entity.Rigidbody.linearVelocity.y) + verticalImpulse);
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
            Time.timeScale = HitStopScale;
            yield return new WaitForSecondsRealtime(HitStopDuration);
            Time.timeScale = 1f;
        }

        public void OnDispose() => _attackDelayEndDisposable?.Dispose();
    }
}