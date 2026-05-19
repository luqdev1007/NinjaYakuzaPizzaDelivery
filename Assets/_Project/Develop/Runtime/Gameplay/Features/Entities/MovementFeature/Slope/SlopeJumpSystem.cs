using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeJumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canSlopeJump;

        private ReactiveVariable<bool> _intentJump;

        private ReactiveVariable<bool> _isSliding;

        private ReactiveVariable<float> _baseSlopeJumpForce;
        private ReactiveVariable<Vector2> _jumpForceModifier;


        private ReactiveVariable<MovementStates> _movementState;

        private Rigidbody2D _rigidbody;

        private bool _wasJumpIntendedLastFrame;

        public void OnInit(Entity entity)
        {
            _canSlopeJump = entity.CanSlopeJump;

            _intentJump = entity.IntentJump;

            _isSliding = entity.IsSliding;

            _movementState = entity.CurrentMovementState;

            _baseSlopeJumpForce = entity.BaseSlopeJumpForce;
            _jumpForceModifier = entity.SlopeJumpForceModifier;

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            bool currentJumpIntent = _intentJump.Value;

            bool isJumpPressedThisFrame = currentJumpIntent && !_wasJumpIntendedLastFrame;
            _wasJumpIntendedLastFrame = currentJumpIntent;

            if (!isJumpPressedThisFrame)
                return;

            if (!_canSlopeJump.Evaluate())
                return;

            ExecuteSlopeJump();
        }

        private void ExecuteSlopeJump()
        {
            Debug.Log("Slope jump!");
            Vector2 currentVelocity = _rigidbody.linearVelocity;

            float targetX = currentVelocity.x * _jumpForceModifier.Value.x;
            float targetY = _baseSlopeJumpForce.Value + (Mathf.Abs(currentVelocity.x) * _jumpForceModifier.Value.y);

            _rigidbody.linearVelocity = new Vector2(targetX, targetY);

            _intentJump.Value = false;
            _isSliding.Value = false;
            // _movementState.Value = MovementStates.InAir;
        }
    }
}