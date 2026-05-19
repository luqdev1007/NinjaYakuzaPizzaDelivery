using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class ExtraJumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canJump;

        private ReactiveVariable<bool> _intentJump;

        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;

        private ReactiveVariable<int> _maxExtraJumps;
        private ReactiveVariable<int> _extraJumpsAvailable;

        private ReactiveVariable<float> _airJumpMultiplier;

        private Rigidbody2D _rigidbody;

        // extras
        private bool _isCharging;
        private ReactiveVariable<float> _jumpChargeTime;
        private float _chargeTimer;
        private bool _wasJumpIntendedLastFrame;

        private float _jumpBufferTimer;
        private const float JumpBufferTime = 0.15f;

        public void OnInit(Entity entity)
        {
            _canJump = entity.CanJump;

            _intentJump = entity.IntentJump;

            _jumpForce = entity.JumpForce;
            _jumpForceMax = entity.JumpForceMax;

            _airJumpMultiplier = entity.AirJumpMultiplier;

                /*
            _maxExtraJumps = entity.MaxExtraJumps;
            _extraJumpsAvailable = entity.ExtraJumpsAvailable;
                */

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            bool isJumpIntented = _intentJump.Value;
            bool isJumpPressedDown = isJumpIntented && !_wasJumpIntendedLastFrame;
            bool isJumpReleased = !isJumpIntented && _wasJumpIntendedLastFrame;

            _wasJumpIntendedLastFrame = isJumpIntented;

            if (isJumpPressedDown)
                _jumpBufferTimer = JumpBufferTime;
            else
                _jumpBufferTimer -= deltaTime;

            Debug.Log("canJump is" + _canJump.Evaluate());

            if (_jumpBufferTimer > 0 && _canJump.Evaluate() && !_isCharging)
            {
                _isCharging = true;
                Debug.Log("charging is true");
                _chargeTimer = 0;
                _jumpBufferTimer = 0;
            }

            if (_isCharging && isJumpIntented)
            {
                _chargeTimer = Mathf.Min(_chargeTimer + deltaTime, _jumpChargeTime.Value);
                Debug.Log("charge timer is" + _chargeTimer);
            }

            if (_isCharging && isJumpReleased)
            {
                ExecuteJump();
            }
        }

        private void ExecuteJump()
        {
            float chargeRatio = _jumpChargeTime.Value > 0f ? _chargeTimer / _jumpChargeTime.Value : 1f;
            float verticalForce = Mathf.Lerp(_jumpForce.Value, _jumpForceMax.Value, chargeRatio);

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);

            _extraJumpsAvailable.Value--;

            _isCharging = false;
            Debug.Log("ExtraJumpExecuted");
        }
    }
}