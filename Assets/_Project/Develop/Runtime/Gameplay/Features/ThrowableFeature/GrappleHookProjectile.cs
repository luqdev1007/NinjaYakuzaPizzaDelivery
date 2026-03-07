using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
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

        private float _defaultGravityScale;
        private Func<bool> _isCancelled;

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
                    EndGrapple(applyBounce: false);
                    yield break;
                }

                Vector3 toAnchor = anchor - _heroTransform.position;

                if (toAnchor.magnitude <= _config.ArriveDistance)
                {
                    EndGrapple(applyBounce: true);
                    yield break;
                }

                _heroRigidbody.linearVelocity = toAnchor.normalized * _config.GrappleSpeed;
                yield return null;
            }
        }

        private IEnumerator PullToEnemyCoroutine(Collider2D enemy)
        {
            OnGrappleStarted?.Invoke();
            _heroRigidbody.gravityScale = 0f;
            _heroRigidbody.linearVelocity = Vector2.zero;

            while (true)
            {
                if (_isCancelled != null && _isCancelled())
                {
                    EndGrapple(applyBounce: false);
                    yield break;
                }

                if (enemy == null || !enemy.gameObject.activeSelf)
                {
                    EndGrapple(applyBounce: false);
                    yield break;
                }

                Vector3 toEnemy = enemy.transform.position - _heroTransform.position;

                if (toEnemy.magnitude <= _config.ArriveDistance)
                {
                    EndGrapple(applyBounce: true);
                    OnEnemyArrived?.Invoke();
                    yield break;
                }

                _heroRigidbody.linearVelocity = toEnemy.normalized * _config.GrappleSpeed;
                yield return null;
            }
        }

        private void FlipTowards(Vector3 target)
        {
            Vector3 scale = _heroTransform.localScale;
            float dirX = target.x - _heroTransform.position.x;
            scale.x = dirX > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _heroTransform.localScale = scale;
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

        private void EndGrapple(bool applyBounce)
        {
            _heroRigidbody.gravityScale = _defaultGravityScale;

            if (applyBounce)
                _heroRigidbody.linearVelocity = new Vector2(
                    _heroRigidbody.linearVelocity.x,
                    _config.ArrivalBounce);
            else
                _heroRigidbody.linearVelocity = Vector2.zero;

            OnGrappleEnded?.Invoke();
            Destroy();
        }
    }
}