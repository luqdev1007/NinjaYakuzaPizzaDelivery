using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment; // Добавили для корутин
using System;
using System.Collections;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class MeleeAttackHitSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _attackDelayEndEvent;
        private ReactiveVariable<float> _attackRange;
        private ReactiveVariable<float> _attackDamage;
        private ReactiveVariable<bool> _isGrounded;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private IDisposable _attackDelayEndDisposable;

        private readonly LayerMask _enemyMask;
        private readonly float _hitBounceForce;
        private readonly ICoroutinesPerformer _coroutines; // Сервис для остановки времени

        private const float HitStopDuration = 0.08f; // Оптимально для быстрого экшена
        private const float HitStopScale = 0.05f;   // Почти полная остановка

        public MeleeAttackHitSystem(LayerMask enemyMask, float hitBounceForce, ICoroutinesPerformer coroutines)
        {
            _enemyMask = enemyMask;
            _hitBounceForce = hitBounceForce;
            _coroutines = coroutines;
        }

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.AttackDelayEndEvent;
            _attackRange = entity.AttackRange;
            _attackDamage = entity.AttackDamage;
            _isGrounded = entity.IsGrounded;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _attackDelayEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }

        public void OnDispose() => _attackDelayEndDisposable?.Dispose();

        private void OnAttackDelayEnd()
        {
            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            Vector2 origin = _transform.position;
            Vector2 attackDirection = Vector2.right * direction;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                origin + attackDirection * (_attackRange.Value * 0.5f),
                _attackRange.Value * 0.5f,
                _enemyMask);

            if (hits.Length == 0) return;

            bool hitAnyEnemy = false;

            foreach (Collider2D hit in hits)
            {
                var monoEntity = hit.GetComponentInParent<MonoEntity>();
                if (monoEntity != null)
                {
                    ApplyDamageToTarget(monoEntity.LinkedEntity, hit.transform.position);
                    hitAnyEnemy = true;
                }
            }

            if (hitAnyEnemy)
            {
                ApplyDmcJuggle();
                _coroutines.StartPerform(DoHitStop()); // Запускаем "стоп-кадр"
            }
        }

        private IEnumerator DoHitStop()
        {
            Time.timeScale = HitStopScale;
            // Используем WaitForSecondsRealtime, потому что обычный таймер замедлен
            yield return new WaitForSecondsRealtime(HitStopDuration);
            Time.timeScale = 1f;
        }

        private void ApplyDamageToTarget(Entity target, Vector2 hitPoint)
        {
            if (target.HasComponent<CurrentHealth>())
            {
                float damage = _attackDamage.Value;
                target.CurrentHealth.Value -= damage;
                target.TakeDamageEvent?.Invoke(new DamageData { Amount = damage, SourcePosition = hitPoint });
            }
        }

        private void ApplyDmcJuggle()
        {
            if (!_isGrounded.Value)
            {
                float newYVelocity = Mathf.Max(_rigidbody.linearVelocity.y, 0) + _hitBounceForce;
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, newYVelocity);
            }
            else
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _hitBounceForce * 0.4f);
            }
        }
    }
}