using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float SlopeSlideMaxDuration = 2.5f;
        private const float SlopeSlideSpeedBonus = 0.6f;

        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly SlopeSystem _slopeSystem;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<float> _slideSpeed;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize, _defaultColliderOffset;
        private Vector2 _slideColliderSize, _slideColliderOffset;

        public SlideSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer, SlopeSystem slopeSystem)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _slopeSystem = slopeSystem;
        }

        public void OnInit(Entity entity)
        {
            _canSlide = entity.CanSlide;
            _isSliding = entity.IsSliding;
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _slideSpeed = entity.SlideSpeed;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.BodyCollider;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.2f));
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isSliding.Value) return;

            // 1. Активация по кнопке
            if (_inputService.IsSlideKeyPressed && _canSlide.Evaluate())
            {
                if (_isOnSlope.Value) _coroutinesPerformer.StartPerform(SlopeSlideCoroutine());
                else if (_isGrounded.Value) _coroutinesPerformer.StartPerform(SlideCoroutine());
                return;
            }

            // 2. АВТО-ПОДКАТ ПРИ ПРИЗЕМЛЕНИИ НА СКЛОН
            if (_isOnSlope.Value && _slopeAccumSpeed.Value > 7f && _canSlide.Evaluate())
            {
                _coroutinesPerformer.StartPerform(SlopeSlideCoroutine());
            }
        }

        private IEnumerator SlideCoroutine()
        {
            _isSliding.Value = true;
            SetSlideCollider(true);
            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            float elapsed = 0f;
            float duration = 0.6f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t * t);
                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetSlideCollider(false);
            _isSliding.Value = false;
        }

        private IEnumerator SlopeSlideCoroutine()
        {
            _isSliding.Value = true;
            SetSlideCollider(true);
            float elapsed = 0f;

            while (_isOnSlope.Value && elapsed < SlopeSlideMaxDuration)
            {
                Vector2 slopeNormal = _slopeSystem.SlopeNormal;
                Vector2 downhill = new Vector2(slopeNormal.y, -slopeNormal.x);
                if (downhill.y > 0f) downhill = -downhill;

                float speed = _slideSpeed.Value + (_slopeAccumSpeed.Value * SlopeSlideSpeedBonus);
                _rigidbody.AddForce(downhill * speed, ForceMode2D.Force);
                _rigidbody.AddForce(-slopeNormal * 10f, ForceMode2D.Force);

                // --- ПРИНУДИТЕЛЬНЫЙ РАЗВОРОТ ПО ДВИЖЕНИЮ ---
                if (Mathf.Abs(_rigidbody.linearVelocity.x) > 0.2f)
                {
                    float direction = _rigidbody.linearVelocity.x > 0 ? 1f : -1f;
                    Vector3 scale = _transform.localScale;
                    scale.x = direction * Mathf.Abs(scale.x);
                    _transform.localScale = scale;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            SetSlideCollider(false);
            _isSliding.Value = false;
        }

        private void SetSlideCollider(bool sliding)
        {
            if (_collider is CapsuleCollider2D capsule)
            {
                capsule.size = sliding ? _slideColliderSize : _defaultColliderSize;
                capsule.offset = sliding ? _slideColliderOffset : _defaultColliderOffset;
            }
        }
    }
}