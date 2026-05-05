using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<float> _moveSpeedMin;
        private ReactiveVariable<float> _acceleration;
        private ReactiveVariable<float> _deceleration;
        private ReactiveVariable<Vector2> _inputDirection;
        private Rigidbody2D _rigidbody;
        private ReactiveVariable<bool> _isMoving;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<bool> _isSliding;
        private ICompositeCondition _canMove;

        private float _currentSpeedX;

        public void OnInit(Entity entity)
        {
            _moveSpeed = entity.MoveSpeed;
            _moveSpeedMin = entity.MoveSpeedMin;
            _acceleration = entity.Acceleration;
            _deceleration = entity.Deceleration;

            _inputDirection = entity.MoveDirectionInput;

            _rigidbody = entity.Rigidbody;
            _canMove = entity.CanMove;
            _isMoving = entity.IsMoving;

            _isOnSlope = entity.IsOnSlope;
            _isSliding = entity.IsSliding;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_isOnSlope.Value && _isSliding.Value) 
                return;

            _currentSpeedX = _rigidbody.linearVelocity.x;

            if (!_canMove.Evaluate())
            {
                StopMovement(deltaTime);
                ApplyVelocity();
                return;
            }

            float inputX = _inputDirection.Value.x;

            if (Mathf.Abs(inputX) > 0.01f)
            {
                ApplyAcceleration(inputX, deltaTime);
            }
            else
            {
                StopMovement(deltaTime);
            }

            ApplyVelocity();
        }

        private void ApplyAcceleration(float inputX, float deltaTime)
        {
            float targetSpeed = inputX * _moveSpeed.Value;

            bool changingDirection = (_currentSpeedX > 0.1f && inputX < -0.1f) || (_currentSpeedX < -0.1f && inputX > 0.1f);
            float rate = changingDirection ? _deceleration.Value : _acceleration.Value;

            _currentSpeedX = Mathf.MoveTowards(_currentSpeedX, targetSpeed, rate * deltaTime);

            if (Mathf.Abs(_currentSpeedX) < _moveSpeedMin.Value)
            {
                _currentSpeedX = _moveSpeedMin.Value * Mathf.Sign(inputX);
            }
        }

        private void StopMovement(float deltaTime)
        {
            _currentSpeedX = Mathf.MoveTowards(_currentSpeedX, 0f, _deceleration.Value * deltaTime);
        }

        private void ApplyVelocity()
        {
            _rigidbody.linearVelocity = new Vector2(_currentSpeedX, _rigidbody.linearVelocity.y);
            _isMoving.Value = Mathf.Abs(_currentSpeedX) > 0.01f;
        }
    }
}