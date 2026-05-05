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
        private InputState _glideInput;

        private ICompositeCondition _canGlide;
        private ReactiveVariable<float> _glideHorizontalDrag;
        private ReactiveVariable<bool> _isGliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _minFallVelocity;
        private ReactiveVariable<float> _glideMaxFallSpeed;
        private ReactiveVariable<float> _glideSpeedDamping;
        private ReactiveVariable<float> _glideBounceForce;
        private ReactiveVariable<float> _glideSnapSpeed;
        private ReactiveVariable<float> _glideSnapDuration;
        private ReactiveVariable<float> _glideCounterMultiplier;
        private ReactiveVariable<int> _jumpsAvailable;

        private Rigidbody2D _rigidbody;

        private float _defaultGravityScale;
        private float _glideTimer;
        private bool _glideUsed;
        private float _holdTimer;
        private const float GlideHoldThreshold = 0.15f;

        public void OnInit(Entity entity)
        {
            _glideInput = entity.JumpInput;

            _isGliding = entity.IsGliding;
            _isGrounded = entity.IsGrounded;
            _minFallVelocity = entity.MinFallVelocityForAction;
            _glideMaxFallSpeed = entity.GlideMaxFallSpeed;
            _glideSpeedDamping = entity.GlideSpeedDamping;
            _glideBounceForce = entity.GlideBounceForce;
            _glideSnapSpeed = entity.GlideSnapSpeed;
            _glideSnapDuration = entity.GlideSnapDuration;
            _glideCounterMultiplier = entity.GlideCounterMultiplier;
            _canGlide = entity.CanGlide;
            _glideHorizontalDrag = entity.GlideHorizontalDrag;
            _jumpsAvailable = entity.JumpsAvailable;

            _rigidbody = entity.Rigidbody;
            _defaultGravityScale = _rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isGrounded.Value || _jumpsAvailable.Value > 0)
            {
                _glideUsed = false;
            }

            if (_isGrounded.Value)
            {
                _holdTimer = 0f;

                if (_isGliding.Value) 
                    StopGlide(applyBounce: false);

                return;
            }

            if (_isGliding.Value)
            {
                ApplyGlideDamping(deltaTime);

                if (_glideInput.IsPressed.Value) 
                    StopGlide(applyBounce: true);

                return;
            }

            bool isFallingFastEnough = _rigidbody.linearVelocity.y < _minFallVelocity.Value;

            if (_glideInput.IsHeld.Value && !_glideUsed && _canGlide.Evaluate())
            {
                _holdTimer += deltaTime;
                if (_holdTimer >= GlideHoldThreshold && isFallingFastEnough)
                {
                    StartGlide();
                    _holdTimer = 0f;
                }
            }
            else
            {
                _holdTimer = 0f;
            }
        }

        private void StartGlide()
        {
            _isGliding.Value = true;
            _glideUsed = true;
            _glideTimer = 0f;
            _rigidbody.gravityScale = 0f;

            float currentVerticalVelocity = _rigidbody.linearVelocity.y;
            float counterForce = Mathf.Abs(currentVerticalVelocity) * _glideCounterMultiplier.Value;

            _rigidbody.linearVelocity = new Vector2(
                _rigidbody.linearVelocity.x,
                currentVerticalVelocity + counterForce
            );
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

            float currentX = _rigidbody.linearVelocity.x;
            float newX = Mathf.MoveTowards(currentX, 0f, _glideHorizontalDrag.Value * deltaTime);

            _rigidbody.linearVelocity = new Vector2(newX, newY);
        }
    }
}