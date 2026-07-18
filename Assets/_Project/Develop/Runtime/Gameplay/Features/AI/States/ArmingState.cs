using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    /// <summary>
    /// Взведение перед взрывом: призрак замирает и заводит таймер.
    /// </summary>
    /// <remarks>
    /// Update пуст намеренно — тик таймера живёт в ArmingTimerSystem на
    /// fixed-канале, рядом с остальными боевыми таймерами.
    ///
    /// Это состояние — единственный писатель IsArming: Enter поднимает флаг,
    /// Exit опускает. ArmingTimerSystem флаг только читает. Один флаг — один
    /// писатель, иначе выход из взведения по DisarmRadius разошёлся бы со
    /// сбросом таймера.
    ///
    /// Неподвижность обеспечивается не только обнулением MoveDirection: в
    /// EntitiesFactory.CreateAngryGhost условие CanMove доужесточается проверкой
    /// IsArming == false, поэтому SimpleRigidbodyMovementSystem во время
    /// взведения вообще не пишет скорость, а PhysicsStabilizationSystem гасит
    /// остаточную.
    /// </remarks>
    public class ArmingState : State, IUpdatableState
    {
        private readonly ReactiveVariable<bool> _isArming;
        private readonly ReactiveVariable<float> _armingTimer;
        private readonly ReactiveVariable<float> _armingDuration;
        private readonly ReactiveVariable<Vector2> _moveDirection;

        public ArmingState(Entity entity)
        {
            _isArming = entity.IsArming;
            _armingTimer = entity.ArmingTimer;
            _armingDuration = entity.ArmingDuration;
            _moveDirection = entity.MoveDirection;
        }

        public override void Enter()
        {
            base.Enter();

            _isArming.Value = true;
            _armingTimer.Value = _armingDuration.Value;
            _moveDirection.Value = Vector2.zero;
        }

        public override void Exit()
        {
            base.Exit();

            _isArming.Value = false;
        }

        public void Update(float deltaTime)
        {
        }
    }
}
