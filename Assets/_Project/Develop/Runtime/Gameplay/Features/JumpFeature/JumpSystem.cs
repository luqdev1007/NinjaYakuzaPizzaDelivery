using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private InputState _jumpInput;
        private ReactiveEvent _doubleJumpEvent;
        private ReactiveEvent _jumpEvent;

        private ICompositeCondition _canJump;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;
        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<float> _jumpChargeTime;

        private Rigidbody2D _rigidbody;

        private float _chargeTimer;
        private float _jumpBufferTimer;
        private bool _isCharging;
        private const float JumpBufferTime = 0.15f;

        public void OnInit(Entity entity)
        {
            _jumpInput = entity.JumpInput;
            _doubleJumpEvent = entity.DoubleJumpEvent;
            _jumpEvent = entity.JumpEvent;

            _canJump = entity.CanJump;
            _isGrounded = entity.IsGrounded;
            _jumpsAvailable = entity.JumpsAvailable;
            _maxJumps = entity.MaxJumps;
            _jumpForce = entity.JumpForce;
            _jumpForceMax = entity.JumpForceMax;
            _jumpChargeTime = entity.JumpChargeTime;

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isGrounded.Value)
                _jumpsAvailable.Value = _maxJumps.Value;

            if (_jumpInput.IsPressed.Value)
                _jumpBufferTimer = JumpBufferTime;
            else
                _jumpBufferTimer -= deltaTime;

            if (_jumpBufferTimer > 0f && _canJump.Evaluate() && !_isCharging && _jumpsAvailable.Value > 0)
            {
                _isCharging = true;
                _chargeTimer = 0f;
                _jumpBufferTimer = 0f;
            }

            if (_isCharging && _jumpInput.IsHeld.Value)
            {
                _chargeTimer = Mathf.Min(_chargeTimer + deltaTime, _jumpChargeTime.Value);
            }

            if (_isCharging && _jumpInput.IsReleased.Value)
            {
                ExecuteJump();
            }
        }

        private void ExecuteJump()
        {
            float chargeRatio = _jumpChargeTime.Value > 0f ? _chargeTimer / _jumpChargeTime.Value : 1f;
            float verticalForce = Mathf.Lerp(_jumpForce.Value, _jumpForceMax.Value, chargeRatio);

            if (_jumpsAvailable.Value < _maxJumps.Value)
                _doubleJumpEvent.Invoke();
            else
                _jumpEvent.Invoke();

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);

            _jumpsAvailable.Value--;
            _isCharging = false;
            _chargeTimer = 0f;
        }
    }
}