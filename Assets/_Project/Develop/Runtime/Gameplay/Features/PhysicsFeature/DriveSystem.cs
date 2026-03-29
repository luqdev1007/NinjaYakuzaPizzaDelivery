using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DriveSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Entity _entity;

        // Порог скорости, выше которого мы считаем, что начался "беспредел"
        private const float AnomalousVelocityThreshold = 25f;
        private const float MinUpwardVelocity = 0.5f;

        // Уменьшил время накопления, 4 сек - это вечность. Сделаем 1.5-2.
        private const float RequiredTimeInAir = 1.8f;
        private const float DriveDuration = 3.0f;

        private float _accumulationTimer;
        private float _driveTimer;
        private bool _isDriveActive;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDriveActive)
            {
                HandleDrive(deltaTime);
                return;
            }

            bool isInAir = _entity.IsGrounded.Value == false;
            // Проверяем не только движение вверх, но и аномально высокую горизонтальную скорость
            bool isChaotic = _rigidbody.linearVelocity.magnitude > AnomalousVelocityThreshold;
            bool isMovingUp = _rigidbody.linearVelocity.y > MinUpwardVelocity;

            if (isInAir && (isMovingUp || isChaotic))
            {
                // Если скорость зашкаливает, таймер копится в 2 раза быстрее (быстрый вход в драйв)
                float multiplier = isChaotic ? 2f : 1f;
                _accumulationTimer += deltaTime * multiplier;

                if (_accumulationTimer >= RequiredTimeInAir)
                {
                    StartDrive();
                }
            }
            else
            {
                // Медленно сбрасываем, если просто стоим на земле
                _accumulationTimer = Mathf.MoveTowards(_accumulationTimer, 0, deltaTime * 2f);
            }
        }

        private void StartDrive()
        {
            _isDriveActive = true;
            _driveTimer = DriveDuration;

            // Включаем неуязвимость на время Драйва (логично же!)
            if (_entity.IsAttackInvulnerable != null)
                _entity.IsAttackInvulnerable.Value = true;

            if (_entity.IsDriveActive != null)
                _entity.IsDriveActive.Value = true;

            // Гасим дикую инерцию, чтобы игрок не улетел за карту сразу после включения
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, AnomalousVelocityThreshold);

            Debug.Log("<color=orange>КРИТИЧЕСКАЯ ИНЕРЦИЯ! РЕЖИМ ДРАЙВА АКТИВИРОВАН</color>");
        }

        private void HandleDrive(float deltaTime)
        {
            _driveTimer -= deltaTime;

            // Во время драйва отключаем гравитацию полностью, чтобы он "плыл", но под контролем
            _rigidbody.gravityScale = 0f;

            // Добавляем микро-сопротивление воздуха, чтобы "бесоебство" затухало
            _rigidbody.linearDamping = 2f;

            if (_driveTimer <= 0)
            {
                EndDrive();
            }
        }

        private void EndDrive()
        {
            _isDriveActive = false;
            _accumulationTimer = 0;

            _rigidbody.linearDamping = 0f; // Возвращаем как было
            _rigidbody.gravityScale = 3f; // Твое стандартное значение гравитации

            // Важно: при выходе из драйва резко гасим скорость, чтобы персонаж не "застрял"
            _rigidbody.linearVelocity *= 0.1f;

            if (_entity.IsDriveActive != null)
                _entity.IsDriveActive.Value = false;

            if (_entity.IsAttackInvulnerable != null)
                _entity.IsAttackInvulnerable.Value = false;

            Debug.Log("<color=cyan>ПОТОК СТАБИЛИЗИРОВАН.</color>");
        }
    }
}