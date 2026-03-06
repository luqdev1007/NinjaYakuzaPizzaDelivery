using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class PlayerInputMovementState : State, IUpdatableState
    {
        private IInputService _inputService;

        private ReactiveVariable<Vector2> _movementDirection;

        public PlayerInputMovementState(Entity entity, IInputService inputService)
        {
            _inputService = inputService;

            _movementDirection = entity.MoveDirection;
        }

        public void Update(float deltaTime)
        {
            _movementDirection.Value = _inputService.MoveDirection;
        }

        public override void Exit()
        {
            base.Exit();

            _movementDirection.Value = Vector3.zero;
        }
    }
}
