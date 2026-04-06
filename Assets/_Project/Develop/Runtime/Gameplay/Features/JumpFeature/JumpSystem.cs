using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature; // Добавлено
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly SlopeSystem _slopeSystem;
        private readonly CameraService _cameraService; // Добавлено

        private ReactiveEvent _doubleJumpEvent;
        private ReactiveEvent _jumpEvent;

        private ICompositeCondition _canJump;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<bool> _isDriveActive; // Добавлено
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;
        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<float> _jumpChargeTime;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private ReactiveVariable<Vector2> _slopeJumpForce;

        private Rigidbody2D _rigidbody;

        private float _chargeTimer;
        private float _jumpBufferTimer;
        private bool _isCharging;
        private const float JumpBufferTime = 0.15f;

        public JumpSystem(IInputService inputService, SlopeSystem slopeSystem, CameraService cameraService)
        {
            _inputService = inputService;
            _slopeSystem = slopeSystem;
            _cameraService = cameraService;
        }

        public void OnInit(Entity entity)
        {
            _doubleJumpEvent = entity.DoubleJumpEvent;
            _jumpEvent = entity.JumpEvent;
            _slopeJumpForce = entity.SlopeJumpForce;

            _canJump = entity.CanJump;
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _isDriveActive = entity.IsDriveActive; // Кэшируем состояние драйва

            _jumpsAvailable = entity.JumpsAvailable;
            _maxJumps = entity.MaxJumps;
            _jumpForce = entity.JumpForce;
            _jumpForceMax = entity.JumpForceMax;
            _jumpChargeTime = entity.JumpChargeTime;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isGrounded.Value || _isOnSlope.Value)
                _jumpsAvailable.Value = _maxJumps.Value;

            if (_inputService.IsJumpKeyPressed) _jumpBufferTimer = JumpBufferTime;
            else _jumpBufferTimer -= deltaTime;

            // Если мы в Драйве, зарядка не нужна — прыгаем мгновенно и мощно
            if (_jumpBufferTimer > 0f && _isDriveActive.Value)
            {
                _jumpBufferTimer = 0f;
                ExecuteJump();
                return;
            }

            if (_jumpBufferTimer > 0f && _canJump.Evaluate() && !_isCharging)
            {
                _isCharging = true;
                _chargeTimer = 0f;
                _jumpBufferTimer = 0f;
            }

            if (_isCharging && _inputService.IsJumpKeyHeld)
            {
                _chargeTimer = Mathf.Min(_chargeTimer + deltaTime, _jumpChargeTime.Value);
            }

            if (_isCharging && _inputService.IsJumpKeyReleased)
                ExecuteJump();
        }

        private void ExecuteJump()
        {
            float chargeRatio = _jumpChargeTime.Value > 0f ? _chargeTimer / _jumpChargeTime.Value : 1f;
            float verticalForce = Mathf.Lerp(_jumpForce.Value, _jumpForceMax.Value, chargeRatio);

            // МОДИФИКАТОР ДРАЙВА
            if (_isDriveActive.Value)
            {
                verticalForce *= 1.6f; // Усиливаем прыжок на 60%
                _cameraService.Shake(0.3f); // Сочный удар по камере
            }

            if (_jumpsAvailable.Value < _maxJumps.Value)
                _doubleJumpEvent.Invoke();
            else
                _jumpEvent.Invoke();

            if (_isOnSlope.Value && _slopeAccumSpeed.Value > 0.1f)
            {
                ExecuteSlopeJump(verticalForce);
            }
            else
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
                _rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
            }

            _jumpsAvailable.Value--;
            _isCharging = false;
        }

        private void ExecuteSlopeJump(float baseVerticalForce)
        {
            Vector2 slopeNormal = _slopeSystem.SlopeNormal;
            float accumSpeed = _slopeAccumSpeed.Value;
            Vector2 configForce = _slopeJumpForce.Value;

            float influence = Mathf.Clamp01(accumSpeed / 12f);
            Vector2 jumpDir = Vector2.Lerp(Vector2.up, slopeNormal, influence * 0.7f).normalized;

            float finalVerticalForce = baseVerticalForce + configForce.y + (accumSpeed * 0.5f);
            float finalHorizontalForce = configForce.x * (accumSpeed > 1f ? accumSpeed * 0.5f : 1f);

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.8f, 0f);
            Vector2 finalImpulse = new Vector2(jumpDir.x * finalHorizontalForce, jumpDir.y * finalVerticalForce);

            _rigidbody.AddForce(finalImpulse, ForceMode2D.Impulse);

            _slopeAccumSpeed.Value = 0f;
            _isOnSlope.Value = false;
        }
    }
}