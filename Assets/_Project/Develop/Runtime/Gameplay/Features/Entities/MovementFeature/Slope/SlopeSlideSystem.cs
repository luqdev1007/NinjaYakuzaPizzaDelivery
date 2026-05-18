using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeSlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<MovementStates> _movementState;

        private ReactiveVariable<bool> _intentSlide;

        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<Vector2> _slopeNormal;

        private ReactiveVariable<float> _baseSlideSpeed;
        private ReactiveVariable<float> _slideAcceleration;
        private ReactiveVariable<float> _maxSlideSpeed;    

        private Rigidbody2D _rigidbody;

        private float _currentSlideSpeed;

        public void OnInit(Entity entity)
        {
            _movementState = entity.CurrentMovementState;
            _isOnSlope = entity.IsOnSlope;
            _slopeNormal = entity.SlopeNormal;
            _intentSlide = entity.IntentSlide;

            _baseSlideSpeed = entity.SlopeBaseSlideSpeed;
            _slideAcceleration = entity.SlopeSlideAcceleration;
            _maxSlideSpeed = entity.SlopeMaxSlideSpeed;

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            MovementStates currentState = _movementState.Value;
            bool canStartSlide = _intentSlide.Value && _isOnSlope.Value;

            if (currentState == MovementStates.Default && canStartSlide)
            {
                _movementState.Value = MovementStates.Sliding;

                _currentSlideSpeed = Mathf.Max(Mathf.Abs(_rigidbody.linearVelocity.x), _baseSlideSpeed.Value);
            }

            if (_movementState.Value == MovementStates.Sliding)
            {
                if (!_isOnSlope.Value || !_intentSlide.Value)
                {
                    _movementState.Value = MovementStates.Default;
                    return;
                }

                Vector2 slopeTangent = new Vector2(_slopeNormal.Value.y, -_slopeNormal.Value.x).normalized;
                Vector2 downSlopeDirection = slopeTangent.y < 0 ? slopeTangent : -slopeTangent;

                _currentSlideSpeed = Mathf.MoveTowards(_currentSlideSpeed, _maxSlideSpeed.Value, _slideAcceleration.Value * deltaTime);

                _rigidbody.linearVelocity = downSlopeDirection * _currentSlideSpeed;
            }
        }
    }
}