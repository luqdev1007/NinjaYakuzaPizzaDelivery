using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class JumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly SlopeSystem _slopeSystem;

        private ReactiveEvent _doubleJumpEvent;
        private ReactiveEvent _jumpEvent;

        private ICompositeCondition _canJump;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;
        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<float> _jumpChargeTime;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private Rigidbody2D _rigidbody;

        private float _chargeTimer;
        private float _jumpBufferTimer;
        private bool _isCharging;
        private const float JumpBufferTime = 0.15f;

        public JumpSystem(IInputService inputService, SlopeSystem slopeSystem)
        {
            _inputService = inputService;
            _slopeSystem = slopeSystem;
        }

        public void OnInit(Entity entity)
        {
            _doubleJumpEvent = entity.DoubleJumpEvent;
            _jumpEvent = entity.JumpEvent;

            _canJump = entity.CanJump;
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
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

            // Логика определения: обычный это прыжок или двойной
            if (_jumpsAvailable.Value < _maxJumps.Value)
                _doubleJumpEvent.Invoke(); // Вызываем событие двойного прыжка
            else
                _jumpEvent.Invoke(); // Вызываем событие обычного прыжка

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

            // ЛОГИКА ТРАМПЛИНА: 
            // Смешиваем "Вверх" и "Нормаль склона" в зависимости от скорости
            float influence = Mathf.Clamp01(accumSpeed / 12f);
            Vector2 jumpDir = Vector2.Lerp(Vector2.up, slopeNormal, influence * 0.7f).normalized;

            // Итоговая сила прыжка
            float totalForce = baseVerticalForce + (accumSpeed * 0.6f);

            // Мягкое обнуление скорости для чистого вылета
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.5f, 0f);

            _rigidbody.AddForce(jumpDir * totalForce, ForceMode2D.Impulse);
            _slopeAccumSpeed.Value = 0f;
        }
    }
}