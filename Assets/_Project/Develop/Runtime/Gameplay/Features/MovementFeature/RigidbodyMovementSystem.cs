using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

public class RigidbodyMovementSystem : IInitializableSystem, IUpdatableSystem
{
    private ReactiveVariable<Vector2> _moveDirection;
    private ReactiveVariable<float> _moveSpeed;
    private ReactiveVariable<float> _moveSpeedMin;
    private ReactiveVariable<float> _acceleration;
    private ReactiveVariable<float> _deceleration;
    private Rigidbody2D _rigidbody;
    private ReactiveVariable<bool> _isMoving;
    private ICompositeCondition _canMove;

    private float _currentSpeedX; // текущая скорость с учётом разгона

    public void OnInit(Entity entity)
    {
        _moveDirection = entity.MoveDirection;
        _moveSpeed = entity.MoveSpeed;
        _moveSpeedMin = entity.MoveSpeedMin;
        _acceleration = entity.Acceleration;
        _deceleration = entity.Deceleration;
        _rigidbody = entity.Rigidbody;
        _canMove = entity.CanMove;
        _isMoving = entity.IsMoving;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_canMove.Evaluate() == false)
        {
            // торможение при запрете движения
            _currentSpeedX = Mathf.MoveTowards(
                _currentSpeedX, 0f, _deceleration.Value * deltaTime);
            _rigidbody.linearVelocity = new Vector2(
                _currentSpeedX, _rigidbody.linearVelocity.y);
            _isMoving.Value = Mathf.Abs(_currentSpeedX) > 0.01f;
            return;
        }

        float inputX = _moveDirection.Value.x;

        if (Mathf.Abs(inputX) > 0.01f)
        {
            // разгон в направлении инпута
            float targetSpeed = inputX > 0
                ? _moveSpeed.Value
                : -_moveSpeed.Value;

            // если меняем направление — сначала тормозим быстрее
            bool changingDirection = (_currentSpeedX > 0 && inputX < 0)
                || (_currentSpeedX < 0 && inputX > 0);

            float rate = changingDirection
                ? _deceleration.Value
                : _acceleration.Value;

            _currentSpeedX = Mathf.MoveTowards(
                _currentSpeedX, targetSpeed, rate * deltaTime);

            // минимальная скорость — чтобы не ползти в начале
            if (Mathf.Abs(_currentSpeedX) < _moveSpeedMin.Value)
                _currentSpeedX = _moveSpeedMin.Value * Mathf.Sign(inputX);
        }
        else
        {
            // инерция при отпускании
            _currentSpeedX = Mathf.MoveTowards(
                _currentSpeedX, 0f, _deceleration.Value * deltaTime);
        }

        _rigidbody.linearVelocity = new Vector2(
            _currentSpeedX, _rigidbody.linearVelocity.y);

        _isMoving.Value = Mathf.Abs(_currentSpeedX) > 0.01f;
    }
}