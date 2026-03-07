using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _attackDelayEndEvent;
        private ReactiveVariable<float> _attackRange;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private IDisposable _attackDelayEndDisposable;

        private readonly LayerMask _enemyMask;
        private readonly float _hitBounceForce;

        public MeleeAttackHitSystem(LayerMask enemyMask, float hitBounceForce = 8f)
        {
            _enemyMask = enemyMask;
            _hitBounceForce = hitBounceForce;
        }

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.AttackDelayEndEvent;
            _attackRange = entity.AttackRange;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _attackDelayEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }

        public void OnDispose()
        {
            _attackDelayEndDisposable.Dispose();
        }

        private void OnAttackDelayEnd()
        {
            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            Vector2 origin = _transform.position;
            Vector2 attackDirection = Vector2.right * direction;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                origin + attackDirection * (_attackRange.Value * 0.5f),
                _attackRange.Value * 0.5f,
                _enemyMask);

            if (hits.Length == 0)
                return;

            foreach (Collider2D hit in hits)
                hit.gameObject.SetActive(false);

            _rigidbody.linearVelocity = new Vector2(
                _rigidbody.linearVelocity.x,
                _hitBounceForce);
        }
    }
}
