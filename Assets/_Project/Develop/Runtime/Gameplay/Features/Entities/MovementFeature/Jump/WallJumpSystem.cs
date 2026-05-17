using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump
{
    public class WallJumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private ReactiveVariable<bool> _intentJump;
        private ReactiveVariable<bool> _isWallJumping;
        private ReactiveVariable<Vector2> _moveDirection;

        private LayerMask _wallMask;

        private ReactiveVariable<float> _jumpForceMin;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<Vector2> _wallJumpForceMultiplier;

        private ReactiveVariable<float> _lockoutDuration;

        private int _lastWallDir;
        private float _lastEntrySpeedX;

        private float _lockoutTimer;

        private bool _wasJumpIntendedLastFrame;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

            _intentJump = entity.IntentJump;

            _wallJumpForceMultiplier = entity.WallJumpForceMultiplier;

            _lockoutDuration = entity.LockoutDuration;

            _moveDirection = entity.MoveDirection;

            _isWallJumping = entity.IsWallJumping;

            _wallMask = entity.WallMask;

            _jumpForceMin = entity.JumpForceMin;
            _jumpForceMax = entity.JumpForceMax;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lockoutTimer > 0f)
            {
                _lockoutTimer -= Time.unscaledDeltaTime;

                if (_lockoutTimer <= 0f)
                {
                    _isWallJumping.Value = false; 
                }
            }

            int currentWallDir = GetWallDirection();
            UpdateEntrySpeed(currentWallDir);

            bool currentJumpIntent = _intentJump.Value;
            bool isJumpPressed = currentJumpIntent && !_wasJumpIntendedLastFrame;
            _wasJumpIntendedLastFrame = currentJumpIntent;

            if (isJumpPressed && currentWallDir != 0)
            {
                PerformWallJump(currentWallDir);
            }
        }

        private void UpdateEntrySpeed(int currentWallDir)
        {
            if (currentWallDir != 0)
            {
                _lastWallDir = currentWallDir;

                float currentSpeedX = Mathf.Abs(_rigidbody.linearVelocity.x);
                if (currentSpeedX > _lastEntrySpeedX && currentSpeedX > 0.1f)
                {
                    _lastEntrySpeedX = currentSpeedX;
                }
            }
            else if (_lockoutTimer <= 0f)
            {
                _lastEntrySpeedX = Mathf.MoveTowards(_lastEntrySpeedX, 0f, Time.deltaTime * 5f);
            }
        }

        private int GetWallDirection()
        {
            float checkDist = 0.6f;

            Vector2 rayOrigin = (Vector2)_transform.position + Vector2.up * 0.2f;

            if (Physics2D.Raycast(rayOrigin, Vector2.right, checkDist, _wallMask))
                return 1;

            if (Physics2D.Raycast(rayOrigin, Vector2.left, checkDist, _wallMask))
                return -1;

            return 0;
        }

        private void PerformWallJump(int wallDir)
        {
            _isWallJumping.Value = true;
            _lockoutTimer = _lockoutDuration.Value;

            _moveDirection.Value = new Vector2(-wallDir, _moveDirection.Value.y);

            float bounceForceX = Mathf.Max(_lastEntrySpeedX, _jumpForceMin.Value) * _wallJumpForceMultiplier.Value.x;
            bounceForceX = Mathf.Clamp(bounceForceX, _jumpForceMin.Value, _jumpForceMax.Value);

            float jumpForceY = _jumpForceMin.Value * _wallJumpForceMultiplier.Value.y;

            _rigidbody.linearVelocity = new Vector2(-wallDir * bounceForceX, jumpForceY);

            _lastEntrySpeedX = 0f;
        }
    }
}