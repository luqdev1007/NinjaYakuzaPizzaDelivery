using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class RandomMovementState : State, IUpdatableState
    {
        private readonly ReactiveVariable<Vector2> _movementDirection;
        private readonly float _cooldown;
        private float _timer;

        public RandomMovementState(Entity entity, float cooldown)
        {
            // _movementDirection = entity.MoveDirection;
            _cooldown = cooldown;
        }

        public override void Enter()
        {
            base.Enter();
            GenerateNewDirection();
            _timer = 0;
        }

        public override void Exit()
        {
            base.Exit();
            _movementDirection.Value = Vector2.zero;
        }

        public void Update(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _cooldown)
            {
                GenerateNewDirection();
                _timer = 0;
            }
        }

        private void GenerateNewDirection()
        {
            // Генерируем случайный угол для 2D пространства (XY)
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _movementDirection.Value = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}