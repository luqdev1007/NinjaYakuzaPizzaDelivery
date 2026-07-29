using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class SlashAttackChargeSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _intentAttack;
        private ReactiveVariable<bool> _isCharging;
        private ReactiveEvent _spawnChargedSlashEvent;

        private ReactiveVariable<float> _chargeTimer;
        private ReactiveVariable<float> _requiredChargeTime;

        private ReactiveVariable<int> _charges;

        private ICompositeCondition _canCharge;

        public void OnInit(Entity entity)
        {
            _intentAttack = entity.IntentAttack;

            _isCharging = entity.IsChargingSlashAttack;
            _spawnChargedSlashEvent = entity.SpawnChargedSlashAtackEvent;

            _chargeTimer = entity.ChargeSlashAttackCurrentTimer;
            _requiredChargeTime = entity.ChargeSlashAttackRequiredTimer;
            _canCharge = entity.CanChargeSlashAttack;

            _charges = entity.ChargedSlashCharges;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_intentAttack.Value && _canCharge.Evaluate())
            {
                _isCharging.Value = true;
                _chargeTimer.Value += deltaTime;
            }
            else
            {
                if (_isCharging.Value)
                {
                    _isCharging.Value = false;

                    if (_intentAttack.Value == false && _chargeTimer.Value >= _requiredChargeTime.Value)
                    {
                        _spawnChargedSlashEvent.Invoke();

                        // Списываем ПОСЛЕ Invoke, а не до: Invoke синхронно
                        // доходит до SlashAttackSpawnSystem и создаёт снаряд,
                        // так что заряд снимается за состоявшийся выстрел.
                        // Само «хватает ли зарядов» проверено раньше — условием
                        // в CanChargeSlashAttack, без которого заряд бы даже не
                        // начался.
                        _charges.Value--;

                        Debug.Log("Charge Attack!");
                    }

                    _chargeTimer.Value = 0f;
                }
            }
        }
    }
}
