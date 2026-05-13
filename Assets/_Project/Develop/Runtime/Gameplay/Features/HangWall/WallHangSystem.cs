using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{
    public class WallHangSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canWallHang;

        private LayerMask _wallHangLayer;

        private ReactiveVariable<bool> _isWallHanging;

        private ReactiveVariable<bool> _isGrappleThrowing;

        private ReactiveVariable<float> _wallDirection;
        private ReactiveVariable<Vector2> _wallJumpForce;
        private ReactiveVariable<float> _wallHangSlideSpeed;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _defaultGravityScale;

        private float _wallCoyoteTimer;
        private const float WallCoyoteTime = 0.15f;
        private float _jumpBufferTimer;
        private const float JumpBufferTime = 0.15f;

        public void OnInit(Entity entity)
        {
            /*
            _canWallHang = entity.CanWallHang;
            _isWallHanging = entity.IsWallHanging;
            _isGrappleThrowing = entity.IsThrowing;
            _wallHangSlideSpeed = entity.WallHangSlideSpeed;
            _wallJumpForce = entity.WallJumpForce;
            _wallDirection = entity.WallDirection;
            _jumpsAvailable = entity.MaxJumps;
            _maxJumps = entity.MaxJumps;
            _wallHangLayer = entity.WallHangLayer;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _defaultGravityScale = _rigidbody.gravityScale;

            _isGrappleThrowing.Subscribe((_, isThrowing) =>
            {
                if (isThrowing && _isWallHanging.Value)
                {
                    StopWallHang();
                }
            });
            */
        }

        public void OnUpdate(float deltaTime)
        {
            UpdateTimers(deltaTime);

            if (_isWallHanging.Value)
            {
                UpdateWallHang(deltaTime);
                return;
            }

            /*
            if (_inputService.IsAttackKeyHeld && _canWallHang.Evaluate())
            {
                TryStartWallHang();
            }

            if (_wallCoyoteTimer > 0 && _jumpBufferTimer > 0)
            {
                ExecuteWallJump();
            }
            */
        }

        private void UpdateTimers(float deltaTime)
        {
            /*
            if (_inputService.IsJumpKeyPressed)
                _jumpBufferTimer = JumpBufferTime;
            else
                _jumpBufferTimer -= deltaTime;

            if (_isWallHanging.Value)
                _wallCoyoteTimer = WallCoyoteTime;
            else
                _wallCoyoteTimer -= deltaTime;
            */
        }

        private void TryStartWallHang()
        {
            /*
            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            Vector2 checkOrigin = (Vector2)_transform.position + Vector2.right * direction * 0.3f;
            Collider2D hit = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);

            if (hit == null) return;

            _isWallHanging.Value = true;
            _wallDirection.Value = direction;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _jumpsAvailable.Value = _maxJumps.Value;
            */
        }

        private void UpdateWallHang(float deltaTime)
        {
            float direction = _wallDirection.Value;
            Vector2 checkOrigin = (Vector2)_transform.position + Vector2.right * direction * 0.3f;
            Collider2D wallCheck = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);

            if (wallCheck == null )// || !_inputService.IsAttackKeyHeld)
            {
                StopWallHang();
                return;
            }

            _rigidbody.linearVelocity = new Vector2(0f, -_wallHangSlideSpeed.Value);

            if (_jumpBufferTimer > 0)
            {
                ExecuteWallJump();
            }
        }

        private void ExecuteWallJump()
        {
            float bounceX = -_wallDirection.Value * _wallJumpForce.Value.x;
            float bounceY = _wallJumpForce.Value.y;
            _rigidbody.linearVelocity = new Vector2(bounceX, bounceY);

            _jumpBufferTimer = 0;
            _wallCoyoteTimer = 0;
            StopWallHang();
        }

        private void StopWallHang()
        {
            _isWallHanging.Value = false;
            if (!_isGrappleThrowing.Value)
            {
                _rigidbody.gravityScale = _defaultGravityScale;
            }
        }
    }
}