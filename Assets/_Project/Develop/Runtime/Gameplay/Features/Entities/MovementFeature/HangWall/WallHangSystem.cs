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
        private ReactiveVariable<float> _wallDirection;
        private ReactiveVariable<Vector2> _wallJumpForce;
        private ReactiveVariable<float> _wallHangSlideSpeed;
        private ReactiveVariable<float> _defaultGravityScale;

        private ReactiveVariable<bool> _intentAttack;
        private ReactiveVariable<bool> _intentJump;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        public void OnInit(Entity entity)
        {
            _canWallHang = entity.CanWallHang;

            _wallHangLayer = entity.WallHangLayer;

            _isWallHanging = entity.IsWallHanging;

            _wallDirection = entity.WallDirection;

            _wallHangSlideSpeed = entity.WallHangSlideSpeed;
            _wallJumpForce = entity.WallJumpForce;

            _intentAttack = entity.IntentAttack;
            _intentJump = entity.IntentJump;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

            _defaultGravityScale = entity.BaseGravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isWallHanging.Value)
            {
                UpdateWallHang();
                return;
            }

            if (_intentAttack.Value && _canWallHang.Evaluate())
            {
                TryStartWallHang();
            }
        }

        private void TryStartWallHang()
        {
            Vector2 forward = _transform.right;
            Vector2 checkOrigin = (Vector2)_transform.position + forward * 0.3f;
            Collider2D hit = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);

            if (hit == null)
                return;

            _isWallHanging.Value = true;
            _wallDirection.Value = forward.x > 0 ? 1f : -1f;

            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
        }

        private void UpdateWallHang()
        {
            Vector2 forward = _transform.right;
            Vector2 checkOrigin = (Vector2)_transform.position + forward * 0.3f;
            Collider2D wallCheck = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);

            if (wallCheck == null || !_intentAttack.Value)
            {
                StopWallHang();
                return;
            }

            _rigidbody.linearVelocity = new Vector2(0f, -_wallHangSlideSpeed.Value);

            if (_intentJump.Value)
            {
                ExecuteWallJump();
            }
        }

        private void ExecuteWallJump()
        {
            float bounceX = -_wallDirection.Value * _wallJumpForce.Value.x;
            float bounceY = _wallJumpForce.Value.y;

            _rigidbody.linearVelocity = new Vector2(bounceX, bounceY);

            StopWallHang();
        }

        private void StopWallHang()
        {
            _isWallHanging.Value = false;
            _rigidbody.gravityScale = _defaultGravityScale.Value;
        }
    }
}