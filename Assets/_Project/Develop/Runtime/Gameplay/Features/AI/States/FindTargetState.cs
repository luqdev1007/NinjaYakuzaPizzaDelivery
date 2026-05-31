using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class FindTargetState : State, IUpdatableState
    {
        private readonly ITargetSelector _targetSelector;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly ReactiveVariable<Entity> _currentTarget;

        private float _scanTimer;
        private const float ScanInterval = 0.2f; 

        public FindTargetState(
            ITargetSelector targetSelector,
            EntitiesLifeContext entitiesLifeContext,
            Entity entity)
        {
            _targetSelector = targetSelector;
            _entitiesLifeContext = entitiesLifeContext;
            // _currentTarget = entity.CurrentTarget;
        }

        public void Update(float deltaTime)
        {
            _scanTimer -= deltaTime;

            if (_scanTimer <= 0f)
            {
                _currentTarget.Value = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);
                _scanTimer = ScanInterval;
            }
        }
    }
}