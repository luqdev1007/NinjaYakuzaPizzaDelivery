using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GrappleHookConfig _config;
        private readonly IThrowableBehaviourFactory _behaviourFactory;
        private readonly CollidersRegistryService _collidersRegistry;

        private ICompositeCondition _canThrow;
        private ReactiveVariable<bool> _isThrowing;
        private ReactiveVariable<bool> _isWallHanging;
        private ReactiveVariable<int> _charges;
        private ReactiveEvent _startAttackRequest;

        public readonly ReactiveEvent OnLaunched = new();
        public readonly ReactiveEvent OnImpact = new();
        public readonly ReactiveEvent OnBreak = new();

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private GrappleRopeView _ropeView;

        private ThrowableProjectile _activeProjectile;
        private Coroutine _pullCoroutine;
        private bool _isPulling;
        private Vector2 _lastPullDirection;
        private InputState _grappleInput;

        public GrappleSystem(
            ICoroutinesPerformer performer,
            GrappleHookConfig config,
            IThrowableBehaviourFactory factory,
            CollidersRegistryService collidersRegistry)
        {
            _coroutinesPerformer = performer;
            _config = config;
            _behaviourFactory = factory;
            _collidersRegistry = collidersRegistry;
        }

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _canThrow = entity.CanGrapple;
            _isThrowing = entity.IsThrowing;
            _isWallHanging = entity.IsWallHanging;
            _charges = entity.GrappleCharges;
            _startAttackRequest = entity.StartAttackRequest;
            _ropeView = entity.Transform.GetComponentInChildren<GrappleRopeView>();
            _grappleInput = entity.GrappleInput;

            _isWallHanging.Subscribe((_, isHanging) =>
            {
                if (isHanging && (_isThrowing.Value || _isPulling))
                    StopPulling(false);
            });
        }

        public void OnUpdate(float deltaTime)
        {
            if (_grappleInput.IsPressed.Value && _canThrow.Evaluate() && !_isPulling)
                TryLaunch();

            if (_grappleInput.IsReleased.Value && _isPulling)
                StopPulling(true);
        }

        private void TryLaunch()
        {
            if (_charges.Value <= 0) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = ((Vector2)mousePos - (Vector2)_transform.position).normalized;

            _charges.Value--;
            _isThrowing.Value = true;
            OnLaunched.Invoke();

            _activeProjectile = _behaviourFactory.Create(_config, _rigidbody, _transform);

            if (_activeProjectile is GrappleHookProjectile grapple)
            {
                grapple.OnAnchored += HandleAnchored;
                grapple.Launch(_transform.position, dir);
                _ropeView?.SetHookTransform(grapple.Instance.transform);
            }
        }

        private void HandleAnchored(Vector2 pos, Collider2D hit)
        {
            OnImpact.Invoke();
            _isPulling = true;

            Entity targetEntity = _collidersRegistry.GetBy(hit);

            if (targetEntity != null)
                _pullCoroutine = _coroutinesPerformer.StartPerform(PullMutualRoutine(targetEntity));
            else
                _pullCoroutine = _coroutinesPerformer.StartPerform(PullStaticRoutine(hit.transform.InverseTransformPoint(pos), hit));
        }

        private IEnumerator PullStaticRoutine(Vector2 localOffset, Collider2D hit)
        {
            float defaultGravity = _rigidbody.gravityScale;
            _rigidbody.gravityScale = 0.2f;

            while (hit != null && _grappleInput.IsHeld.Value)
            {
                Vector2 anchor = hit.transform.TransformPoint(localOffset);
                Vector2 toTarget = anchor - (Vector2)_transform.position;
                _lastPullDirection = toTarget.normalized;
                _ropeView?.FixateToPoint(anchor);

                if (toTarget.magnitude > _config.ArriveDistance)
                    _rigidbody.AddForce(_lastPullDirection * _config.GrappleSpeed * Mathf.Clamp(toTarget.magnitude / 2f, 1f, 2.5f), ForceMode2D.Force);

                yield return new WaitForFixedUpdate();
            }

            _rigidbody.gravityScale = defaultGravity;
            StopPulling(true);
        }

        private IEnumerator PullMutualRoutine(Entity target)
        {
            var targetRb = target.Rigidbody;
            var playerGravity = _rigidbody.gravityScale;
            var targetGravity = targetRb != null ? targetRb.gravityScale : 0;

            var movementLock = new ManualCondition(false);
            target.CanMove?.Add(movementLock);

            _rigidbody.gravityScale = 0.2f;
            if (targetRb != null) targetRb.gravityScale = 0.2f;

            while (target != null && target.Transform != null && _grappleInput.IsHeld.Value)
            {
                Vector2 toTarget = (Vector2)target.Transform.position - (Vector2)_transform.position;
                _lastPullDirection = toTarget.normalized;
                _ropeView?.FixateToPoint(target.Transform.position);

                if (toTarget.magnitude <= _config.ArriveDistance)
                {
                    _startAttackRequest.Invoke();
                }
                else
                {
                    float force = _config.GrappleSpeed;
                    _rigidbody.AddForce(_lastPullDirection * force, ForceMode2D.Force);
                    targetRb?.AddForce(-_lastPullDirection * force, ForceMode2D.Force);
                }

                yield return new WaitForFixedUpdate();
            }

            target.CanMove?.Remove(movementLock);
            _rigidbody.gravityScale = playerGravity;
            if (targetRb != null) targetRb.gravityScale = targetGravity;

            StopPulling(true);
        }

        private void StopPulling(bool applyInertia)
        {
            if (applyInertia && _isPulling)
            {
                Vector2 launchDir = (_lastPullDirection + Vector2.up * 0.4f).normalized;
                _rigidbody.linearVelocity = launchDir * Mathf.Max(_rigidbody.linearVelocity.magnitude, 15f);
            }

            if (_pullCoroutine != null)
                _coroutinesPerformer.StopPerform(_pullCoroutine);

            _isPulling = false;
            _isThrowing.Value = false;
            _activeProjectile?.Cancel();
            _activeProjectile = null;
            _ropeView?.ClearHookTransform();

            if (!applyInertia)
                OnBreak.Invoke();
        }
    }
}