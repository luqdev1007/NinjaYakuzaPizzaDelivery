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

        private WallJumpParams _params;

        private float _wallCoyoteTimer;
        private int _lastWallDir;
        private float _lastEntrySpeedX;

        public WallJumpSystem(IInputService inputService) => _inputService = inputService;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _groundMask = entity.GroundMask;

            _jumpsAvailable = entity.JumpsAvailable;
            _isWallJumping = entity.IsWallJumping;
            _isGrounded = entity.IsGrounded;
            _lockTimer = entity.WallJumpLockTimer;
            _moveDirection = entity.MoveDirection;

            _params = entity.GetComponent<WallJumpParams>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lockTimer.Value > 0)
            {
                _lockTimer.Value -= deltaTime;
                return;
            }

            if (_isGrounded.Value)
            {
                _wallCoyoteTimer = 0;
                return;
            }

            int currentWallDir = GetWallDirection();

            if (currentWallDir != 0)
            {
                _lastWallDir = currentWallDir;
                _wallCoyoteTimer = 0.15f;

                if (Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f)
                {
                    _lastEntrySpeedX = Mathf.Abs(_rigidbody.linearVelocity.x);
                }
            }
            else if (_wallCoyoteTimer > 0)
            {
                _wallCoyoteTimer -= deltaTime;
            }

            if (_wallCoyoteTimer > 0)
            {
                if (_inputService.IsJumpKeyPressed)
                {
                    PerformWallJump(_lastWallDir);
                }
            }
        }

        private int GetWallDirection()
        {
            float checkDist = 0.6f;

            if (Physics2D.Raycast(_transform.position, Vector2.right, checkDist, _groundMask)) return 1;
            if (Physics2D.Raycast(_transform.position, Vector2.left, checkDist, _groundMask)) return -1;

            return 0;
        }

        private void PerformWallJump(int wallDir)
        {
            _wallCoyoteTimer = 0;
            _jumpsAvailable.Value++;

            _moveDirection.Value = new Vector2(-wallDir, _moveDirection.Value.y);

            float bounceForceX = Mathf.Max(_lastEntrySpeedX, _params.JumpForce.x);

            Vector2 force = new Vector2(-wallDir * bounceForceX, _params.JumpForce.y);
            _rigidbody.linearVelocity = force;

            _isWallJumping.Value = true;
            _isWallJumping.Value = false;

            _lockTimer.Value = _params.ControlLockDuration;

            _lastEntrySpeedX = 0;
        }
    }
}