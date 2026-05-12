using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;

        private ReactiveEvent _successfulHitEvent;

        private ReactiveVariable<float> _attackRange;
        private ReactiveVariable<float> _attackDamage;

        private ReactiveVariable<LayerMask> _enemyMask;

        private IDisposable _attackDelayEndDisposable;


        public void OnInit(Entity entity)
        {
            _entity = entity;

            /*
            _successfulHitEvent = _entity.SuccessfulHitEvent;
            _attackRange = _entity.AttackRange;
            _attackDamage = _entity.AttackDamage;
            _enemyMask = _entity.AttackEnemyMask;

            _attackDelayEndDisposable = _entity.AttackDelayEndEvent.Subscribe(OnAttackHit);
            */
        }

        private void OnAttackHit()
        {
            /*
            float dir = _entity.Transform.localScale.x > 0 ? 1f : -1f;
            float range = _attackRange.Value;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                (Vector2)_entity.Transform.position + Vector2.right * dir * (range * 0.5f),
                range * 0.5f,
                _enemyMask.Value);

            if (hits.Length == 0) 
                return;

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
            }
            */
        }

        private void ApplyDamage(Entity target, Vector2 pos)
        {
            if (target.HasComponent<TakeDamageRequest>())
            {
                var damageData = new DamageData
                {
                    Amount = _attackDamage.Value,
                    SourcePosition = pos,
                    Type = DamageType.General
                };

                // target.TakeDamageRequest.Invoke(damageData);
            }
        }

        public void OnDispose() => _attackDelayEndDisposable?.Dispose();
    }
}