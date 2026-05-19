using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canSlide;

        private ReactiveVariable<bool> _isSliding;
        private ReactiveEvent _slideRequest;
        private ReactiveVariable<Vector2> _moveDirection;

        private ReactiveVariable<float> _slideDuration;
        private ReactiveVariable<float> _slideSpeed;

        private IDisposable _slideRequestDisposable;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;

        private Vector2 _slideColliderSize;
        private Vector2 _slideColliderOffset;

        public SlideSystem(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        { 
            _canSlide = entity.CanSlide;
            _moveDirection = entity.MoveDirection;
            _isSliding = entity.IsSliding;
            _slideDuration = entity.SlideDuration;
            _slideSpeed = entity.SlideSpeed;
            _slideRequest = entity.SlideRequest;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.BodyCollider;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;

                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y / 2);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.1f));
            }

            _slideRequestDisposable = _slideRequest.Subscribe(OnSlideRequested);
        }

        private void OnSlideRequested()
        {
            if (_canSlide.Evaluate())
                _coroutinesPerformer.StartPerform(SlideCoroutine());
        }

        private IEnumerator SlideCoroutine()
        {
            _isSliding.Value = true;

            SetSlideCollider(true);

            float elapsed = 0f;
            float duration = _slideDuration.Value;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t * t);

                _rigidbody.linearVelocityX = _moveDirection.Value.x * currentSpeed;

                elapsed += Time.deltaTime;

                yield return null;
            }

            _rigidbody.linearVelocityX = 0;

            SetSlideCollider(false);

            _isSliding.Value = false;
        }

        private void SetSlideCollider(bool sliding)
        {
            if (_collider is not CapsuleCollider2D capsule)
                return;

            capsule.size = sliding ? _slideColliderSize : _defaultColliderSize;
            capsule.offset = sliding ? _slideColliderOffset : _defaultColliderOffset;
        }

        public void OnDispose()
        {
            SetSlideCollider(false);
            _slideRequestDisposable?.Dispose();
        }
    }
}