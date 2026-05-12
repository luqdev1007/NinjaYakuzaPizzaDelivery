using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canJump;

        private ReactiveVariable<bool> _intentJump;

        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<float> _jumpChargeTime;

        private Rigidbody2D _rigidbody;

        private float _chargeTimer;
        private float _jumpBufferTimer;

        private bool _isCharging;
        private bool _wasJumpIntendedLastFrame;

        private const float JumpBufferTime = 0.15f;

        public void OnInit(Entity entity)
        {
            _canJump = entity.CanJump;

            _intentJump = entity.IntentJump;

            _jumpChargeTime = entity.JumpChargeTime;

            _jumpForce = entity.JumpForceMin;
            _jumpForceMax = entity.JumpForceMax;

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            bool currentIntent = _intentJump.Value;
            bool isJumpPressedDown = currentIntent && !_wasJumpIntendedLastFrame;
            bool isJumpReleased = !currentIntent && _wasJumpIntendedLastFrame;

            _wasJumpIntendedLastFrame = currentIntent;

            if (isJumpPressedDown)
                _jumpBufferTimer = JumpBufferTime;
            else
                _jumpBufferTimer -= deltaTime;

            if (_jumpBufferTimer > 0f && _canJump.Evaluate() && !_isCharging)
            {
                _isCharging = true;
                _chargeTimer = 0f;
                _jumpBufferTimer = 0f;
            }

            if (_isCharging)
            {
                if (!_canJump.Evaluate())
                {
                    _isCharging = false;
                    return;
                }

                if (currentIntent)
                {
                    _chargeTimer += deltaTime;

                    if (_chargeTimer >= _jumpChargeTime.Value)
                    {
                        ExecuteJump();
                    }
                }
                else if (isJumpReleased)
                {
                    ExecuteJump();
                }
            }
        }

        private void ExecuteJump()
        {
            float chargeRatio = _jumpChargeTime.Value > 0f ? _chargeTimer / _jumpChargeTime.Value : 1f;
            float verticalForce = Mathf.Lerp(_jumpForce.Value, _jumpForceMax.Value, chargeRatio);

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);

            _isCharging = false;
        }
    }
}