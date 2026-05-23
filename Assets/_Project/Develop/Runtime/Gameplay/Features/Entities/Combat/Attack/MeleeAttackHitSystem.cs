using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _attackDelayEndDisposable;

        private ReactiveEvent _successfulHitEvent;
        private ReactiveVariable<float> _attackRange;
        private ReactiveVariable<float> _attackDamage;
        private ReactiveVariable<Vector2> _attackKnockback;
        private ReactiveVariable<LayerMask> _enemyMask;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _successfulHitEvent = _entity.SuccessfulHitEvent;
            _attackRange = _entity.AttackRange;
            _attackDamage = _entity.AttackDamage;
            _enemyMask = _entity.AttackHitMask;
            _attackKnockback = _entity.AttackKnocback;

            _attackDelayEndDisposable = _entity.AttackDelayEndEvent.Subscribe(OnAttackHit);
        }

        private void OnAttackHit()
        {
            float direction = Mathf.Abs(_entity.Transform.localRotation.eulerAngles.y - 180f) < 1f ? -1f : 1f;
            float range = _attackRange.Value;

            Vector2 origin = (Vector2)_entity.Transform.position + Vector2.right * direction * (range * 0.5f);
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range * 0.5f, _enemyMask.Value);

            bool hitAny = false;

            foreach (var hit in hits)
            {
                var monoEntity = hit.GetComponentInParent<MonoEntity>();

                if (monoEntity != null)
                {
                    ApplyDamage(monoEntity.LinkedEntity, hit.transform.position);
                    hitAny = true;
                }
            }

            if (hitAny)
            {
                _successfulHitEvent?.Invoke();
            }
        }

        private void ApplyDamage(Entity target, Vector2 position)
        {
            if (target.HasComponent<TakeDamageRequest>())
            {
                float direction = Mathf.Abs(_entity.Transform.localRotation.eulerAngles.y - 180f) < 1f ? -1f : 1f;
                Vector2 appliedKnockback = new Vector2(_attackKnockback.Value.x * direction, _attackKnockback.Value.y);

                var damageData = new DamageData
                {
                    Amount = _attackDamage.Value,
                    SourcePosition = position,
                    KnockbackForce = appliedKnockback 
                };

                target.TakeDamageRequest.Invoke(damageData);
            }
        }

        public void OnDispose()
        {
            _attackDelayEndDisposable?.Dispose();
        }
    }
}