using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _attackDelayEndDisposable;

        private readonly Collider2D[] _hitBuffer = new Collider2D[15];
        private ContactFilter2D _contactFilter;

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _entity.AttackEnemyMask.Value
            };

            _attackDelayEndDisposable = _entity.AttackDelayEndEvent.Subscribe(OnAttackHit);
        }

        private void OnAttackHit()
        {
            float dir = _entity.Transform.localScale.x > 0 ? 1f : -1f;
            float range = _entity.AttackRange.Value;
            Vector2 origin = (Vector2)_entity.Transform.position + Vector2.right * dir * (range * 0.5f);

            int hitCount = Physics2D.OverlapCircle(
                origin,
                range * 0.5f,
                _contactFilter,
                _hitBuffer);

            if (hitCount == 0) return;

            bool hitAnyTarget = false;

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitBuffer[i];
                if (hit == null) continue;

                var mono = hit.GetComponentInParent<MonoEntity>();

                if (mono != null)
                {
                    ApplyDamage(mono.LinkedEntity, hit.transform.position);
                    hitAnyTarget = true;
                }
            }

            if (hitAnyTarget)
            {
                _entity.SuccessfulHitEvent?.Invoke();
                ApplyJuggle(dir);
                RefreshInvulnerability();
            }
        }

        private void ApplyDamage(Entity target, Vector2 pos)
        {
            if (target.HasComponent<TakeDamageRequest>())
            {
                target.TakeDamageRequest.Invoke(new DamageData
                {
                    Amount = _entity.AttackDamage.Value,
                    SourcePosition = pos
                });
            }
        }

        private void ApplyJuggle(float direction)
        {
            float baseForce = _entity.AttackHitBounceForce.Value;
            Vector2 modifiers = _entity.IsGrounded.Value
                ? _entity.GroundHitBounceModifiers.Value
                : _entity.AirHitBounceModifiers.Value;

            _entity.Rigidbody.linearVelocity = new Vector2(
                direction * baseForce * modifiers.x,
                Mathf.Max(0, _entity.Rigidbody.linearVelocity.y) + (baseForce * modifiers.y)
            );
        }

        private void RefreshInvulnerability()
        {
            if (_entity.HasComponent<IsAttackInvulnerable>())
            {
                _entity.AttackInvulnerabilityTimer.Value = _entity.AttackInvulnerabilityDuration.Value;
                _entity.IsAttackInvulnerable.Value = true;
            }
        }

        public void OnDispose() => _attackDelayEndDisposable?.Dispose();
    }
}