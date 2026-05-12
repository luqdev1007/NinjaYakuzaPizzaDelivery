using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canMove;

        private ReactiveVariable<bool> _isMoving;
        private ReactiveVariable<float> _lookDirectionX;

        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<float> _moveSpeedMin;

        private ReactiveVariable<Vector2> _intentMovement;

        private ReactiveVariable<float> _acceleration;
        private ReactiveVariable<float> _deceleration;

        private Rigidbody2D _rigidbody;
        private float _currentSpeedX;

        public void OnInit(Entity entity)
        {
            _canMove = entity.CanMove;
            _isMoving = entity.IsMoving;

            _intentMovement = entity.IntentMovement;
            _lookDirectionX = entity.LookDirectionX;

            _moveSpeed = entity.MoveSpeed;
            _moveSpeedMin = entity.MoveSpeedMin;
            _acceleration = entity.Acceleration;
            _deceleration = entity.Deceleration;
            _rigidbody = entity.Rigidbody;

            _currentSpeedX = _rigidbody.linearVelocity.x;
        }

        public void OnUpdate(float deltaTime)
        {
            if (Mathf.Abs(_rigidbody.linearVelocity.x - _currentSpeedX) > 0.5f)
            {
                _currentSpeedX = _rigidbody.linearVelocity.x;
            }

            if (_canMove.Evaluate() == false)
            {
                StopMovement(deltaTime);
                return;
            }

            float inputX = _intentMovement.Value.x;

            if (Mathf.Abs(inputX) > 0.01f)
            {
                ApplyMovement(inputX, deltaTime);
                _lookDirectionX.Value = Mathf.Sign(inputX);
            }
            else
            {
                StopMovement(deltaTime);
            }

            _rigidbody.linearVelocity = new Vector2(_currentSpeedX, _rigidbody.linearVelocity.y);

            _isMoving.Value = Mathf.Abs(_currentSpeedX) > 0.01f;
        }

        private void ApplyMovement(float inputX, float deltaTime)
        {
            float targetSpeed = inputX * _moveSpeed.Value;

            bool isReversing = (_currentSpeedX > 0 && inputX < 0) || (_currentSpeedX < 0 && inputX > 0);

            float rate = isReversing ? _deceleration.Value : _acceleration.Value;

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
    }
}