using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly JumpSystem _jumpSystem;

        private ReactiveVariable<Vector2> _moveDirection;

        public PlayerInputSystem(IInputService inputService, JumpSystem jumpSystem)
        {
            _inputService = inputService;
            _jumpSystem = jumpSystem;
        }

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
        }

        public void OnUpdate(float deltaTime)
        {
            _moveDirection.Value = _inputService.MoveDirection;

            if (_inputService.IsJumpKeyPressed)
                _jumpSystem.TryJump();
        }
    }
}
