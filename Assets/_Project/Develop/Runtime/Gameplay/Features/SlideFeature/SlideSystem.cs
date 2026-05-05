using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float GroundSlideDuration = 0.5f;
        private const float SlideCooldown = 0.2f;

        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ReactiveEvent _slideRequest;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _slideSpeed;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize, _defaultColliderOffset;
        private Vector2 _slideColliderSize, _slideColliderOffset;
        private float _cooldownTimer;

        public SlideSystem(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _slideRequest = entity.SlideRequest;

            _canSlide = entity.CanSlide;
            _isSliding = entity.IsSliding;
            _isGrounded = entity.IsGrounded;
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

            _slideRequest.Subscribe(OnSlideRequested);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= deltaTime;
        }

        private void OnSlideRequested()
        {
            if (!_isSliding.Value && _isGrounded.Value && _canSlide.Evaluate() && _cooldownTimer <= 0)
            {
                _coroutinesPerformer.StartPerform(SlideCoroutine());
            }
        }

        private IEnumerator SlideCoroutine()
        {
            StartSlide();

            float direction = Mathf.Sign(_transform.localScale.x);
            float elapsed = 0f;

            while (elapsed < GroundSlideDuration)
            {
                if (!_isGrounded.Value) 
                    break;

                float t = elapsed / GroundSlideDuration;
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t);

                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }

            EndSlide();
        }

        private void StartSlide()
        {
            _isSliding.Value = true;
            SetSlideCollider(true);
        }

        private void EndSlide()
        {
            SetSlideCollider(false);
            _isSliding.Value = false;
            _cooldownTimer = SlideCooldown;
        }

        private void SetSlideCollider(bool isSliding)
        {
            if (_collider is CapsuleCollider2D capsule)
            {
                capsule.size = isSliding ? _slideColliderSize : _defaultColliderSize;
                capsule.offset = isSliding ? _slideColliderOffset : _defaultColliderOffset;
            }
        }
    }
}