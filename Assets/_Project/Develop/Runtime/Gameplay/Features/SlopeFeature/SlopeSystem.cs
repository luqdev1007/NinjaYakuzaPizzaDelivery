using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private readonly IInputService _inputService;

        private Entity _entity;
        private Rigidbody2D _rigidbody;
        private Transform _viewContainerTransform;
        private LayerMask _slopeMask;
        private EntityCollisionProxy _collisionProxy;

        private float _accumulatedSpeed;
        private Vector2 _currentNormal;
        private bool _isContactingWithSlope;
        private float _originalGravityScale;

        // Константы
        private const float GravityForce = 35f;      // Насколько быстро разгоняемся вниз
        private const float MaxSlideSpeed = 30f;     // Предел скорости
        private const float MagnetForce = 8f;       // Прижим к поверхности
        private const float MinSlopeAngle = 15f;    // Минимальный угол (чтобы не скользить на ровном полу)

        public SlopeSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _rigidbody = entity.Rigidbody;
            _slopeMask = entity.SlopeMask;
            _viewContainerTransform = entity.Transform.Find("ViewContainer");

            _originalGravityScale = _rigidbody.gravityScale;

            // Находим наш мост коллизий на объекте
            _collisionProxy = _entity.Transform.GetComponent<EntityCollisionProxy>();
            if (_collisionProxy != null)
            {
                _collisionProxy.OnCollisionStayEvent += HandleCollision;
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isContactingWithSlope)
            {
                UpdateSliding(deltaTime);
            }
            else if (_entity.IsOnSlope.Value)
            {
                // Если флаг контакта пропал (например, улетели с трамплина), выходим
                ExitSlope();
            }

            // Сбрасываем каждый кадр. OnCollisionStay поднимет его до конца кадра, если контакт есть.
            _isContactingWithSlope = false;
        }

        private void HandleCollision(Collision2D collision)
        {
            // Проверяем, что слой объекта входит в SlopeMask
            if (((1 << collision.gameObject.layer) & _slopeMask) == 0) return;

            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            if (angle >= MinSlopeAngle && angle < 89f)
            {
                _currentNormal = contact.normal;
                _isContactingWithSlope = true;

                if (!_entity.IsOnSlope.Value)
                    EnterSlope();
            }
        }

        private void EnterSlope()
        {
            _entity.IsOnSlope.Value = true;

            // Подхватываем текущую инерцию игрока, чтобы не было рывка
            float entrySpeed = _rigidbody.linearVelocity.magnitude;
            _accumulatedSpeed = Mathf.Max(entrySpeed, 12f);

            _rigidbody.gravityScale = 0f; // Отключаем стандартную гравитацию
        }

        private void UpdateSliding(float deltaTime)
        {
            // 1. Направление движения (перпендикуляр к нормали, смотрящий вниз)
            Vector2 slideDir = new Vector2(_currentNormal.y, -_currentNormal.x);
            if (slideDir.y > 0) slideDir = -slideDir;

            // 2. Ускорение (сила гравитации зависит от крутизны склона)
            float slopeSteepness = 1f - Vector2.Dot(_currentNormal, Vector2.up);
            _accumulatedSpeed += GravityForce * slopeSteepness * deltaTime;
            _accumulatedSpeed = Mathf.Min(_accumulatedSpeed, MaxSlideSpeed);

            // 3. Магнитизм (прижимаем игрока к склону, чтобы не "скакал")
            Vector2 magnet = -_currentNormal * MagnetForce;

            // Применяем скорость напрямую
            _rigidbody.linearVelocity = (slideDir * _accumulatedSpeed) + magnet;

            // 4. Визуал (поворот персонажа параллельно склону)
            if (_viewContainerTransform != null)
            {
                float angle = Vector2.SignedAngle(Vector2.up, _currentNormal);
                _viewContainerTransform.localEulerAngles = new Vector3(0, 0, angle);
            }

            // 5. Прыжок (единственное доступное действие)
            if (_inputService.IsJumpKeyPressed)
            {
                ApplySlopeJump();
            }
        }

        private void ApplySlopeJump()
        {
            // 1. Вектор "выплеска" - это нормаль (перпендикуляр) 
            // Мы можем смешать его с направлением взгляда, если нужно
            Vector2 jumpNormal = _currentNormal;

            // 2. Направление, куда мы катились (для сохранения инерции)
            Vector2 slideDir = new Vector2(_currentNormal.y, -_currentNormal.x);
            if (slideDir.y > 0) slideDir = -slideDir;

            // 3. Формула выплеска: Сила из конфига + накопленная скорость
            // SlopeJumpForce (Vector2) из конфига позволит тебе настроить 
            // насколько сильно прыжок подбрасывает вверх относительно нормали
            Vector2 configForce = _entity.SlopeJumpForce.Value;

            // Итоговый вектор скорости
            Vector2 finalVelocity = (jumpNormal * configForce.y) + (slideDir * _accumulatedSpeed * _entity.SlopeBoostMultiplier.Value);

            _rigidbody.linearVelocity = finalVelocity;

            // Вызываем событие прыжка, чтобы сработала анимация/звук
            _entity.JumpEvent.Invoke();

            ExitSlope();
        }

        private void ExitSlope()
        {
            _entity.IsOnSlope.Value = false;
            _rigidbody.gravityScale = _originalGravityScale;
            _accumulatedSpeed = 0f;

            if (_viewContainerTransform != null)
                _viewContainerTransform.localEulerAngles = Vector3.zero;
        }

        public void OnDispose()
        {
            if (_collisionProxy != null)
                _collisionProxy.OnCollisionStayEvent -= HandleCollision;
        }
    }
}