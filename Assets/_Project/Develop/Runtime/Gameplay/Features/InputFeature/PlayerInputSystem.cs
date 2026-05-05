using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private Entity _entity;

        public PlayerInputSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            _entity.MoveDirectionInput.Value = _inputService.MoveDirection;
            _entity.InventoryScrollDelta.Value = _inputService.MouseScrollDelta;

            bool isJumpHeld = _inputService.IsJumpKeyHeld;
            bool isGrounded = _entity.IsGrounded.Value;
            float verticalVelocity = _entity.Rigidbody.linearVelocity.y;

            var jump = _entity.JumpInput.IsPressed != null ? _entity.JumpInput : null;
            if (jump != null)
            {
                jump.IsPressed.Value = _inputService.IsJumpKeyPressed;
                jump.IsHeld.Value = isJumpHeld;
                jump.IsReleased.Value = _inputService.IsJumpKeyReleased;
            }

            _entity.GlideActive.Value = !isGrounded && verticalVelocity < 0.1f && isJumpHeld;

            var dash = _entity.DashInput.IsPressed != null ? _entity.DashInput : null;
            if (dash != null)
            {
                dash.IsPressed.Value = _inputService.IsDashKeyPressed;
                dash.IsHeld.Value = _inputService.IsDashKeyHeld;
                dash.IsReleased.Value = _inputService.IsDashKeyReleased;
            }

            if (isGrounded)
            {
                if (_inputService.IsSlideKeyPressed)
                    _entity.SlideRequest.Invoke();

                _entity.PlungeActive.Value = false;
            }
            else
            {
                _entity.PlungeActive.Value = _inputService.IsSlideKeyHeld;
            }

            if (_inputService.IsThrowKeyPressed)
                _entity.ThrowProjectileRequest.Invoke();

            _entity.GrapplingHookActive.Value = _inputService.IsGrappleKeyHeld;

            if (_inputService.IsUltimateKeyPressed)
                _entity.UltimateRequest.Invoke();

            _entity.ShowTargetActive.Value = _inputService.IsShowTargetKeyHeld;

            if (_inputService.IsAutoTargetTogglePressed)
                _entity.AutoTargetToggleRequest.Invoke();

            if (_inputService.IsCycleTargetPressed)
                _entity.CycleTargetRequest.Invoke();
        }
    }
}