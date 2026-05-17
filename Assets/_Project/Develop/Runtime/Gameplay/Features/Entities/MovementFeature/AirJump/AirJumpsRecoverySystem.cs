using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump
{
    public class AirJumpsRecoverySystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _mustRestoreAirJumpsCount;

        private ReactiveVariable<int> _airJumpsCount;
        private ReactiveVariable<int> _airJumpsMaxCount;

        public void OnInit(Entity entity)
        {
            _mustRestoreAirJumpsCount = entity.MustRestoreAirJumpsCount;

            _airJumpsCount = entity.AirJumpsCount;
            _airJumpsMaxCount = entity.AirJumpsMaxCount;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mustRestoreAirJumpsCount.Evaluate())
            {
                _airJumpsCount.Value = _airJumpsMaxCount.Value;
                Debug.Log($"Air jumps restored: {_airJumpsCount.Value}/{_airJumpsMaxCount.Value}");
            }
        }
    }


}
