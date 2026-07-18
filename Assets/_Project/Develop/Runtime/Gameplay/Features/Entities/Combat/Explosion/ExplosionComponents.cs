using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Explosion
{
    // — Агро —

    public class DetectionRadius : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ChaseSpeed : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    /// <summary>
    /// Агро необратимо по дизайну: выставляется один раз AgroDetectionSystem и
    /// больше не сбрасывается. Обратного перехода Chase -> Wander нет.
    /// </summary>
    public class IsAgro : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    // — Взведение —

    public class ArmingRadius : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class DisarmRadius : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ArmingDuration : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ArmingTimer : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    /// <summary>
    /// Single-writer — ArmingState: Enter выставляет true, Exit сбрасывает в false.
    /// ArmingTimerSystem только читает флаг и тикает таймер, сбросом не занимается.
    /// </summary>
    public class IsArming : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    // — Взрыв —

    public class ExplosionRadius : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ExplosionDamage : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ExplosionKnockbackForce : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class ForcedExplosionKnockbackMultiplier : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    /// <summary>
    /// Форма как у PlungeLandImpactHitMask, а не ReactiveVariable: маска поражения
    /// в рантайме не меняется, подписываться на неё некому.
    /// </summary>
    public class ExplosionLayerMask : IEntityComponent
    {
        public LayerMask Value;
    }

    /// <summary>
    /// Просьба подорваться. Шлют ArmingTimerSystem (Natural) и
    /// ForcedDetonationSystem (Forced), исполняет ExplosionSystem.
    /// </summary>
    public class DetonationRequest : IEntityComponent
    {
        public ReactiveEvent<DetonationKind> Value;
    }

    /// <summary>
    /// Факт состоявшегося взрыва — для вьюх (VFX, звук). Несёт тот же
    /// DetonationKind, чтобы визуал мог различать полноценный взрыв и вынужденный.
    /// </summary>
    public class DetonationEvent : IEntityComponent
    {
        public ReactiveEvent<DetonationKind> Value;
    }

    /// <summary>
    /// Защёлка против повторного взрыва. Пишет ExplosionSystem — до самоуничтожения,
    /// иначе цепочка CurrentHealth = 0 -> DeathSystem -> IsDead ->
    /// ForcedDetonationSystem подорвала бы призрака второй раз.
    /// </summary>
    public class HasDetonated : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }
}
