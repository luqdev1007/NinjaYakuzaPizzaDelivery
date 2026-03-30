using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    /// <summary>
    /// Расширение прыжка: если игрок прыгает находясь на склоне (IsOnSlope == true),
    /// к обычному вертикальному импульсу добавляется компонент вдоль нормали склона,
    /// умноженный на SlopeAccumSpeed. Чем быстрее игрок ехал по склону — тем дальше улетит.
    /// </summary>
    public class JumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ICompositeCondition _canJump;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;
        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<float> _jumpChargeTime;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private ReactiveVariable<Vector2> _slopeJumpForce;  // конфиг: базовый slope-импульс
        private ReactiveEvent _jumpEvent;
        private ReactiveEvent _doubleJumpEvent;
        private Rigidbody2D _rigidbody;
        private SlopeSystem _slopeSystem;

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
            _canJump = entity.CanJump;
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _jumpsAvailable = entity.JumpsAvailable;
            _maxJumps = entity.MaxJumps;
            _jumpForce = entity.JumpForce;
            _jumpForceMax = entity.JumpForceMax;
            _jumpChargeTime = entity.JumpChargeTime;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;
            _slopeJumpForce = entity.SlopeJumpForce;
            _jumpEvent = entity.JumpEvent;
            _doubleJumpEvent = entity.DoubleJumpEvent;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            // На земле восстанавливаем прыжки
            if (_isGrounded.Value || _isOnSlope.Value)
                _jumpsAvailable.Value = _maxJumps.Value;

            // Jump buffer
            if (_inputService.IsJumpKeyPressed)
                _jumpBufferTimer = JumpBufferTime;
            else
                _jumpBufferTimer -= deltaTime;

            // Начало зарядки
            if (_jumpBufferTimer > 0f && _canJump.Evaluate() && !_isCharging)
            {
                _isCharging = true;
                _chargeTimer = 0f;
                _jumpBufferTimer = 0f;
            }

            // Зарядка зажатой кнопкой
            if (_isCharging && _inputService.IsJumpKeyHeld)
            {
                _chargeTimer = Mathf.Min(
                    _chargeTimer + deltaTime,
                    _jumpChargeTime.Value);
            }

            // Выполнение прыжка при отпускании
            if (_isCharging && _inputService.IsJumpKeyReleased)
                ExecuteJump();
        }

        private void ExecuteJump()
        {
            float chargeRatio = _jumpChargeTime.Value > 0f
                ? _chargeTimer / _jumpChargeTime.Value
                : 1f;

            float verticalForce = Mathf.Lerp(
                _jumpForce.Value,
                _jumpForceMax.Value,
                chargeRatio);

            // Сбрасываем вертикальную скорость перед импульсом
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);

            if (_isOnSlope.Value && _slopeAccumSpeed.Value > 0.1f)
            {
                ExecuteSlopeJump(verticalForce);
            }
            else
            {
                _rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
            }

            bool isDoubleJump = !_isGrounded.Value && !_isOnSlope.Value
                                && _jumpsAvailable.Value < _maxJumps.Value;

            _jumpsAvailable.Value--;
            _isCharging = false;
            _chargeTimer = 0f;

            if (isDoubleJump)
                _doubleJumpEvent.Invoke();
            else
                _jumpEvent.Invoke();
        }

        /// <summary>
        /// Slope jump = стандартный вертикальный импульс
        ///            + компонент вдоль нормали склона × накопленная скорость.
        ///
        /// Это даёт эффект "вылета" с трамплина: чем быстрее ехал — тем дальше.
        /// SlopeJumpForce из конфига — базовый множитель направления (x=горизонталь, y=вертикаль).
        /// </summary>
        private void ExecuteSlopeJump(float verticalForce)
        {
            _rigidbody.linearVelocity = Vector2.zero;

            Vector2 slopeNormal = _slopeSystem != null ? _slopeSystem.SlopeNormal : Vector2.up;
            float accumSpeed = _slopeAccumSpeed.Value;

            // 1. Смягчаем влияние накопленной скорости коэффициентом (например, 0.4f)
            // Чтобы прыжок не рос бесконечно
            float speedBonus = accumSpeed * 0.4f;

            // 2. Рассчитываем импульс отталкивания от поверхности (нормаль)
            // Используем x из конфига как множитель "вылета" вбок
            Vector2 normalImpulse = slopeNormal * (speedBonus * _slopeJumpForce.Value.x);

            // 3. Вертикальный прыжок теперь получает лишь небольшую добавку от скорости
            // Используем y из конфига для контроля высоты
            float finalVerticalForce = verticalForce + (speedBonus * _slopeJumpForce.Value.y);
            Vector2 verticalImpulse = Vector2.up * finalVerticalForce;

            // Применяем итоговый вектор
            _rigidbody.AddForce(normalImpulse + verticalImpulse, ForceMode2D.Impulse);

            // Сбрасываем накопленное
            _slopeAccumSpeed.Value = 0f;
        }
    }
}