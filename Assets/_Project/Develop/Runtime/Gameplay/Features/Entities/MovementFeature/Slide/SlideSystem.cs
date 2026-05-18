using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _intentSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _lookDirectionX;

        private ReactiveVariable<float> _slideSpeed;
        private ReactiveVariable<float> _slideDuration;
        private ReactiveVariable<float> _slideCooldown;

        private ReactiveVariable<Vector2> _slideHitBoxSize;

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;

        private const float SlideBufferTimeMax = 0.15f;
        private float _slideBufferTimer;
        private float _cooldownTimer;
        private bool _wasSlideIntendedLastFrame;

        public SlideSystem(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _canSlide = entity.CanSlide;
            _intentSlide = entity.IntentSlide;
            _isSliding = entity.IsSliding;
            _lookDirectionX = entity.LookDirectionX;

            _slideSpeed = entity.SlideSpeed;
            _slideDuration = entity.SlideDuration;
            _slideCooldown = entity.SlideCooldown;

            _slideHitBoxSize = entity.SlideHitBoxSize;

            _rigidbody = entity.Rigidbody;
            _collider = entity.BodyCollider;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
            }
        }

        public void OnUpdate(float deltaTime)
        {
            float unscaledDt = Time.unscaledDeltaTime;
            bool currentIntent = _intentSlide.Value;
            bool isPressedDown = currentIntent && !_wasSlideIntendedLastFrame;
            _wasSlideIntendedLastFrame = currentIntent;

            if (_cooldownTimer > 0f)
                _cooldownTimer -= unscaledDt;

            if (isPressedDown)
                _slideBufferTimer = SlideBufferTimeMax;
            else if (_slideBufferTimer > 0f)
                _slideBufferTimer -= unscaledDt;

            if (_isSliding.Value && !_canSlide.Evaluate())
            {
                InterruptSlide();
                return;
            }

            if (_slideBufferTimer > 0f && _canSlide.Evaluate() && _cooldownTimer <= 0 && !_isSliding.Value)
            {
                _slideBufferTimer = 0f;
                ExecuteSlide();
            }
        }

        private void ExecuteSlide()
        {
            _isSliding.Value = true;
            _cooldownTimer = _slideCooldown.Value;
            SetSlideCollider(true);

            _coroutinesPerformer.StartPerform(SlideCoroutine(_slideSpeed.Value, _lookDirectionX.Value));
        }

        private IEnumerator SlideCoroutine(float speed, float direction)
        {
            float elapsed = 0f;
            float duration = _slideDuration.Value;

            while (elapsed < duration)
            {
                if (!_isSliding.Value)
                    yield break;

                float t = elapsed / duration;
                float speedCurve = 1f - t * t;
                float currentSpeed = speed * speedCurve;

                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }

            EndSlide();
        }

        private void InterruptSlide()
        {
            _isSliding.Value = false;
            SetSlideCollider(false);
        }

        private void EndSlide()
        {
            _isSliding.Value = false;
            SetSlideCollider(false);
            _rigidbody.linearVelocity = new Vector2(directionX() * 2f, _rigidbody.linearVelocity.y);
        }

        private float directionX() => Mathf.Sign(_lookDirectionX.Value);

        private void SetSlideCollider(bool isSliding)
        {
            if (_collider is CapsuleCollider2D capsule)
            {
                if (isSliding)
                {
                    Vector2 targetSize = _slideHitBoxSize.Value;

                    float heightDifference = _defaultColliderSize.y - targetSize.y;
                    Vector2 targetOffset = new Vector2(_defaultColliderOffset.x, _defaultColliderOffset.y - (heightDifference * 0.5f));

                    capsule.size = targetSize;
                    capsule.offset = targetOffset;
                    capsule.direction = CapsuleDirection2D.Horizontal;
                }
                else
                {
                    capsule.size = _defaultColliderSize;
                    capsule.offset = _defaultColliderOffset;
                    capsule.direction = CapsuleDirection2D.Vertical;
                }
            }
        }
    }
}