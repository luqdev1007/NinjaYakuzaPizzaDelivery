using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    /// <summary>
    /// Погоня за героем. Пишет MoveDirection каждый тик, исполняет движение
    /// SimpleRigidbodyMovementSystem на fixed — та же схема, что у
    /// RandomMovementState.
    /// </summary>
    /// <remarks>
    /// Ссылка на героя НЕ кэшируется в конструкторе: мозг создаётся в
    /// EnemiesFactory на этапе спавна врагов, а герой к этому моменту может быть
    /// ещё не зарегистрирован в MainHeroHolderService. Читаем сервис каждый тик.
    ///
    /// Проверок две, и обе обязательны. MainHeroHolderService не обнуляет
    /// _mainHero после смерти героя — отписка от Added происходит на первом же
    /// найденном герое и ссылка живёт до конца сцены. При этом MonoEntitiesFactory
    /// уже уничтожил GameObject, поэтому Transform мёртв, хотя Entity не null.
    /// Образец обработки — NearestDamagableTargetSelector.GetSqrDistanceTo и
    /// RotateToTargetState.
    /// </remarks>
    public class ChaseTargetState : State, IUpdatableState
    {
        private readonly MainHeroHolderService _mainHeroHolderService;

        private readonly Transform _selfTransform;
        private readonly ReactiveVariable<Vector2> _moveDirection;
        private readonly ReactiveVariable<float> _moveSpeed;
        private readonly ReactiveVariable<float> _chaseSpeed;

        public ChaseTargetState(Entity entity, MainHeroHolderService mainHeroHolderService)
        {
            _mainHeroHolderService = mainHeroHolderService;

            _selfTransform = entity.Transform;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _chaseSpeed = entity.ChaseSpeed;
        }

        public override void Enter()
        {
            base.Enter();

            // Скорость погони подменяет блуждательную. Обратно её никто не
            // возвращает намеренно: агро необратимо, в блуждание призрак уже
            // не вернётся.
            _moveSpeed.Value = _chaseSpeed.Value;
        }

        public override void Exit()
        {
            base.Exit();

            _moveDirection.Value = Vector2.zero;
        }

        public void Update(float deltaTime)
        {
            Entity mainHero = _mainHeroHolderService.MainHero;

            if (mainHero == null)
            {
                return;
            }

            if (mainHero.Transform == null)
            {
                return;
            }

            Vector2 offset = mainHero.Transform.position - _selfTransform.position;

            _moveDirection.Value = offset.normalized;
        }
    }
}
