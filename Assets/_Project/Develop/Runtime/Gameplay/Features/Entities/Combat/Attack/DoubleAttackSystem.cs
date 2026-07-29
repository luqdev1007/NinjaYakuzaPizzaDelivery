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

        private const float MinChancePercent = 0f;
        private const float MaxChancePercent = 100f;

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

            if (IsChanceProceed(_procChance.Value) == false)
                return;

            DOVirtual.DelayedCall(ExtraAttackDelay, ExecuteExtraHit).SetUpdate(true);
        }

        /// <summary>
        /// Бросок шанса в процентах (0..100). Формула СЛОВО В СЛОВО повторяет
        /// ApplyDamageSystem.IsChanceProceed — оба шансовых стата обязаны
        /// читаться одинаково.
        ///
        /// Раньше здесь было Range(0, 100) &lt;= percent: включающий максимум в
        /// паре с &lt;= давал щель, при percent == 0 прок формально проходил,
        /// если выпадал ровно 0.0f. Пока базовый шанс был 45%, это был шум. Но
        /// теперь крит качается покупкой С НУЛЯ, и «не куплено» обязано означать
        /// НИКОГДА, без оговорок — иначе игрок изредка получает то, за что не
        /// платил, и это невозможно объяснить.
        ///
        /// Поэтому: 0 и 100 закорочены на константы, а в середине сравнение
        /// СТРОГОЕ (&lt;), так что верхняя граница диапазона рандома не может
        /// подарить лишний прок.
        /// </summary>
        private bool IsChanceProceed(float chancePercent)
        {
            if (chancePercent <= MinChancePercent)
                return false;

            if (chancePercent >= MaxChancePercent)
                return true;

            return _random.Range(MinChancePercent, MaxChancePercent) < chancePercent;
        }

        private void ExecuteExtraHit()
        {
            _startAttackEvent.Invoke();
            _currentCooldown.Value = _baseCooldown.Value;
            Debug.Log("Double attack!");
        }
    }
}
