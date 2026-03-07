using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using System.Collections.Generic;
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
        private ReactiveVariable<float> _grappleSpeed;
        private ReactiveVariable<float> _projectileSpeed;
        private ReactiveVariable<float> _arriveDistance;
        private ReactiveVariable<Vector3> _anchorPoint;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private GameObject _hookInstance;
        private Coroutine _grappleCoroutine;
        private float _defaultGravityScale;

        private GrappleRopeView _ropeView;

        private ReactiveVariable<bool> _isThrowingHook;

        public GrappleSystem(
            IInputService inputService,
            ICoroutinesPerformer coroutinesPerformer,
            LayerMask grappleMask,
            string projectilePrefabPath)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _grappleMask = grappleMask;
            _projectilePrefabPath = projectilePrefabPath;
        }

        public void OnInit(Entity entity)
        {
            _canGrapple = entity.CanGrapple;
            _isGrappling = entity.IsGrappling;
            _grappleSpeed = entity.GrappleSpeed;
            _projectileSpeed = entity.GrappleProjectileSpeed;
            _arriveDistance = entity.GrappleArriveDistance;
            _anchorPoint = entity.GrappleAnchorPoint;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

            _isThrowingHook = entity.IsThrowingHook;

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

            GameObject prefab = Resources.Load<GameObject>(_projectilePrefabPath);

            if (prefab == null)
            {
                Debug.LogError($"GrappleSystem: префаб не найден по пути '{_projectilePrefabPath}'");
                return;
            }

            _hookInstance = Object.Instantiate(prefab, _transform.position, Quaternion.identity);
            _isThrowingHook.Value = true;
            _ropeView?.SetHookTransform(_hookInstance.transform);

            _grappleCoroutine = _coroutinesPerformer.StartPerform(
                HookFlyCoroutine(direction));
        }

        private IEnumerator HookFlyCoroutine(Vector3 direction)
        {
            Transform hook = _hookInstance.transform;

            while (true)
            {
                if (_inputService.IsGrappleKeyReleased)
                {
                    StopGrapple();
                    yield break;
                }

                hook.position += direction * _projectileSpeed.Value * Time.deltaTime;

                Collider2D hit = Physics2D.OverlapPoint(hook.position, _grappleMask);

                if (hit != null)
                {
                    _anchorPoint.Value = hook.position;
                    _coroutinesPerformer.StartPerform(PullCoroutine());
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
                    StopGrapple();
                    yield break;
                }

                _rigidbody.linearVelocity = toAnchor.normalized * _grappleSpeed.Value;

                yield return null;
            }
        }

        private void StopGrapple()
        {
            _isThrowingHook.Value = false;

            _ropeView.ClearHookTransform();

            _isGrappling.Value = false;
            _rigidbody.gravityScale = _defaultGravityScale;
            _rigidbody.linearVelocity = Vector2.zero;

            if (_hookInstance != null)
            {
                Object.Destroy(_hookInstance);
                _hookInstance = null;
            }
        }
    }
}