using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly LayerMask _grappleMask;
        private readonly string _projectilePrefabPath;

        private ICompositeCondition _canGrapple;
        private ReactiveVariable<bool> _isGrappling;
        private ReactiveVariable<bool> _isThrowingHook;
        private ReactiveVariable<float> _grappleSpeed;
        private ReactiveVariable<float> _projectileSpeed;
        private ReactiveVariable<float> _arriveDistance;
        private ReactiveVariable<float> _minDistance;
        private ReactiveVariable<float> _maxDistance;
        private ReactiveVariable<float> _grappleArrivalBounce;
        private ReactiveVariable<Vector3> _anchorPoint;
        private ReactiveEvent _startAttackRequest;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private LayerMask _enemyMask;

        private GameObject _hookInstance;
        private float _defaultGravityScale;
        private GrappleRopeView _ropeView;

        public GrappleSystem(
            IInputService inputService,
            ICoroutinesPerformer coroutinesPerformer,
            LayerMask grappleMask,
            LayerMask enemyMask,
            string projectilePrefabPath)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _grappleMask = grappleMask;
            _enemyMask = enemyMask;
            _projectilePrefabPath = projectilePrefabPath;
        }

        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.StartAttackRequest;

            _canGrapple = entity.CanGrapple;
            _isGrappling = entity.IsGrappling;
            _isThrowingHook = entity.IsThrowingHook;
            _grappleSpeed = entity.GrappleSpeed;
            _projectileSpeed = entity.GrappleProjectileSpeed;
            _arriveDistance = entity.GrappleArriveDistance;
            _minDistance = entity.GrappleMinDistance;
            _maxDistance = entity.GrappleMaxDistance;
            _grappleArrivalBounce = entity.GrappleArrivalBounce;
            _anchorPoint = entity.GrappleAnchorPoint;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _ropeView = entity.Transform.GetComponentInChildren<GrappleRopeView>();
            _defaultGravityScale = _rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inputService.IsGrappleKeyPressed && _canGrapple.Evaluate())
                LaunchHook();

            if (_inputService.IsGrappleKeyReleased && _isGrappling.Value)
                StopGrapple();
        }

        private void LaunchHook()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = _transform.position.z;
            Vector3 direction = (mouseWorld - _transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(
                _transform.position,
                direction,
                _maxDistance.Value,
                _grappleMask);

            if (hit.collider != null && hit.distance < _minDistance.Value)
                return;

            GameObject prefab = Resources.Load<GameObject>(_projectilePrefabPath);

            if (prefab == null)
            {
                Debug.LogError($"GrappleSystem: префаб не найден по пути '{_projectilePrefabPath}'");
                return;
            }

            _hookInstance = Object.Instantiate(prefab, _transform.position, Quaternion.identity);
            _isThrowingHook.Value = true;
            _ropeView?.SetHookTransform(_hookInstance.transform);

            _coroutinesPerformer.StartPerform(HookFlyCoroutine(direction));
        }

        private IEnumerator HookFlyCoroutine(Vector3 direction)
        {
            Transform hook = _hookInstance.transform;
            Vector3 startPosition = hook.position;

            while (true)
            {
                if (_inputService.IsGrappleKeyReleased)
                {
                    StopGrapple();
                    yield break;
                }

                hook.position += direction * _projectileSpeed.Value * Time.deltaTime;

                float travelledDistance = Vector3.Distance(startPosition, hook.position);

                if (travelledDistance >= _maxDistance.Value)
                {
                    _coroutinesPerformer.StartPerform(ReturnHookCoroutine(startPosition));
                    yield break;
                }

                Collider2D hit = Physics2D.OverlapPoint(hook.position, _grappleMask);

                if (hit != null)
                {
                    bool hitEnemy = (_enemyMask.value & (1 << hit.gameObject.layer)) != 0;

                    if (hitEnemy)
                    {
                        Debug.Log($"Крюк попал во врага: {hit.gameObject.name}");
                        _anchorPoint.Value = hook.position;
                        _coroutinesPerformer.StartPerform(PullToEnemyCoroutine(hit));
                    }
                    else
                    {
                        _anchorPoint.Value = hook.position;
                        _coroutinesPerformer.StartPerform(PullCoroutine());
                    }

                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator PullToEnemyCoroutine(Collider2D enemy)
        {
            _isGrappling.Value = true;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;

            while (true)
            {
                if (_inputService.IsGrappleKeyReleased)
                {
                    StopGrapple();
                    yield break;
                }

                // враг мог умереть пока летели
                if (enemy == null || !enemy.gameObject.activeSelf)
                {
                    StopGrapple();
                    yield break;
                }

                // обновляем точку якоря если враг движется
                _anchorPoint.Value = enemy.transform.position;

                Vector3 toEnemy = _anchorPoint.Value - _transform.position;
                float distance = toEnemy.magnitude;

                if (distance <= _arriveDistance.Value)
                {
                    ArriveAtEnemy(enemy);
                    yield break;
                }

                _rigidbody.linearVelocity = toEnemy.normalized * _grappleSpeed.Value;

                yield return null;
            }
        }

        private void ArriveAtEnemy(Collider2D enemy)
        {
            Debug.Log($"Герой долетел до врага: {enemy.gameObject.name} — автоатака!");
            StopGrapple(); // сначала сбрасываем IsGrappling
            _rigidbody.linearVelocity = new Vector2(
                _rigidbody.linearVelocity.x,
                _grappleArrivalBounce.Value);
            _startAttackRequest.Invoke(); // потом запрашиваем атаку
        }

        private IEnumerator ReturnHookCoroutine(Vector3 returnTarget)
        {
            Transform hook = _hookInstance.transform;

            while (true)
            {
                if (_inputService.IsGrappleKeyReleased)
                {
                    StopGrapple();
                    yield break;
                }

                hook.position = Vector3.MoveTowards(
                    hook.position,
                    returnTarget,
                    _projectileSpeed.Value * 2f * Time.deltaTime);

                if (Vector3.Distance(hook.position, returnTarget) <= 0.1f)
                {
                    StopGrapple();
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator PullCoroutine()
        {
            _isGrappling.Value = true;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;

            while (true)
            {
                if (_inputService.IsGrappleKeyReleased)
                {
                    StopGrapple();
                    yield break;
                }

                Vector3 toAnchor = _anchorPoint.Value - _transform.position;
                float distance = toAnchor.magnitude;

                if (distance <= _arriveDistance.Value)
                {
                    ArriveAtAnchor();
                    yield break;
                }

                _rigidbody.linearVelocity = toAnchor.normalized * _grappleSpeed.Value;

                yield return null;
            }
        }

        private void ArriveAtAnchor()
        {
            StopGrapple();
            _rigidbody.linearVelocity = new Vector2(
                _rigidbody.linearVelocity.x,
                _grappleArrivalBounce.Value);
        }

        private void StopGrapple()
        {
            _isThrowingHook.Value = false;
            _isGrappling.Value = false;
            _rigidbody.gravityScale = _defaultGravityScale;
            _rigidbody.linearVelocity = Vector2.zero;
            _ropeView?.ClearHookTransform();

            if (_hookInstance != null)
            {
                Object.Destroy(_hookInstance);
                _hookInstance = null;
            }
        }
    }
}