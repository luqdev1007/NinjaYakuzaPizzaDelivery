using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
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
        private ReactiveVariable<float> _glideSnapSpeed;
        private ReactiveVariable<float> _glideSnapDuration;
        private Rigidbody2D _rigidbody;

        private float _defaultGravityScale;
        private float _glideTimer;
        private bool _glideUsed;

        private float _glideActivationDelay;
        private const float GlideActivationDelayTime = 0.08f;

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
            _glideSnapSpeed = entity.GlideSnapSpeed;
            _glideSnapDuration = entity.GlideSnapDuration;
            _rigidbody = entity.Rigidbody;
            _canGlide = entity.CanGlide;
            _defaultGravityScale = _rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isGrounded.Value)
            {
                _glideUsed = false;
                _glideActivationDelay = 0f;

                if (_isGliding.Value)
                    StopGlide(applyBounce: false);

                return;
            }

            if (_isGliding.Value)
            {
                ApplyGlideDamping(deltaTime);

                if (_inputService.IsJumpKeyPressed)
                    StopGlide(applyBounce: true);

                return;
            }

            bool isFalling = _rigidbody.linearVelocity.y < _minFallVelocity.Value;

            if (_inputService.IsJumpKeyPressed && isFalling && !_glideUsed && _canGlide.Evaluate())
                _glideActivationDelay = GlideActivationDelayTime;

            if (_glideActivationDelay > 0f)
            {
                _glideActivationDelay -= deltaTime;

                if (_glideActivationDelay <= 0f && isFalling && !_glideUsed && _canGlide.Evaluate())
                    StartGlide();
            }
        }

        private void StartGlide()
        {
            _isGliding.Value = true;
            _glideUsed = true;
            _glideTimer = 0f;
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
            _glideTimer += deltaTime;

            float snapDuration = _glideSnapDuration.Value;
            float snapSpeed = _glideSnapSpeed.Value;
            float normalSpeed = _glideSpeedDamping.Value;

            float dampingSpeed = _glideTimer < snapDuration
                ? Mathf.Lerp(snapSpeed, normalSpeed, _glideTimer / snapDuration)
                : normalSpeed;

            float targetY = _glideMaxFallSpeed.Value;
            float currentY = _rigidbody.linearVelocity.y;
            float newY = Mathf.MoveTowards(currentY, targetY, dampingSpeed * deltaTime);

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, newY);
        }
    }
}