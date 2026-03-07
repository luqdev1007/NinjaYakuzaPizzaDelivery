using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{

    public class WallHangSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ICompositeCondition _canWallHang;
        private ReactiveVariable<bool> _isWallHanging;
        private ReactiveVariable<float> _wallHangSlideSpeed;
        private ReactiveVariable<Vector2> _wallJumpForce;
        private ReactiveVariable<float> _wallDirection;
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;
        private LayerMask _wallHangLayer;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private float _defaultGravityScale;

        public WallHangSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _canWallHang = entity.CanWallHang;
            _isWallHanging = entity.IsWallHanging;
            _wallHangSlideSpeed = entity.WallHangSlideSpeed;
            _wallJumpForce = entity.WallJumpForce;
            _wallDirection = entity.WallDirection;
            _jumpsAvailable = entity.JumpsAvailable;
            _maxJumps = entity.MaxJumps;
            _wallHangLayer = entity.WallHangLayer;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _defaultGravityScale = _rigidbody.gravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isWallHanging.Value)
            {
                UpdateWallHang(deltaTime);
                return;
            }

            if (_inputService.IsAttackKeyHeld && _canWallHang.Evaluate())
                TryStartWallHang();
        }

        private void TryStartWallHang()
        {
            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            Vector2 checkOrigin = (Vector2)_transform.position + Vector2.right * direction * 0.3f;

            Collider2D hit = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);

            if (hit == null)
                return;

            _isWallHanging.Value = true;
            _wallDirection.Value = direction;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _jumpsAvailable.Value = _maxJumps.Value; // восстанавливаем прыжки
        }

        private void UpdateWallHang(float deltaTime)
        {
            // проверяем что стена всё ещё рядом
            float direction = _wallDirection.Value;
            Vector2 checkOrigin = (Vector2)_transform.position + Vector2.right * direction * 0.3f;
            Collider2D wallCheck = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);
            if (wallCheck == null)
            {
                StopWallHang();
                return;
            }

            // остальной код без изменений
            _rigidbody.linearVelocity = new Vector2(0f, -_wallHangSlideSpeed.Value);

            if (_inputService.IsJumpKeyPressed)
            {
                float bounceX = -_wallDirection.Value * _wallJumpForce.Value.x;
                float bounceY = _wallJumpForce.Value.y;
                _rigidbody.linearVelocity = new Vector2(bounceX, bounceY);
                StopWallHang();
                return;
            }

            if (!_inputService.IsAttackKeyHeld)
                StopWallHang();
        }

        private void StopWallHang()
        {
            _isWallHanging.Value = false;
            _rigidbody.gravityScale = _defaultGravityScale;
        }
    }
}
