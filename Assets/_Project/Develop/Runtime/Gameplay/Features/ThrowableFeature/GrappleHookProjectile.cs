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
        private readonly float _defaultGravityScale;

        public event Action OnGrappleStarted;
        public event Action OnGrappleEnded;
        public event Action OnEnemyArrived;

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
            _defaultGravityScale = heroRigidbody.gravityScale;
        }

        public void SetCancelCondition(Func<bool> isCancelled)
        {
            _isCancelled = isCancelled;
        }

        protected override void OnHit(Collider2D hit)
        {
            bool hitEnemy = (_enemyMask.value & (1 << hit.gameObject.layer)) != 0;
            FlipTowards(hit.transform.position);

            if (hitEnemy)
                CoroutinesPerformer.StartPerform(PullToEnemyCoroutine(hit));
            else
                CoroutinesPerformer.StartPerform(PullCoroutine(hit.ClosestPoint(_heroTransform.position)));
        }

        protected override void OnMaxDistanceReached(Vector3 startPosition)
        {
            CoroutinesPerformer.StartPerform(ReturnCoroutine(startPosition));
        }

        private IEnumerator PullCoroutine(Vector3 anchor)
        {
            OnGrappleStarted?.Invoke();
            _heroRigidbody.gravityScale = 0f;
            _heroRigidbody.linearVelocity = Vector2.zero;

            while (true)
            {
                if (_isCancelled != null && _isCancelled())
                {
                    EndGrapple(preserveVelocity: true);
                    yield break;
                }

                Vector3 toAnchor = anchor - _heroTransform.position;
                if (toAnchor.magnitude <= _config.ArriveDistance)
                {
                    EndGrapple(preserveVelocity: false, applyBounce: true);
                    yield break;
                }

                _heroRigidbody.linearVelocity = toAnchor.normalized * _config.GrappleSpeed;
                yield return null;
            }
        }

        private IEnumerator PullToEnemyCoroutine(Collider2D enemyCollider)
        {
            OnGrappleStarted?.Invoke();
            _heroRigidbody.gravityScale = 0f;
            _heroRigidbody.linearVelocity = Vector2.zero;

            while (true)
            {
                if (_isCancelled != null && _isCancelled())
                {
                    EndGrapple(preserveVelocity: true);
                    yield break;
                }

                if (enemyCollider == null || !enemyCollider.gameObject.activeSelf)
                {
                    EndGrapple(preserveVelocity: true);
                    yield break;
                }

                Vector3 toEnemy = enemyCollider.transform.position - _heroTransform.position;

                if (toEnemy.magnitude <= _config.ArriveDistance)
                {
                    // --- ЗОНА УНИЧТОЖЕНИЯ ВРАГА ---

                    // 1. Попытка через Entity (на будущее)
                    var monoEntity = enemyCollider.GetComponentInParent<MonoEntity>();
                    if (monoEntity != null && monoEntity.LinkedEntity != null)
                    {
                        var enemyEntity = monoEntity.LinkedEntity;
                        if (enemyEntity.CurrentHealth != null)
                        {
                            enemyEntity.CurrentHealth.Value = 0;
                            Debug.Log("Enemy Entity killed!");
                        }
                    }
                    else
                    {
                        // 2. Обычное уничтожение GameObject (пока враги не Entity)
                        // Можно использовать Destroy(enemyCollider.gameObject), 
                        // но SetActive(false) безопаснее, если у тебя есть пулинг.
                        enemyCollider.gameObject.SetActive(false);
                        Debug.Log("Enemy GameObject deactivated!");
                    }

                    // ------------------------------

                    EndGrapple(preserveVelocity: false, applyBounce: true);
                    OnEnemyArrived?.Invoke();
                    yield break;
                }

                _heroRigidbody.linearVelocity = toEnemy.normalized * _config.GrappleSpeed;
                yield return null;
            }
        }

        private IEnumerator ReturnCoroutine(Vector3 returnTarget)
        {
            while (Instance != null)
            {
                if (_isCancelled != null && _isCancelled())
                {
                    Destroy();
                    OnGrappleEnded?.Invoke();
                    yield break;
                }

                Instance.transform.position = Vector3.MoveTowards(
                    Instance.transform.position,
                    returnTarget,
                    Config.ProjectileSpeed * 2f * Time.deltaTime);

                if (Vector3.Distance(Instance.transform.position, returnTarget) <= 0.1f)
                {
                    Destroy();
                    OnGrappleEnded?.Invoke();
                    yield break;
                }

                yield return null;
            }
        }

        private void EndGrapple(bool preserveVelocity, bool applyBounce = false)
        {
            Vector2 savedVelocity = _heroRigidbody.linearVelocity;
            _heroRigidbody.gravityScale = _defaultGravityScale;

            if (preserveVelocity || applyBounce)
            {
                _heroRigidbody.linearVelocity = savedVelocity * _config.CancelInertiaMultiplier;
            }
            else
            {
                _heroRigidbody.linearVelocity = Vector2.zero;
            }

            OnGrappleEnded?.Invoke();
            Destroy();
        }

        private void FlipTowards(Vector3 target)
        {
            Vector3 scale = _heroTransform.localScale;
            float dirX = target.x - _heroTransform.position.x;
            scale.x = dirX > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _heroTransform.localScale = scale;
        }
    }
}