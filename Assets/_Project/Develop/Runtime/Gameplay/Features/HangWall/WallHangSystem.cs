using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{
    public class WallHangSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly AudioService _audioService;

        private ICompositeCondition _canWallHang;
        private ReactiveVariable<bool> _isWallHanging;
        private ReactiveVariable<bool> _isGrappleThrowing;
        private ReactiveVariable<float> _wallHangSlideSpeed;
        private ReactiveVariable<Vector2> _wallJumpForce;
        private ReactiveVariable<float> _wallDirection;
        private ReactiveVariable<int> _jumpsAvailable;
        private ReactiveVariable<int> _maxJumps;
        private LayerMask _wallHangLayer;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private float _defaultGravityScale;

        public WallHangSystem(IInputService inputService, AudioService audioService)
        {
            _inputService = inputService;
            _audioService = audioService;
        }

        public void OnInit(Entity entity)
        {
            _canWallHang = entity.CanWallHang;
            _isWallHanging = entity.IsWallHanging;
            _isGrappleThrowing = entity.IsThrowing;
            _wallHangSlideSpeed = entity.WallHangSlideSpeed;
            _wallJumpForce = entity.WallJumpForce;
            _wallDirection = entity.WallDirection;
            _jumpsAvailable = entity.MaxJumps; // Уточни, какую переменную используешь для прыжков
            _maxJumps = entity.MaxJumps;
            _wallHangLayer = entity.WallHangLayer;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _defaultGravityScale = _rigidbody.gravityScale;

            // Если висим на стене, но игрок кидает хук — отменяем вис
            _isGrappleThrowing.Subscribe((_, isThrowing) =>
            {
                if (isThrowing && _isWallHanging.Value)
                {
                    StopWallHang();
                }
            });
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

            // Просто включаем вис. GrappleSystem сам увидит это и отключится.
            _isWallHanging.Value = true;
            _wallDirection.Value = direction;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = Vector2.zero;
            _jumpsAvailable.Value = _maxJumps.Value;
        }

        private void UpdateWallHang(float deltaTime)
        {
            float direction = _wallDirection.Value;
            Vector2 checkOrigin = (Vector2)_transform.position + Vector2.right * direction * 0.3f;
            Collider2D wallCheck = Physics2D.OverlapCircle(checkOrigin, 0.15f, _wallHangLayer);

            if (wallCheck == null)
            {
                StopWallHang();
                return;
            }

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

            // Важно: возвращаем гравитацию, ТОЛЬКО если мы не прервали вис броском хука
            if (!_isGrappleThrowing.Value)
            {
                _rigidbody.gravityScale = _defaultGravityScale;
            }
        }
    }
}