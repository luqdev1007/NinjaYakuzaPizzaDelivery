using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
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
        private ReactiveVariable<bool> _isThrowingHook;
        private ReactiveEvent _startAttackRequest;
        private Transform _transform;
        private Rigidbody2D _rigidbody;
        private Dictionary<int, ReactiveVariable<int>> _charges;
        private ThrowableProjectile _activeProjectile;
        private GrappleRopeView _ropeView;

        public ThrowableSystem(
            IInputService inputService,
            ICoroutinesPerformer coroutinesPerformer,
            ThrowableConfig[] configs,
            IThrowableBehaviourFactory behaviourFactory)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _configs = configs;
            _behaviourFactory = behaviourFactory;
        }

        public void OnInit(Entity entity)
        {
            _canThrow = entity.CanGrapple;
            _currentIndex = entity.CurrentThrowableIndex;
            _isThrowingHook = entity.IsThrowingHook;
            _startAttackRequest = entity.StartAttackRequest;
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;
            _ropeView = entity.Transform.GetComponentInChildren<GrappleRopeView>();

            _charges = new Dictionary<int, ReactiveVariable<int>>
            {
                { 0, entity.GrappleCharges },
                { 1, entity.ShurikenCharges },
                { 2, entity.SleepDartCharges }
            };
        }

        public void OnUpdate(float deltaTime)
        {
            HandleScrollInput();

            if (_inputService.IsGrappleKeyPressed && _canThrow.Evaluate() && _activeProjectile == null)
                TryLaunch();

            if (_inputService.IsGrappleKeyReleased && _activeProjectile is GrappleHookProjectile)
                CancelActive();
        }

        private void HandleScrollInput()
        {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

            if (scroll > 0f)
                _currentIndex.Value = (_currentIndex.Value + 1) % _configs.Length;
            else if (scroll < 0f)
                _currentIndex.Value = (_currentIndex.Value - 1 + _configs.Length) % _configs.Length;

            if (scroll != 0f)
            {
                int index = _currentIndex.Value;
                Debug.Log($"Прожектайл: {_configs[index].name} | Зарядов: {_charges[index].Value}");
            }
        }

        private void TryLaunch()
        {
            int index = _currentIndex.Value;
            ThrowableConfig config = _configs[index];

            if (_charges[index].Value <= 0)
            {
                Debug.Log($"Нет зарядов: {config.name}");
                return;
            }

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = _transform.position.z;
            Vector3 direction = (mouseWorld - _transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(
                _transform.position,
                direction,
                config.MaxDistance,
                config.HitMask);

            if (hit.collider != null && hit.distance < config.MinDistance)
                return;

            _charges[index].Value--;
            _activeProjectile = _behaviourFactory.Create(config, _rigidbody, _transform);

            if (_activeProjectile is GrappleHookProjectile grapple)
            {
                grapple.SetCancelCondition(() => _inputService.IsGrappleKeyReleased);
                grapple.OnGrappleStarted += OnGrappleStarted;
                grapple.OnGrappleEnded += OnGrappleEnded;
                grapple.OnEnemyArrived += OnEnemyArrived;
                _isThrowingHook.Value = true;
            }

            _activeProjectile.OnCompleted += OnProjectileCompleted;
            _activeProjectile.Launch(_transform.position, direction);

            if (_activeProjectile is GrappleHookProjectile grapplerAfterLaunch)
                _ropeView?.SetHookTransform(grapplerAfterLaunch.Instance.transform);
        }

        private void CancelActive()
        {
            _activeProjectile?.Cancel();
            _activeProjectile = null;
            _isThrowingHook.Value = false;
        }

        private void OnProjectileCompleted()
        {
            _activeProjectile = null;
        }

        private void OnGrappleStarted() { }

        private void OnGrappleEnded()
        {
            _activeProjectile = null;
            _isThrowingHook.Value = false;
        }

        private void OnEnemyArrived()
        {
            _startAttackRequest.Invoke();
        }
    }
}