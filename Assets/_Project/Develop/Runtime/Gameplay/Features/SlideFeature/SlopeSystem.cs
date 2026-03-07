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
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private ReactiveVariable<Vector2> _slopeJumpForce;
        private LayerMask _slopeMask;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private bool _isOnSlope;
        private float _accumulatedSpeed;
        private const float MaxAccumulatedSpeed = 20f;
        private const float AccumulationRate = 8f;

        public SlopeSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _isSliding = entity.IsSliding;
            _slopeBoostMultiplier = entity.SlopeBoostMultiplier;
            _slopeJumpForce = entity.SlopeJumpForce;
            _slopeMask = entity.SlopeMask;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            _isOnSlope = CheckSlope(out float slopeAngle, out Vector2 slopeDirection);

            if (!_isOnSlope || !_isGrounded.Value)
            {
                if (_accumulatedSpeed > 0f)
                    _accumulatedSpeed = 0f;
                return;
            }

            // автоматически разгоняем по склону
            _accumulatedSpeed = Mathf.Min(
                _accumulatedSpeed + AccumulationRate * deltaTime,
                MaxAccumulatedSpeed);

            float boost = _accumulatedSpeed * _slopeBoostMultiplier.Value;
            _rigidbody.linearVelocity = new Vector2(
                slopeDirection.x * boost,
                _rigidbody.linearVelocity.y);

            // прыжок со склона — сила зависит от накопленной скорости
            if (_inputService.IsJumpKeyPressed)
            {
                float speedRatio = _accumulatedSpeed / MaxAccumulatedSpeed;
                float jumpX = slopeDirection.x * _slopeJumpForce.Value.x * (1f + speedRatio);
                float jumpY = _slopeJumpForce.Value.y * (1f + speedRatio * 0.5f);
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
                _rigidbody.AddForce(new Vector2(jumpX, jumpY), ForceMode2D.Impulse);
                _accumulatedSpeed = 0f;
            }
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

            // направление вдоль склона (вниз по наклону)
            slopeDirection = new Vector2(hit.normal.y, -hit.normal.x);

            // совпадаем с направлением взгляда героя
            if (_transform.localScale.x < 0)
                slopeDirection = -slopeDirection;

            return true;
        }
    }
}
