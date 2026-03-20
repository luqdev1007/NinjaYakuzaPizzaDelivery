using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _slideDuration;
        private ReactiveVariable<float> _slideSpeed;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;
        private Vector2 _slideColliderSize;
        private Vector2 _slideColliderOffset;

        public SlideSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _canSlide = entity.CanSlide;
            _isSliding = entity.IsSliding;
            _isGrounded = entity.IsGrounded;
            _slideDuration = entity.SlideDuration;
            _slideSpeed = entity.SlideSpeed;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.BodyCollider;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.25f));
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isSliding.Value)
                return;

            if (_inputService.IsSlideKeyPressed && _canSlide.Evaluate() && _isGrounded.Value)
                _coroutinesPerformer.StartPerform(SlideCoroutine());
        }

        private IEnumerator SlideCoroutine()
        {
            _isSliding.Value = true;
            SetSlideCollider(true);

            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            float elapsed = 0f;
            float duration = _slideDuration.Value;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t * t);
                _rigidbody.linearVelocity = new Vector2(
                    direction * currentSpeed,
                    _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
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
    }
}