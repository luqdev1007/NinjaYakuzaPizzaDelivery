using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class RecoilForce : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value;
    }

    public class ChargeSlashAttackRequiredTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ChargeSlashAttackCurrentTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class IsChargingSlashAttack : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class SpawnChargedSlashAtackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class CanChargeSlashAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class SpeedDamageDealtEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class SuccessfulHitEvent : IEntityComponent
    {
        public ReactiveEvent Value = new();
    }

    public class CanDoubleAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class DoubleAttackInitialCooldown: IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class DoubleAttackCurrentCooldown : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class DoubleAttackChance : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    /// <summary>
    /// Сколько заряженных слэшей осталось на забег.
    ///
    /// Рефилла на старте уровня НЕТ и не нужно: герой пересобирается фабрикой
    /// на каждый забег, поэтому счётчик и так рождается полным. Ровно по этой
    /// же причине рефил расходников живёт в InventorySystem.OnInit, а не в
    /// отдельной системе старта уровня.
    ///
    /// Свой компонент, а не слот в InventoryCharges: тот — список в лок-степе с
    /// массивом расходников, и вход туда затащил бы слэш в ротацию колёсиком,
    /// в инвентарный HUD и в TryUseActiveItem, из которых слэшу не нужно
    /// ничего. Инвентарь дал бы только хранение int.
    /// </summary>
    public class ChargedSlashCharges : IEntityComponent
    {
        public ReactiveVariable<int> Value = new();
    }

    public class AttackHitStopScale : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class AttackHitStopDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class AttackHitBounceForce : IEntityComponent
    {
        public ReactiveVariable<float> Value = new();
    }

    public class IsAttackInvulnerable : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class GroundHitBounceModifiers : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new();
    }

    public class AttackKnocback : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new();
    }

    

    public class AirHitBounceModifiers : IEntityComponent
    {
        public ReactiveVariable<Vector2> Value = new();
    }

    public class AttackHitMask : IEntityComponent
    {
        public ReactiveVariable<LayerMask> Value = new();
    }

    public class AttackInvulnerabilityDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackInvulnerabilityTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class StartAttackRequest : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class StartAttackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class CanStartAttack : IEntityComponent
    {
        public ICompositeCondition Value;
    }

    public class EndAttackEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class AttackProcessInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackProcessCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InAttackProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class AttackRange : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDelayTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackDelayEndEvent : IEntityComponent
    {
        public ReactiveEvent Value;
    }

    public class ShootPoint : IEntityComponent
    {
        public Transform Value;
    }

    public class AttackCooldownInitialTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class AttackCooldownCurrentTime : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class InAttackCooldown : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}

