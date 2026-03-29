using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _attackDelayEndDisposable;
        private readonly LayerMask _enemyMask;
        private readonly float _hitBounceForce;
        private readonly ICoroutinesPerformer _coroutines;

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
                ApplyJuggle();
                ExtendInvulnerability();
                _coroutines.StartPerform(DoHitStop());
            }
        }

        private void ApplyDamage(Entity target, Vector2 pos)
        {
            if (target.HasComponent<CurrentHealth>())
            {
                target.CurrentHealth.Value -= _entity.AttackDamage.Value;
                target.TakeDamageEvent?.Invoke(new DamageData { Amount = _entity.AttackDamage.Value, SourcePosition = pos });
            }
        }

        private void ApplyJuggle()
        {
            float jump = _entity.IsGrounded.Value ? _hitBounceForce * 0.5f : _hitBounceForce;
            _entity.Rigidbody.linearVelocity = new Vector2(_entity.Rigidbody.linearVelocity.x, Mathf.Max(0, _entity.Rigidbody.linearVelocity.y) + jump);
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