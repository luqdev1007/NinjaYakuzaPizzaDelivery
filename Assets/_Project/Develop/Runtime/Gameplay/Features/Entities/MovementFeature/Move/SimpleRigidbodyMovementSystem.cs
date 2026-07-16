using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move
{
    public class SimpleRigidbodyMovementSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        // Мёртвая зона для знака взгляда по X. Направление призрака — случайный вектор
        // с любого угла, поэтому при около-вертикальном полёте знак x скачет вокруг
        // нуля и фейсинг дёргался бы каждый кадр. Порог заметно крупнее геройских
        // 0.01 (RigidbodyMovementSystem) именно поэтому: у героя вход дискретный.
        private const float LookDirectionDeadzoneX = 0.1f;

        private Rigidbody2D _rigidbody;
        private ReactiveVariable<Vector2> _moveDirection;
        private ICompositeCondition _canMove;
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<bool> _isMoving;
        private ReactiveVariable<float> _lookDirectionX;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _isMoving = entity.IsMoving;
            _canMove = entity.CanMove;
            _lookDirectionX = entity.LookDirectionX;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false)
            {
                return;
            }

            Vector2 direction = _moveDirection.Value;

            if (direction.sqrMagnitude > 0.001f)
            {
                _rigidbody.linearVelocity = direction * _moveSpeed.Value;
                _isMoving.Value = true;

                // Единственный писатель LookDirectionX у призрака: до этого поле
                // выставлялось в 1 при создании и не трогалось никем, из-за чего
                // живая FlipDirectionSystem никогда не срабатывала. По аналогии с
                // RigidbodyMovementSystem героя пишем знак фактического движения.
                if (Mathf.Abs(direction.x) > LookDirectionDeadzoneX)
                {
                    _lookDirectionX.Value = Mathf.Sign(direction.x);
                }
            }
            else
            {
                _isMoving.Value = false;
            }
        }
    }
}
