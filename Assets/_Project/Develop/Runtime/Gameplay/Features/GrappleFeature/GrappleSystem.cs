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
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GrappleHookConfig _config; // Используем конкретный конфиг
        private readonly IThrowableBehaviourFactory _behaviourFactory;

        private ICompositeCondition _canThrow;
        private ReactiveVariable<bool> _isThrowing;
        private ReactiveVariable<int> _charges;
        private ReactiveEvent _startAttackRequest;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private GrappleRopeView _ropeView;

        private ThrowableProjectile _activeProjectile;
        private Coroutine _pullCoroutine;

        private float _defaultGravity;
        private bool _isPulling;
        private Vector2 _lastPullDirection; 

        public GrappleSystem(IInputService input, ICoroutinesPerformer performer, GrappleHookConfig config, IThrowableBehaviourFactory factory)
        {
            _inputService = input;
            _coroutinesPerformer = performer;
            _config = config;
            _behaviourFactory = factory;
        }

        public void OnInit(Entity entity)
        {
            _canThrow = entity.CanGrapple;
            _isThrowing = entity.IsThrowing;
            _charges = entity.GrappleCharges;
            _startAttackRequest = entity.StartAttackRequest;
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;
            _ropeView = entity.Transform.GetComponentInChildren<GrappleRopeView>();

            _defaultGravity = _rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            bool isGrappleActive = _activeProjectile is GrappleHookProjectile;

            // Запуск на RMB (IsGrappleKeyPressed)
            if (_inputService.IsGrappleKeyPressed && _canThrow.Evaluate() && !isGrappleActive && !_isPulling)
                TryLaunch();

            // Отмена при отпускании RMB
            if (_inputService.IsGrappleKeyReleased)
            {
                if (isGrappleActive || _isPulling)
                {
                    StopPulling(applyInertia: true);
                }
            }
        }

        private void TryLaunch()
        {
            if (_charges.Value <= 0) return;

            // Проверка на наличие камеры (редко, но бывает причиной крэша при переключении сцен)
            if (Camera.main == null) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = (mousePos - _transform.position).normalized;
            dir.z = 0;

            _charges.Value--;
            _isThrowing.Value = true;

            _activeProjectile = _behaviourFactory.Create(_config, _rigidbody, _transform);

            // Проверяем, что снаряд вообще создался
            if (_activeProjectile != null && _activeProjectile is GrappleHookProjectile grapple)
            {
                grapple.OnAnchored += (pos, hit) => StartPulling(pos, hit);
                _activeProjectile.Launch(_transform.position, dir);

                // Безопасная установка трансформа: проверяем и View, и сам Instance
                if (_ropeView != null && grapple.Instance != null)
                {
                    _ropeView.SetHookTransform(grapple.Instance.transform);
                }

                _activeProjectile.OnCompleted += () =>
                {
                    if (!_isPulling) _isThrowing.Value = false;
                };
            }
            else
            {
                // Если не удалось создать снаряд, возвращаем состояние
                _isThrowing.Value = false;
            }
        }

        private IEnumerator PullRoutine(Vector2 localOffset, Collider2D hit)
        {
            _rigidbody.gravityScale = 0.2f;

            // Добавляем проверку на существование самого трансформа hit.transform
            while (hit != null && hit.transform != null && _inputService.IsGrappleKeyHeld)
            {
                Vector2 currentAnchorWorld = (Vector2)hit.transform.TransformPoint(localOffset);
                Vector2 toTarget = currentAnchorWorld - (Vector2)_transform.position;
                float dist = toTarget.magnitude;

                _lastPullDirection = toTarget.normalized;
                _ropeView?.FixateToPoint(currentAnchorWorld);

                if (dist <= _config.ArriveDistance)
                {
                    // Безопасная проверка тега
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
            _isThrowing.Value = false;
            _ropeView?.ClearHookTransform();
        }
    }
}