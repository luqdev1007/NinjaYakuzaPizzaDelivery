using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide
{
    public class SlideSystem : IInitializableSystem, IFixedUpdatableSystem
    {
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

        // Окно движения слайда: бывшая SlideCoroutine, развёрнутая в state-машину
        // на физ-тике. Скорость/направление/длительность фиксируются на старте.
        private bool _isInSlideWindow;
        private float _slideWindowElapsed;
        private float _slideWindowDuration;
        private float _slideWindowSpeed;
        private float _slideWindowDirection;

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

        public void OnFixedUpdate(float deltaTime)
        {
            // Кулдаун и буфер сохраняют иммунитет к хитстопу (Time.timeScale):
            // fixedUnscaledDeltaTime — прямой fixed-эквивалент unscaledDeltaTime.
            float unscaledDt = Time.fixedUnscaledDeltaTime;
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

            // Окно прокручивается в том же тике, где стартовало: корутина
            // выполнялась синхронно до первого yield, т.е. её первый проход тела
            // цикла (t=0) и первый инкремент elapsed приходились на кадр старта.
            if (_isInSlideWindow)
                AdvanceSlideWindow(deltaTime);
        }

        private void ExecuteSlide()
        {
            _isSliding.Value = true;
            _cooldownTimer = _slideCooldown.Value;
            SetSlideCollider(true);

            _slideWindowSpeed = _slideSpeed.Value;
            _slideWindowDirection = _lookDirectionX.Value;
            _slideWindowDuration = _slideDuration.Value;
            _slideWindowElapsed = 0f;
            _isInSlideWindow = true;
        }

        private void AdvanceSlideWindow(float deltaTime)
        {
            // Корутина проверяла флаг в начале тела цикла и выходила БЕЗ записи и
            // без финального нуджа — сохраняем: внешний сброс _isSliding просто
            // гасит окно.
            if (_isSliding.Value == false)
            {
                _isInSlideWindow = false;
                return;
            }

            if (_slideWindowElapsed < _slideWindowDuration)
            {
                float t = _slideWindowElapsed / _slideWindowDuration;
                float speedCurve = 1f - t * t;
                float currentSpeed = _slideWindowSpeed * speedCurve;

                _rigidbody.linearVelocity = new Vector2(_slideWindowDirection * currentSpeed, _rigidbody.linearVelocity.y);

                _slideWindowElapsed += deltaTime;

                return;
            }

            EndSlide();
        }

        private void InterruptSlide()
        {
            _isInSlideWindow = false;
            _isSliding.Value = false;
            SetSlideCollider(false);
        }

        private void EndSlide()
        {
            _isInSlideWindow = false;
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
