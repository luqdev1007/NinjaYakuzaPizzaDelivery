using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Extensions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using DG.Tweening;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class DoubleAttackSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<float> _procChance;
        private ReactiveVariable<float> _currentCooldown;
        private ReactiveVariable<float> _baseCooldown;
        private ICompositeCondition _canDoubleAttack;        

        private IDisposable _disposable;

        private const float ExtraAttackDelay = 0.1f;

        public void OnInit(Entity entity)
        {
            _startAttackEvent = entity.StartAttackEvent;
            _procChance = entity.DoubleAttackChance;
            _currentCooldown = entity.DoubleAttackCurrentCooldown;
            _baseCooldown = entity.DoubleAttackInitialCooldown;
            _canDoubleAttack = entity.CanDoubleAttack;

            _disposable =  entity.SuccessfulHitEvent.Subscribe(OnSuccesfulHit);
        }

        public void OnDispose() => _disposable?.Dispose();

        private void OnSuccesfulHit()
        {
            if (_canDoubleAttack.Evaluate() == false)
                return;

            if (GameRandom.IsChanceProceed(_procChance.Value) == false)
                return;

            DOVirtual.DelayedCall(ExtraAttackDelay, ExecuteExtraHit).SetUpdate(true);
        }

        private void ExecuteExtraHit()
        {
            _startAttackEvent.Invoke();
            _currentCooldown.Value = _baseCooldown.Value;
            Debug.Log("Double attack!");
        }
    }
}