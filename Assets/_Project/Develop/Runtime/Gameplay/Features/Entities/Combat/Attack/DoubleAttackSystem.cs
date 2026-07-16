using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.RandomManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using DG.Tweening;
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

        // Прок влияет на нанесённый урон, поэтому решение реплей-чувствительное и
        // берётся из засеянного потока. Раньше шло через глобальный статический
        // рандом на UnityEngine.Random — недетерминированный, из-за чего забег
        // было не воспроизвести. Теперь источник инъектируется и засеян на забег.
        private readonly IGameplayRandom _random;

        private const float ExtraAttackDelay = 0.1f;

        public DoubleAttackSystem(IGameplayRandom random)
        {
            _random = random;
        }

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

            // Формула эквивалентна прежней глобальной: Range(0, 100) <= percent,
            // тот же включающий float-максимум — семантика прока не изменилась,
            // изменился только источник рандома.
            if (IsChanceProceed(_procChance.Value) == false)
                return;

            DOVirtual.DelayedCall(ExtraAttackDelay, ExecuteExtraHit).SetUpdate(true);
        }

        private bool IsChanceProceed(float percent)
        {
            return _random.Range(0f, 100f) <= percent;
        }

        private void ExecuteExtraHit()
        {
            _startAttackEvent.Invoke();
            _currentCooldown.Value = _baseCooldown.Value;
            Debug.Log("Double attack!");
        }
    }
}
