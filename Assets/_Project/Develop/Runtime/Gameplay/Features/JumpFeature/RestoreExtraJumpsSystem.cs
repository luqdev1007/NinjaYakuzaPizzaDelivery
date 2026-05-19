using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class RestoreExtraJumpsSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _mustRestoreExtraJumps;

        private ReactiveVariable<int> _maxExtraJumps;
        private ReactiveVariable<int> _extraJumpsAvailable;

        public void OnInit(Entity entity)
        {
            _maxExtraJumps = entity.MaxExtraJumps;
            _extraJumpsAvailable = entity.ExtraJumpsAvailable;

            _mustRestoreExtraJumps = entity.MustRestoreExtraJumps;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mustRestoreExtraJumps.Evaluate())
            {
                Debug.Log("Restoring Jumps");
                Debug.Log("Available Jumps was " + _extraJumpsAvailable.Value);
                _extraJumpsAvailable.Value = _maxExtraJumps.Value;
                Debug.Log("Available Jumps now is " + _extraJumpsAvailable.Value + ", which is the maximum");
            }
        }
    }
}