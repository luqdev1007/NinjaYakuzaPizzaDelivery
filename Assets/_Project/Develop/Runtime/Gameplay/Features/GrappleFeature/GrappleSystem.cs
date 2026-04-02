using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class GrappleSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GrappleHookConfig _config;
        private readonly IThrowableBehaviourFactory _behaviourFactory;
        private readonly AudioService _audioService;

        private ICompositeCondition _canThrow;
        private ReactiveVariable<bool> _isThrowing;
        private ReactiveVariable<bool> _isWallHanging; // Ссылка на стену
        private ReactiveVariable<int> _charges;
        private ReactiveEvent _startAttackRequest;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private GrappleRopeView _ropeView;

        private ThrowableProjectile _activeProjectile;
        private Coroutine _pullCoroutine;
        private string _activeLoopId;

        private float _defaultGravity;
        private bool _isPulling;
        private Vector2 _lastPullDirection;

        public GrappleSystem(
            IInputService input,
            ICoroutinesPerformer performer,
            GrappleHookConfig config,
            IThrowableBehaviourFactory factory,
            AudioService audioService)
        {
            _inputService = input;
            _coroutinesPerformer = performer;
            _config = config;
            _behaviourFactory = factory;
            _audioService = audioService;
        }

        public void OnInit(Entity entity)
        {
            _canThrow = entity.CanGrapple;
            _isThrowing = entity.IsThrowing;
            _isWallHanging = entity.IsWallHanging;
            _charges = entity.GrappleCharges;
            _startAttackRequest = entity.StartAttackRequest;
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;
            _ropeView = entity.Transform.GetComponentInChildren<GrappleRopeView>();

            _defaultGravity = _rigidbody.gravityScale;

            // Если начали висеть на стене, а крюк в процессе — обрываем его
            _isWallHanging.Subscribe((_, isHanging) =>
            {
                if (isHanging && (_isThrowing.Value || _isPulling || _activeProjectile != null))
                {
                    _audioService.PlaySfxByPrefixAuto("HookBreak", 1f);
                    StopPulling(applyInertia: false);
                }
            });
        }

        public void OnUpdate(float deltaTime)
        {
            bool isGrappleActive = _activeProjectile is GrappleHookProjectile;

            if (_inputService.IsGrappleKeyPressed && _canThrow.Evaluate() && !isGrappleActive && !_isPulling)
                TryLaunch();

            if (_inputService.IsGrappleKeyReleased)
            {
                if (isGrappleActive || _isPulling)
                    StopPulling(applyInertia: true);
            }
        }

        private void TryLaunch()
        {
            if (_charges.Value <= 0 || Camera.main == null) return;




            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = (mousePos - _transform.position).normalized;
            dir.z = 0;

            _charges.Value--;
            _isThrowing.Value = true; // WallHangSystem это увидит и отпустит стену

            _audioService.PlaySfxByPrefixAuto("HookShot", 1f);
            _activeLoopId = _audioService.PlaySfxVariationLoop("HookLoop", 1, 1);

            _activeProjectile = _behaviourFactory.Create(_config, _rigidbody, _transform);

            if (_activeProjectile is GrappleHookProjectile grapple)
            {
                grapple.OnAnchored += (pos, hit) =>
                {
                    StopLoopSfx();
                    StartPulling(pos, hit);
                };

                _activeProjectile.Launch(_transform.position, dir);

                if (_ropeView != null && grapple.Instance != null)
                    _ropeView.SetHookTransform(grapple.Instance.transform);

                _activeProjectile.OnCompleted += () =>
                {
                    StopLoopSfx();
                    if (!_isPulling) _isThrowing.Value = false;
                };
            }
            else
            {
                StopLoopSfx();
                _isThrowing.Value = false;
            }
        }

        private IEnumerator PullRoutine(Vector2 localOffset, Collider2D hit)
        {
            _rigidbody.gravityScale = 0.2f;

            while (hit != null && hit.transform != null && _inputService.IsGrappleKeyHeld)
            {
                Vector2 currentAnchorWorld = (Vector2)hit.transform.TransformPoint(localOffset);
                Vector2 toTarget = currentAnchorWorld - (Vector2)_transform.position;
                float dist = toTarget.magnitude;

                if (dist > _config.MaxDistance * 1.5f)
                {
                    _audioService.PlaySfxByPrefixAuto("HookBreak", 1f);
                    StopPulling(applyInertia: false);
                    yield break;
                }

                _lastPullDirection = toTarget.normalized;
                _ropeView?.FixateToPoint(currentAnchorWorld);

                if (dist <= _config.ArriveDistance)
                {
                    if (hit.gameObject.activeInHierarchy && hit.CompareTag("Enemy"))
                        _startAttackRequest.Invoke();

                    _rigidbody.linearVelocity *= 0.98f;
                }
                else
                {
                    float forceMultiplier = Mathf.Clamp(dist / 2f, 1f, 2.5f);
                    _rigidbody.AddForce(_lastPullDirection * _config.GrappleSpeed * forceMultiplier, ForceMode2D.Force);
                }

                yield return new WaitForFixedUpdate();
            }

            StopPulling(applyInertia: true);
        }

        private void StartPulling(Vector2 anchorPos, Collider2D hit)
        {
            _isPulling = true;
            Vector2 localOffset = (Vector2)hit.transform.InverseTransformPoint(anchorPos);
            _pullCoroutine = _coroutinesPerformer.StartPerform(PullRoutine(localOffset, hit));
        }

        private void StopPulling(bool applyInertia)
        {
            if (!_isPulling && _activeProjectile == null) return;

            StopLoopSfx();

            // Важно: возвращаем гравитацию, ТОЛЬКО если не висим на стене
            if (!_isWallHanging.Value)
            {
                _rigidbody.gravityScale = _defaultGravity;
            }

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
            StopLoopSfx();
            if (_activeProjectile != null)
            {
                _activeProjectile.Cancel();
                _activeProjectile = null;
            }
            _isThrowing.Value = false;
            _ropeView?.ClearHookTransform();
        }

        private void StopLoopSfx()
        {
            if (!string.IsNullOrEmpty(_activeLoopId))
            {
                _audioService.StopSfx(_activeLoopId);
                _activeLoopId = null;
            }
        }
    }
}