using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class RotateToTargetState : State, IUpdatableState
    {
        private ReactiveVariable<float> _lookDirectionX;
        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<bool> _isTargetingActive; 

        private Transform _transform;

        public RotateToTargetState(Entity entity)
        {
            _lookDirectionX = entity.LookDirectionX;
            _currentTarget = entity.CurrentTarget;
            _isTargetingActive = entity.IsTargetingActive;
            _transform = entity.Transform;
        }

        public void Update(float deltaTime)
        {
            if (_isTargetingActive.Value && _currentTarget.Value != null)
            {
                if (_currentTarget.Value.Transform == null)
                    return;

                float differenceX = _currentTarget.Value.Transform.position.x - _transform.position.x;

                if (Mathf.Abs(differenceX) > 0.1f)
                {
                    _lookDirectionX.Value = Mathf.Sign(differenceX);
                }
            }
        }
    }
}