using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ThrowableSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly ThrowableConfig[] _configs;
        private readonly IThrowableBehaviourFactory _behaviourFactory;

        private ICompositeCondition _canThrow;
        private ReactiveVariable<int> _currentIndex;
        private ReactiveVariable<bool> _isThrowing;
        private ReactiveEvent _startAttackRequest;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private GrappleRopeView _ropeView;

        private Dictionary<int, ReactiveVariable<int>> _charges;
        private ThrowableProjectile _activeProjectile;
        private Coroutine _pullCoroutine;

        private float _defaultGravity;
        private bool _isPulling;
        private Vector2 _lastPullDirection; // Для инерции

        public ThrowableSystem(IInputService input, ICoroutinesPerformer performer, ThrowableConfig[] configs, IThrowableBehaviourFactory factory)
        {
            _inputService = input;
            _coroutinesPerformer = performer;
            _configs = configs;
            _behaviourFactory = factory;
        }

        public void OnInit(Entity entity)
        {
            _canThrow = entity.CanGrapple;
            _currentIndex = entity.CurrentThrowableIndex;
            _isThrowing = entity.IsThrowing;
            _startAttackRequest = entity.StartAttackRequest;
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;
            _ropeView = entity.Transform.GetComponentInChildren<GrappleRopeView>();

            _defaultGravity = _rigidbody.gravityScale;

            _charges = new Dictionary<int, ReactiveVariable<int>>
            {
                { 0, entity.GrappleCharges },
                { 1, entity.ShurikenCharges },
                { 2, entity.SleepDartCharges }
            };
        }

        public void OnUpdate(float deltaTime)
        {
            HandleScroll();

            bool isGrappleActive = _activeProjectile is GrappleHookProjectile;

            if (_inputService.IsGrappleKeyPressed && _canThrow.Evaluate() && !isGrappleActive && !_isPulling)
                TryLaunch();

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
            int idx = _currentIndex.Value;
            if (_charges[idx].Value <= 0) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = (mousePos - _transform.position).normalized;
            dir.z = 0;

            _charges[idx].Value--;
            _isThrowing.Value = true;
            _activeProjectile = _behaviourFactory.Create(_configs[idx], _rigidbody, _transform);

            if (_activeProjectile is GrappleHookProjectile grapple)
            {
                grapple.OnAnchored += (pos, hit) => StartPulling(pos, hit);
                _activeProjectile.Launch(_transform.position, dir);
                _ropeView?.SetHookTransform(grapple.Instance.transform);
            }
            else
            {
                _activeProjectile.Launch(_transform.position, dir);
                _activeProjectile.OnCompleted += () => _isThrowing.Value = false;
            }
        }

        private void StartPulling(Vector2 anchorPos, Collider2D hit)
        {
            _isPulling = true;
            // Вычисляем смещение относительно центра объекта, чтобы крюк "приклеился" к точке попадания
            Vector2 localOffset = (Vector2)hit.transform.InverseTransformPoint(anchorPos);
            _pullCoroutine = _coroutinesPerformer.StartPerform(PullRoutine(localOffset, hit));
        }

        private IEnumerator PullRoutine(Vector2 localOffset, Collider2D hit)
        {
            _rigidbody.gravityScale = 0.2f;
            var config = (GrappleHookConfig)_configs[0];

            while (hit != null && _inputService.IsGrappleKeyHeld)
            {
                // Актуальная мировая точка на движущемся объекте
                Vector2 currentAnchorWorld = (Vector2)hit.transform.TransformPoint(localOffset);
                Vector2 toTarget = currentAnchorWorld - (Vector2)_transform.position;
                float dist = toTarget.magnitude;

                _lastPullDirection = toTarget.normalized;
                _ropeView?.FixateToPoint(currentAnchorWorld);

                if (dist <= config.ArriveDistance)
                {
                    if (hit.CompareTag("Enemy")) _startAttackRequest.Invoke();
                    // Чтобы можно было "кружиться", не гасим скорость в ноль
                    _rigidbody.linearVelocity *= 0.98f;
                }
                else
                {
                    // Прогрессивное ускорение для "эффекта рогатки"
                    float forceMultiplier = Mathf.Clamp(dist / 2f, 1f, 2.5f);
                    _rigidbody.AddForce(_lastPullDirection * config.GrappleSpeed * forceMultiplier, ForceMode2D.Force);
                }

                yield return new WaitForFixedUpdate();
            }

            StopPulling(applyInertia: true);
        }

        private void StopPulling(bool applyInertia)
        {
            if (!_isPulling && _activeProjectile == null) return;

            _rigidbody.gravityScale = _defaultGravity;

            if (applyInertia && _isPulling)
            {
                // МОЩНАЯ ИНЕРЦИЯ (Slingshot effect)
                // Даем импульс в сторону последнего натяжения + небольшой "подброс" вверх
                Vector2 launchDirection = (_lastPullDirection + Vector2.up * 0.4f).normalized;
                float currentSpeed = _rigidbody.linearVelocity.magnitude;
                float finalBoost = Mathf.Max(currentSpeed, 15f); // Минимум 15 для ощутимого вылета

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

        private void HandleScroll()
        {
            float s = Input.GetAxisRaw("Mouse ScrollWheel");
            if (s == 0) return;
            _currentIndex.Value = (_currentIndex.Value + (s > 0 ? 1 : -1) + _configs.Length) % _configs.Length;
        }
    }
}