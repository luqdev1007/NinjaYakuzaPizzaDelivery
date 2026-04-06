using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class WallJumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private LayerMask _groundMask;

        private ReactiveVariable<bool> _isWallJumping;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _lockTimer;
        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;

        private WallJumpParams _params;

        public WallJumpSystem(IInputService inputService) => _inputService = inputService;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _groundMask = entity.GroundMask;

            _jumpsAvailable = entity.JumpsAvailable;
            _maxJumps = entity.MaxJumps;

            _isWallJumping = entity.IsWallJumping;
            _isGrounded = entity.IsGrounded;
            _lockTimer = entity.WallJumpLockTimer;
            _moveDirection = entity.MoveDirection;

            _params = entity.GetComponent<WallJumpParams>();
        }

        public void OnUpdate(float deltaTime)
        {
            // Работаем с таймером блокировки
            if (_lockTimer.Value > 0)
            {
                _lockTimer.Value -= deltaTime;
                return;
            }

            // На земле отскок невозможен
            if (_isGrounded.Value) return;

            // Проверяем наличие стены
            int wallDir = GetWallDirection();

            // Если есть стена и скорость падения/взлета подходит
            if (wallDir != 0 && Mathf.Abs(_rigidbody.linearVelocity.y) >= _params.MinVelocityY)
            {
                if (_inputService.IsJumpKeyPressed)
                {
                    PerformWallJump(wallDir);
                }
            }
        }

        private int GetWallDirection()
        {
            float checkDist = 0.6f; // Расстояние чуть больше половины ширины коллайдера

            // Луч вправо
            RaycastHit2D hitRight = Physics2D.Raycast(_transform.position, Vector2.right, checkDist, _groundMask);
            if (hitRight.collider != null) return 1;

            // Луч влево
            RaycastHit2D hitLeft = Physics2D.Raycast(_transform.position, Vector2.left, checkDist, _groundMask);
            if (hitLeft.collider != null) return -1;

            return 0;
        }

        private void PerformWallJump(int wallDir)
        {
            // _jumpsAvailable.Value = _maxJumps.Value; // max
            _jumpsAvailable.Value++; // +1

            // 1. Устанавливаем направление ВЗГЛЯДА в сторону отскока (от стены)
            // Это сработает в текущем кадре до того, как заблокируется canFlip
            _moveDirection.Value = new Vector2(-wallDir, _moveDirection.Value.y);

            // 2. Применяем физический импульс
            Vector2 force = new Vector2(-wallDir * _params.JumpForce.x, _params.JumpForce.y);
            _rigidbody.linearVelocity = force;

            // 3. Запускаем визуальный эффект (сальто)
            _isWallJumping.Value = true;
            _isWallJumping.Value = false; // Сбрасываем сразу, так как это триггер для View

            // 4. Включаем блокировку управления
            _lockTimer.Value = _params.ControlLockDuration;
        }
    }
}