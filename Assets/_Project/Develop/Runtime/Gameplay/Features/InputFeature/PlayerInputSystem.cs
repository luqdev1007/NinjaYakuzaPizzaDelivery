using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ReactiveVariable<Vector2> _intentMovement;
        private ReactiveVariable<bool> _intentJump;
        private ReactiveVariable<bool> _intentDash;
        private ReactiveVariable<bool> _intentSlide;
        private ReactiveVariable<bool> _intentAttack;

        public PlayerInputSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _intentMovement = entity.IntentMovement;
            _intentJump = entity.IntentJump;
            _intentDash = entity.IntentDash;
            _intentSlide = entity.IntentSlide;
            _intentAttack = entity.IntentAttack;
        }

        public void OnUpdate(float deltaTime)
        {
            _intentMovement.Value = _inputService.MoveDirection;

            _intentJump.Value = _inputService.IsJumpKeyHeld;

            _intentDash.Value = _inputService.IsDashKeyHeld;

            _intentAttack.Value = _inputService.IsAttackKeyHeld;

            _intentSlide.Value = _inputService.IsSlideKeyHeld;
        }
    }
}