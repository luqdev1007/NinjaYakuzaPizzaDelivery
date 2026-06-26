using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly GrappleHookConfig _grappleHookConfig;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canGrapple;
        private ReactiveVariable<bool> _intentGrapple;
        private ReactiveVariable<bool> _isGrappling;
        private ReactiveVariable<Transform> _grappleHookTransform;
        private ReactiveVariable<Vector3> _grappleHookAnchor;
        private ReactiveEvent _grappleAnchoredEvent;

        private ReactiveVariable<Vector2> _intentAimDirection;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private ThrowableProjectile _activeProjectile;
        private Coroutine _pullCoroutine;

        private float _defaultGravity;
        private bool _isPulling;
        private bool _wasGrappleIntendedLastFrame;
        private Vector2 _lastPullDirection;

        private const float PullGravity = 0.2f;

        public GrappleSystem(GrappleHookConfig grappleHookConfig, ICoroutinesPerformer performer)
        {
            _grappleHookConfig = grappleHookConfig;
            _coroutinesPerformer = performer;
        }

        public void OnInit(Entity entity)
        {
            _canGrapple = entity.CanGrapple;
            _intentGrapple = entity.IntentGrapple;
            _isGrappling = entity.IsGrappling;
            _grappleHookTransform = entity.GrappleHookTransform;
            _grappleHookAnchor = entity.GrappleAnchorPoint;
            _intentAimDirection = entity.IntentAimDirection;
            _grappleAnchoredEvent = entity.GrappleAnchoredEvent;

            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;

            _defaultGravity = entity.BaseGravityScale.Value;
        }

        public void OnUpdate(float deltaTime)
        {
            bool currentIntent = _intentGrapple.Value;
            bool isPressedDown = currentIntent && !_wasGrappleIntendedLastFrame;
            bool isReleased = !currentIntent && _wasGrappleIntendedLastFrame;

            _wasGrappleIntendedLastFrame = currentIntent;

            bool isProjectileActive = _activeProjectile is GrappleHookProjectile;

            if (isPressedDown && _canGrapple.Evaluate() && !isProjectileActive && !_isPulling)
            {
                TryLaunch();
            }

            if (isReleased && (isProjectileActive || _isPulling))
            {
                StopPulling(applyInertia: true);
            }
        }

        private void TryLaunch()
        {
            Vector3 dir = new Vector3(_intentAimDirection.Value.x, _intentAimDirection.Value.y, 0f);

            _isGrappling.Value = true;

            _activeProjectile = new GrappleHookProjectile(_grappleHookConfig, _coroutinesPerformer);

            if (_activeProjectile is GrappleHookProjectile grapple)
            {
                grapple.OnAnchored += (pos, hit) =>
                {
                    StartPulling(pos, hit);
                };

                _activeProjectile.Launch(_transform.position, dir);

                if (grapple.Instance != null)
                    _grappleHookTransform.Value = grapple.Instance.transform;

                _activeProjectile.OnCompleted += () =>
                {
                    if (!_isPulling) _isGrappling.Value = false;
                };
            }
            else
            {
                _isGrappling.Value = false;
            }
        }

        private void StartPulling(Vector2 anchorPos, Collider2D hit)
        {
            _grappleAnchoredEvent?.Invoke();
            _isPulling = true;
            _grappleHookTransform.Value = null;

            Vector2 localOffset = (Vector2)hit.transform.InverseTransformPoint(anchorPos);
            _pullCoroutine = _coroutinesPerformer.StartPerform(PullRoutine(localOffset, hit));
        }

        private IEnumerator PullRoutine(Vector2 localOffset, Collider2D hit)
        {
            yield return null;

            _rigidbody.gravityScale = PullGravity;

            while (hit != null && hit.transform != null && _intentGrapple.Value)
            {
                Vector2 currentAnchorWorld = (Vector2)hit.transform.TransformPoint(localOffset);
                Vector2 toTarget = currentAnchorWorld - (Vector2)_transform.position;
                float dist = toTarget.magnitude;

                if (dist > _grappleHookConfig.MaxFlyDistance * 1.5f)
                {
                    StopPulling(applyInertia: false);
                    yield break;
                }

                _lastPullDirection = toTarget.normalized;
                _grappleHookAnchor.Value = currentAnchorWorld;

                if (dist <= _grappleHookConfig.ArriveDistance)
                {
                    _rigidbody.linearVelocity *= 0.98f;
                }
                else
                {
                    float forceMultiplier = Mathf.Clamp(dist / 2f, 1f, 2.5f);
                    _rigidbody.AddForce(_lastPullDirection * _grappleHookConfig.GrappleSpeed * forceMultiplier, ForceMode2D.Force);
                }

                yield return new WaitForFixedUpdate();
            }

            StopPulling(applyInertia: true);
        }

        private void StopPulling(bool applyInertia)
        {
            if (!_isPulling && _activeProjectile == null)
                return;

            _rigidbody.gravityScale = _defaultGravity;

            if (applyInertia && _isPulling)
            {
                Vector2 launchDirection = (_lastPullDirection + Vector2.up * 0.4f).normalized;
                float currentSpeed = _rigidbody.linearVelocity.magnitude;
                float finalBoost = Mathf.Max(currentSpeed, 15f);
                _rigidbody.linearVelocity = launchDirection * finalBoost;
            }

            if (_pullCoroutine != null)
            {
                _coroutinesPerformer.StopPerform(_pullCoroutine);
                _pullCoroutine = null;
            }

            _isPulling = false;
            CancelAll();
        }

        private void CancelAll()
        {
            if (_activeProjectile != null)
            {
                _activeProjectile.Cancel();
                _activeProjectile = null;
            }

            _isGrappling.Value = false;

            _grappleHookTransform.Value = null;
            _grappleHookAnchor.Value = Vector2.zero;
        }
    }
}