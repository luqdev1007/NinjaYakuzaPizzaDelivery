using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleHookProjectile : ThrowableProjectile
    {
        private readonly GrappleHookConfig _config;
        private readonly Rigidbody2D _heroRigidbody;
        private readonly Transform _heroTransform;
        private readonly LayerMask _enemyMask;
        private Func<bool> _isCancelled;

        private const float MaxDistanceMultiplier = 1.5f;

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
        }

        public void SetCancelCondition(Func<bool> isCancelled) => _isCancelled = isCancelled;

        protected override void OnHit(Collider2D hit)
        {
            Entity targetEntity = null;
            var monoEntity = hit.GetComponentInParent<MonoEntity>();
            if (monoEntity != null) targetEntity = monoEntity.LinkedEntity;

            bool isEnemy = (_enemyMask.value & (1 << hit.gameObject.layer)) != 0;

            if (isEnemy && (targetEntity == null || targetEntity.CanApplyDamage.Evaluate() == false))
            {
                return;
            }

            if (Instance != null)
            {
                Instance.transform.SetParent(hit.transform);
                var projectileRb = Instance.GetComponent<Rigidbody2D>();
                if (projectileRb != null) projectileRb.linearVelocity = Vector2.zero;
            }

            FlipTowards(hit.transform.position);
            CoroutinesPerformer.StartPerform(PullPhysicsCoroutine(hit.ClosestPoint(_heroTransform.position), hit, isEnemy, targetEntity));
        }

        protected override void OnMaxDistanceReached(Vector3 startPosition)
        {
            CoroutinesPerformer.StartPerform(ReturnCoroutine(startPosition));
        }

        private IEnumerator PullPhysicsCoroutine(Vector2 anchor, Collider2D hitCollider, bool isEnemy, Entity targetEntity)
        {
            OnGrappleStarted?.Invoke();

            // Защита от уничтожения игрока на старте
            if (_heroTransform == null || _heroRigidbody == null) yield break;

            float originalGravity = _heroRigidbody.gravityScale;
            float originalDrag = _heroRigidbody.linearDamping;

            if (isEnemy && targetEntity != null)
            {
                if (!targetEntity.HasComponent<IsGrappledTarget>()) targetEntity.AddIsGrappledTarget();
                targetEntity.IsGrappledTarget.Value = true;
            }

            _heroRigidbody.gravityScale = 0.5f;
            _heroRigidbody.linearDamping = 0.5f;
            _heroRigidbody.linearVelocity = Vector2.zero;

            Vector2 toTargetInitial = (anchor - (Vector2)_heroTransform.position).normalized;
            Vector2 popUpForce = (Vector2.up + toTargetInitial * 0.5f).normalized * _config.InitialPopUpForce;
            _heroRigidbody.AddForce(popUpForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.05f);

            while (true)
            {
                if (_isCancelled != null && _isCancelled()) break;

                // ГЛАВНЫЙ ФИКС: Проверка на существование объектов в каждом кадре
                if (_heroTransform == null || _heroRigidbody == null) yield break;
                if (hitCollider == null) break;
                if (isEnemy && (targetEntity == null || targetEntity.IsDead.Value)) break;

                // Теперь безопасно берем позицию
                anchor = hitCollider.transform.position;

                Vector2 playerPos = _heroTransform.position;
                Vector2 toTarget = anchor - playerPos;
                float distance = toTarget.magnitude;

                if (distance > _config.MaxDistance * MaxDistanceMultiplier)
                {
                    _heroRigidbody.AddForce(toTarget.normalized * _config.GrappleSpeed * 0.5f, ForceMode2D.Impulse);
                    break;
                }

                if (distance <= _config.ArriveDistance)
                {
                    if (isEnemy) HandleEnemyCollision(targetEntity, hitCollider);
                    break;
                }

                float pullPower = _config.GrappleSpeed * 2.5f;
                _heroRigidbody.AddForce(toTarget.normalized * pullPower, ForceMode2D.Force);

                if (_heroRigidbody.linearVelocity.magnitude > _config.GrappleSpeed * 1.5f)
                {
                    _heroRigidbody.linearVelocity = _heroRigidbody.linearVelocity.normalized * _config.GrappleSpeed * 1.5f;
                }

                yield return new WaitForFixedUpdate();
            }

            // Очистка
            if (targetEntity != null && targetEntity.HasComponent<IsGrappledTarget>())
            {
                targetEntity.IsGrappledTarget.Value = false;
            }

            if (_heroRigidbody != null)
            {
                _heroRigidbody.linearDamping = originalDrag;
                EndGrapple(originalGravity);
            }
        }

        private void HandleEnemyCollision(Entity targetEntity, Collider2D enemyCollider)
        {
            if (targetEntity != null)
            {
                if (targetEntity.CurrentHealth != null) targetEntity.CurrentHealth.Value = 0;
            }
            else if (enemyCollider != null)
            {
                enemyCollider.gameObject.SetActive(false);
            }

            OnEnemyArrived?.Invoke();
        }

        private void EndGrapple(float originalGravity)
        {
            if (_heroRigidbody == null) return;

            _heroRigidbody.gravityScale = originalGravity;
            Vector2 boost = _heroRigidbody.linearVelocity * _config.CancelInertiaMultiplier;
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

                // Проверка на случай удаления Instance в процессе
                if (Instance == null) break;

                Instance.transform.position = Vector3.MoveTowards(Instance.transform.position, returnTarget, Config.ProjectileSpeed * 2f * Time.deltaTime);
                if (Vector3.Distance(Instance.transform.position, returnTarget) <= 0.1f) break;
                yield return null;
            }
            Destroy();
            OnGrappleEnded?.Invoke();
        }

        private void FlipTowards(Vector3 target)
        {
            if (_heroTransform == null) return;
            Vector3 scale = _heroTransform.localScale;
            scale.x = (target.x > _heroTransform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            _heroTransform.localScale = scale;
        }
    }
}