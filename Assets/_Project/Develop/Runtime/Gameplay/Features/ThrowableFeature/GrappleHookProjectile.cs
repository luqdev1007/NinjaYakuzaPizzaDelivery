using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleHookProjectile : ThrowableProjectile
    {
        private readonly GrappleHookConfig _config;
        private readonly Rigidbody2D _heroRigidbody;
        private readonly Transform _heroTransform;
        private readonly LayerMask _enemyMask;

        private Func<bool> _isCancelled;

        public event Action OnGrappleStarted;
        public event Action OnGrappleEnded;
        public event Action OnEnemyArrived; // Возвращаем событие

        public GrappleHookProjectile(
            GrappleHookConfig config,
            ICoroutinesPerformer coroutinesPerformer,
            Rigidbody2D heroRigidbody,
            Transform heroTransform) : base(config, coroutinesPerformer)
        {
            _config = config;
            _heroRigidbody = heroRigidbody;
            _heroTransform = heroTransform;
            _enemyMask = config.EnemyMask;
        }

        public void SetCancelCondition(Func<bool> isCancelled) => _isCancelled = isCancelled;

        protected override void OnHit(Collider2D hit)
        {
            bool hitEnemy = (_enemyMask.value & (1 << hit.gameObject.layer)) != 0;
            FlipTowards(hit.transform.position);

            // Запускаем физическое притягивание
            CoroutinesPerformer.StartPerform(PullPhysicsCoroutine(hit.ClosestPoint(_heroTransform.position), hit, hitEnemy));
        }

        protected override void OnMaxDistanceReached(Vector3 startPosition)
        {
            CoroutinesPerformer.StartPerform(ReturnCoroutine(startPosition));
        }

        private IEnumerator PullPhysicsCoroutine(Vector2 anchor, Collider2D hitCollider, bool isEnemy)
        {
            OnGrappleStarted?.Invoke();

            float originalGravity = _heroRigidbody.gravityScale;

            // 1. ПОЛНЫЙ СБРОС СКОРОСТИ И ГРАВИТАЦИИ
            _heroRigidbody.gravityScale = 0f;
            _heroRigidbody.linearVelocity = Vector2.zero;

            // 2. СТАРТОВЫЙ ИМПУЛЬС (ОТРЫВ ОТ ЗЕМЛИ)
            // Подбрасываем персонажа чуть вверх, чтобы убрать трение о пол
            Vector2 popUpDirection = Vector2.up;

            // Если точка зацепа сильно выше нас, можно чуть сместить импульс в сторону цели
            Vector2 toTargetInitial = anchor - (Vector2)_heroTransform.position;
            if (toTargetInitial.y > 0.5f)
            {
                popUpDirection = (Vector2.up + toTargetInitial.normalized * 0.5f).normalized;
            }

            _heroRigidbody.AddForce(popUpDirection * _config.InitialPopUpForce, ForceMode2D.Impulse);

            // Даем физике один кадр "продышаться" перед тем, как лочить скорость
            yield return null;

            while (true)
            {
                if (_isCancelled != null && _isCancelled()) break;

                if (isEnemy && hitCollider != null)
                    anchor = hitCollider.transform.position;
                else if (isEnemy && hitCollider == null)
                    break;

                Vector2 playerPos = _heroTransform.position;
                Vector2 toTarget = anchor - playerPos;
                float distance = toTarget.magnitude;

                if (distance <= _config.ArriveDistance)
                {
                    if (isEnemy) HandleEnemyCollision(hitCollider);
                    break;
                }

                // 3. ПОЛЕТ ПО ПРЯМОЙ
                // Перезаписываем скорость каждый кадр, игнорируя внешние силы
                _heroRigidbody.linearVelocity = toTarget.normalized * _config.GrappleSpeed;

                yield return null;
            }

            EndGrapple(originalGravity);
        }

        private void HandleEnemyCollision(Collider2D enemyCollider)
        {
            if (enemyCollider == null) return;

            // Полная логика уничтожения как была
            var monoEntity = enemyCollider.GetComponentInParent<MonoEntity>();
            if (monoEntity != null && monoEntity.LinkedEntity != null)
            {
                var enemyEntity = monoEntity.LinkedEntity;
                if (enemyEntity.CurrentHealth != null)
                {
                    enemyEntity.CurrentHealth.Value = 0;
                    Debug.Log("Enemy Entity killed by physical grapple!");
                }
            }
            else
            {
                enemyCollider.gameObject.SetActive(false);
                Debug.Log("Enemy GameObject deactivated by physical grapple!");
            }

            OnEnemyArrived?.Invoke();
        }

        private void EndGrapple(float originalGravity)
        {
            _heroRigidbody.gravityScale = originalGravity;

            // Сохраняем вектор прилета, но даем чуть больше свободы по вертикали в конце
            Vector2 boost = _heroRigidbody.linearVelocity * _config.CancelInertiaMultiplier;

            // Если мы летели вверх, добавим еще немного "бонуса" к прыжку в конце
            if (boost.y > 0) boost.y *= 1.2f;

            _heroRigidbody.linearVelocity = boost;

            OnGrappleEnded?.Invoke();
            Destroy();
        }

        private IEnumerator ReturnCoroutine(Vector3 returnTarget)
        {
            while (Instance != null)
            {
                if (_isCancelled != null && _isCancelled()) break;

                Instance.transform.position = Vector3.MoveTowards(
                    Instance.transform.position,
                    returnTarget,
                    Config.ProjectileSpeed * 2f * Time.deltaTime);

                if (Vector3.Distance(Instance.transform.position, returnTarget) <= 0.1f) break;
                yield return null;
            }
            Destroy();
            OnGrappleEnded?.Invoke();
        }

        private void FlipTowards(Vector3 target)
        {
            Vector3 scale = _heroTransform.localScale;
            scale.x = (target.x > _heroTransform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _heroTransform.localScale = scale;
        }
    }
}