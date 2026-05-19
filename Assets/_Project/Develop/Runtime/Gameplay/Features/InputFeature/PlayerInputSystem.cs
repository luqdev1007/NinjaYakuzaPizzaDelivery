using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using System;
namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<bool> _intentJump;
        private ReactiveVariable<bool> _intentDash;

        private ReactiveEvent _slideRequest;

        public PlayerInputSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
            _intentJump = entity.IntentJump;
            _slideRequest = entity.SlideRequest;
            _intentDash = entity.IntentDash;
        }

        public void OnUpdate(float deltaTime)
        {
            _moveDirection.Value = _inputService.MoveDirection;

            _intentJump.Value = _inputService.IsJumpKeyHeld;
            _intentDash.Value = _inputService.IsDashKeyHeld;


            if (_inputService.IsSlideKeyPressed)
            {
                _slideRequest.Invoke();
            }
        }
    }
}