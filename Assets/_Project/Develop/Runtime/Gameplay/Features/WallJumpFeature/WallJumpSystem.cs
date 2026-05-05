using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class WallJumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private LayerMask _groundMask;

        private ReactiveVariable<bool> _isWallJumping;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _lockTimer;
        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<int> _jumpsAvailable;
        private InputState _jumpInput;

        private WallJumpParams _params;

        private float _wallCoyoteTimer;
        private int _lastWallDir;
        private float _lastEntrySpeedX;
        private const float WallCheckDistance = 0.6f;
        private const float CoyoteDuration = 0.15f;
        private const float MaxVelocityMultiplier = 2.2f;

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

            _jumpInput = entity.JumpInput;
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
                _wallCoyoteTimer = CoyoteDuration;

                float currentAbsVelX = Mathf.Abs(_rigidbody.linearVelocity.x);
                if (currentAbsVelX > 0.1f)
                {
                    _lastEntrySpeedX = currentAbsVelX;
                }
            }
            else if (_wallCoyoteTimer > 0)
            {
                _wallCoyoteTimer -= deltaTime;
            }

            if (_wallCoyoteTimer > 0 && _jumpInput.IsPressed.Value)
            {
                PerformWallJump(_lastWallDir);
            }
        }

        private int GetWallDirection()
        {
            if (Physics2D.Raycast(_transform.position, Vector2.right, WallCheckDistance, _groundMask)) return 1;
            if (Physics2D.Raycast(_transform.position, Vector2.left, WallCheckDistance, _groundMask)) return -1;
            return 0;
        }

        private void PerformWallJump(int wallDir)
        {
            _wallCoyoteTimer = 0;
            _jumpsAvailable.Value++;

            float bounceForceX = Mathf.Max(_lastEntrySpeedX, _params.JumpForce.x);
            float maxAllowedForceX = _params.JumpForce.x * MaxVelocityMultiplier;
            bounceForceX = Mathf.Clamp(bounceForceX, _params.JumpForce.x, maxAllowedForceX);

            _moveDirection.Value = new Vector2(-wallDir, _moveDirection.Value.y);
            _rigidbody.linearVelocity = new Vector2(-wallDir * bounceForceX, _params.JumpForce.y);

            _isWallJumping.Value = true;
            _lockTimer.Value = _params.ControlLockDuration;
            _lastEntrySpeedX = 0;
        }
    }
}