using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private ReactiveVariable<Vector2> _slopeJumpForce;
        private LayerMask _slopeMask;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _accumulatedSpeed;
        private bool _wasOnSlope;
        private bool _isJumping;
        private Vector2 _lastSlopeDirection;

        private const float MaxAccumulatedSpeed = 20f;
        private const float AccumulationRate = 8f;
        private const float MinSpeedForJump = 1f;
        private const float JumpDuration = 0.15f;

        public SlopeSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
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
            if (_isJumping)
                return;

            bool isOnSlope = CheckSlope(out float slopeAngle, out Vector2 slopeDirection);
            bool activeSlope = isOnSlope && _isGrounded.Value && slopeDirection.y < 0f;

            if (_wasOnSlope && !activeSlope && _accumulatedSpeed > MinSpeedForJump)
            {
                _isSliding.Value = false;
                _coroutinesPerformer.StartPerform(SlopeJumpCoroutine());
                _wasOnSlope = false;
                return;
            }

            if (!activeSlope)
            {
                if (_wasOnSlope)
                    _isSliding.Value = false;
                _wasOnSlope = false;
                _accumulatedSpeed = 0f;
                return;
            }

            if (!_isSliding.Value)
                _isSliding.Value = true;

            _accumulatedSpeed = Mathf.Min(
                _accumulatedSpeed + AccumulationRate * deltaTime,
                MaxAccumulatedSpeed);

            _lastSlopeDirection = slopeDirection;
            _wasOnSlope = true;

            float boost = _accumulatedSpeed * _slopeBoostMultiplier.Value;
            _rigidbody.linearVelocity = new Vector2(
                slopeDirection.x * boost,
                _rigidbody.linearVelocity.y);

            if (_inputService.IsJumpKeyPressed)
            {
                _isSliding.Value = false;
                _coroutinesPerformer.StartPerform(SlopeJumpCoroutine());
                _wasOnSlope = false;
            }
        }

        private IEnumerator SlopeJumpCoroutine()
        {
            _isJumping = true;

            float speedRatio = _accumulatedSpeed / MaxAccumulatedSpeed;
            float targetX = _lastSlopeDirection.x * _slopeJumpForce.Value.x * (1f + speedRatio * 3f);
            float targetY = _slopeJumpForce.Value.y * (1f + speedRatio);

            float elapsed = 0f;
            Vector2 startVelocity = _rigidbody.linearVelocity;

            while (elapsed < JumpDuration)
            {
                float t = elapsed / JumpDuration;
                _rigidbody.linearVelocity = Vector2.Lerp(
                    startVelocity,
                    new Vector2(targetX, targetY),
                    t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(targetX, targetY);
            _accumulatedSpeed = 0f;
            _isJumping = false;
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