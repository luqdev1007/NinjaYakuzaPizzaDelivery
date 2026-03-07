using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class GlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ICompositeCondition _canGlide;
        private ReactiveVariable<bool> _isGliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _minFallVelocity;
        private ReactiveVariable<float> _glideMaxFallSpeed;
        private ReactiveVariable<float> _glideSpeedDamping;
        private ReactiveVariable<float> _glideBounceForce;
        private Rigidbody2D _rigidbody;
        private float _defaultGravityScale;

        private float _glideBufferTimer;
        private const float GlideBufferTime = 0.2f;

        public GlideSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _isGliding = entity.IsGliding;
            _isGrounded = entity.IsGrounded;
            _minFallVelocity = entity.MinFallVelocityForAction;
            _glideMaxFallSpeed = entity.GlideMaxFallSpeed;
            _glideSpeedDamping = entity.GlideSpeedDamping;
            _glideBounceForce = entity.GlideBounceForce;
            _rigidbody = entity.Rigidbody;
            _canGlide = entity.CanGlide;
            _defaultGravityScale = _rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isGrounded.Value && _isGliding.Value)
                StopGlide(applyBounce: false);

            bool isFalling = _rigidbody.linearVelocity.y < _minFallVelocity.Value;

            if (_inputService.IsJumpKeyPressed)
                _glideBufferTimer = GlideBufferTime;
            else
                _glideBufferTimer -= deltaTime;

            if (_glideBufferTimer > 0f && isFalling && _canGlide.Evaluate())
            {
                StartGlide();
                _glideBufferTimer = 0f;
            }

            if (_isGliding.Value && _inputService.IsJumpKeyReleased)
                StopGlide(applyBounce: true);

            if (_isGliding.Value)
                ApplyGlideDamping(deltaTime);
        }

        private void StartGlide()
        {
            _isGliding.Value = true;
            _rigidbody.gravityScale = 0f;
        }

        private void StopGlide(bool applyBounce)
        {
            _isGliding.Value = false;
            _rigidbody.gravityScale = _defaultGravityScale;

            if (applyBounce)
            {
                _rigidbody.linearVelocity = new Vector2(
                    _rigidbody.linearVelocity.x,
                    _glideBounceForce.Value);
            }
        }

        private void ApplyGlideDamping(float deltaTime)
        {
            float targetY = _glideMaxFallSpeed.Value;
            float currentY = _rigidbody.linearVelocity.y;

            float newY = Mathf.MoveTowards(
                currentY,
                targetY,
                _glideSpeedDamping.Value * deltaTime);

            _rigidbody.linearVelocity = new Vector2(
                _rigidbody.linearVelocity.x,
                newY);
        }
    }
}

