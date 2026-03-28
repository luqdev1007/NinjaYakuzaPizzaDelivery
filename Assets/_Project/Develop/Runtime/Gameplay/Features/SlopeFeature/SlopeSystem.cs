using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Transform _viewContainerTransform;
        private LayerMask _slopeMask;

        private float _accumulatedSpeed;
        private Vector2 _currentSlopeNormal;
        private bool _wasOnSlope;

        // Константы настройки
        private const float BaseSlidingBoost = 1.5f; // Множитель начального переноса скорости
        private const float GravityAcceleration = 12f; // Нарастание скорости под углом
        private const float MaxSlideSpeed = 25f;
        private const float JumpImpulseMultiplier = 1.2f; // Сила отскока

        public SlopeSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _slopeMask = entity.SlopeMask;
            _viewContainerTransform = entity.Transform.Find("ViewContainer");
        }

        public void OnUpdate(float deltaTime)
        {
            bool isOnSlope = CheckSlope(out float angle, out Vector2 slopeDirection, out Vector3 normal);
            bool canSlide = isOnSlope && _isGrounded.Value;

            if (canSlide)
            {
                HandleSliding(angle, slopeDirection, normal, deltaTime);
            }
            else if (_wasOnSlope)
            {
                ExitSlope();
            }
        }

        private void HandleSliding(float angle, Vector2 direction, Vector3 normal, float deltaTime)
        {
            _currentSlopeNormal = normal;

            if (!_wasOnSlope)
            {
                // ВХОД НА СКЛОН: Берем текущую горизонтальную скорость и превращаем в импульс скольжения
                float entrySpeed = Mathf.Abs(_rigidbody.linearVelocity.x);
                _accumulatedSpeed = Mathf.Max(entrySpeed * BaseSlidingBoost, 5f); // Минимум 5f для сочности
                _isOnSlope.Value = true;
                _wasOnSlope = true;
            }

            // НАРАСТАНИЕ: Скорость растет от гравитации (чем круче склон, тем быстрее)
            float gravityForce = Mathf.Sin(angle * Mathf.Deg2Rad);
            _accumulatedSpeed += gravityForce * GravityAcceleration * deltaTime;
            _accumulatedSpeed = Mathf.Clamp(_accumulatedSpeed, 0, MaxSlideSpeed);

            // ПРИМЕНЕНИЕ: Двигаем строго вдоль поверхности
            _rigidbody.linearVelocity = direction * _accumulatedSpeed;

            // ВИЗУАЛ: Наклон контейнера
            if (_viewContainerTransform != null)
            {
                float sign = direction.x > 0 ? 1f : -1f;
                _viewContainerTransform.localEulerAngles = new Vector3(0f, 0f, -angle * sign);
            }

            // ПРЫЖОК: Перпендикулярно поверхности
            if (_inputService.IsJumpKeyPressed)
            {
                ApplySlopeJump(normal);
            }
        }

        private void ApplySlopeJump(Vector2 normal)
        {
            // Прыгаем в сторону нормали (перпендикулярно) + сохраняем часть накопленной скорости
            Vector2 jumpDirection = (normal + Vector2.up * 0.5f).normalized;
            float jumpForce = _accumulatedSpeed * JumpImpulseMultiplier;

            _rigidbody.linearVelocity = jumpDirection * Mathf.Max(jumpForce, 12f);

            ExitSlope();
        }

        private void ExitSlope()
        {
            _isOnSlope.Value = false;
            _wasOnSlope = false;
            _accumulatedSpeed = 0f;

            if (_viewContainerTransform != null)
                _viewContainerTransform.localEulerAngles = Vector3.zero;
        }

        private bool CheckSlope(out float angle, out Vector2 slopeDirection, out Vector3 normal)
        {
            angle = 0f;
            slopeDirection = Vector2.right;
            normal = Vector2.up;

            RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, 2f, _slopeMask);

            if (hit.collider == null) return false;

            normal = hit.normal;
            angle = Vector2.Angle(normal, Vector2.up);

            if (angle < 10f) return false; // Слишком плоская поверхность

            // Вычисляем направление "вниз" вдоль поверхности
            Vector2 down = new Vector2(normal.y, -normal.x);
            Vector2 up = new Vector2(-normal.y, normal.x);
            slopeDirection = down.y < 0 ? down : up;

            return true;
        }
    }
}