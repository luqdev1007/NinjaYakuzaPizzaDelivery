using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private ReactiveVariable<Vector2> _slopeJumpForce;
        private LayerMask _slopeMask;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _accumulatedSpeed;
        private bool _wasOnSlope;
        private Vector2 _lastSlopeDirection;

        private const float MaxAccumulatedSpeed = 20f;
        private const float AccumulationRate = 8f;
        private const float MinSpeedForJump = 1f;

        public SlopeSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _slopeBoostMultiplier = entity.SlopeBoostMultiplier;
            _slopeJumpForce = entity.SlopeJumpForce;
            _slopeMask = entity.SlopeMask;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            bool isOnSlope = CheckSlope(out float slopeAngle, out Vector2 slopeDirection);
            bool activeSlope = isOnSlope && _isGrounded.Value && slopeDirection.y < 0f;

            // склон закончился — автопрыжок
            if (_wasOnSlope && !activeSlope && _accumulatedSpeed > MinSpeedForJump)
            {
                PerformSlopeJump();
                _wasOnSlope = false;
                return;
            }

            if (!activeSlope)
            {
                _wasOnSlope = false;
                _accumulatedSpeed = 0f;
                return;
            }

            // накапливаем скорость
            _accumulatedSpeed = Mathf.Min(
                _accumulatedSpeed + AccumulationRate * deltaTime,
                MaxAccumulatedSpeed);

            _lastSlopeDirection = slopeDirection;
            _wasOnSlope = true;

            float boost = _accumulatedSpeed * _slopeBoostMultiplier.Value;
            _rigidbody.linearVelocity = new Vector2(
                slopeDirection.x * boost,
                _rigidbody.linearVelocity.y);

            // ручной прыжок со склона — усиленный
            if (_inputService.IsJumpKeyPressed)
            {
                PerformSlopeJump();
                _wasOnSlope = false;
            }
        }

        private void PerformSlopeJump()
        {
            float speedRatio = _accumulatedSpeed / MaxAccumulatedSpeed;
            float jumpX = _lastSlopeDirection.x * _slopeJumpForce.Value.x * (1f + speedRatio);
            float jumpY = _slopeJumpForce.Value.y * (1f + speedRatio * 0.5f);
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.AddForce(new Vector2(jumpX, jumpY), ForceMode2D.Impulse);
            _accumulatedSpeed = 0f;
        }

        private bool CheckSlope(out float angle, out Vector2 slopeDirection)
        {
            angle = 0f;
            slopeDirection = Vector2.right;

            RaycastHit2D hit = Physics2D.Raycast(
                _transform.position,
                Vector2.down,
                2f,
                _slopeMask);

            if (hit.collider == null)
                return false;

            angle = Vector2.Angle(hit.normal, Vector2.up);

            if (angle < 10f)
                return false;

            Vector2 down = new Vector2(hit.normal.y, -hit.normal.x);
            Vector2 up = new Vector2(-hit.normal.y, hit.normal.x);
            slopeDirection = down.y < 0 ? down : up;

            return true;
        }
    }
}