using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.RandomManagment;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class BrainsFactory
    {
        private readonly DIContainer _container;
        private readonly TimerServiceFactory _timerServiceFactory;
        private readonly AIBrainsContext _brainsContext;
        private readonly IInputService _inputService;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        // Геймплейный (засеянный) поток — уходит в решения о направлении движения.
        // Визуальный рандом призраков сюда НЕ ходит, он остаётся на глобальном
        // UnityEngine.Random.
        private readonly IGameplayRandom _gameplayRandom;

        public BrainsFactory(DIContainer container)
        {
            _container = container;
            _timerServiceFactory = _container.Resolve<TimerServiceFactory>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _inputService = _container.Resolve<IInputService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _gameplayRandom = _container.Resolve<IGameplayRandom>();
        }

        public StateMachineBrain CreateMainHeroBrain(Entity entity)
        {
            RotateToTargetState rotateState = new RotateToTargetState(entity);

            // AIStateMachine combatState = CreateAutoAttackStateMachine(entity);
            EmptyState idleState = new EmptyState();

            AIStateMachine behaviour = new AIStateMachine();
            behaviour.AddState(idleState);
            // behaviour.AddState(combatState);

            // behaviour.AddTransition(idleState, combatState, toCombatCondition);
            // behaviour.AddTransition(combatState, idleState, fromCombatCondition);

            // Теперь параллельно работает только поворот к цели (если она есть)
            AIParallelState parallelState = new AIParallelState(rotateState, behaviour);

            AIStateMachine rootStateMachine = new AIStateMachine();
            rootStateMachine.AddState(parallelState);

            StateMachineBrain brain = new StateMachineBrain(rootStateMachine);
            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        // Конфиг пробрасывается параметром — так же, как EntitiesFactory.CreateGhost
        // получает его от EnemiesFactory, у которой он уже на руках.
        public StateMachineBrain CreateGhostBrain(Entity entity, GhostConfig config)
        {
            AIStateMachine stateMachine = CreateRandomMovementStateMachine(entity, config);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        private AIStateMachine CreateRandomMovementStateMachine(Entity entity, GhostConfig config)
        {
            List<IDisposable> disposables = new List<IDisposable>();

            // Тайминги фаз были захардкожены (0.5f / 2f / 3f) — теперь из конфига.
            // DirectionChangeCooldown до этого лежал в конфиге мёртвым грузом.
            RandomMovementState randomMovementState = new RandomMovementState(
                entity,
                config.DirectionChangeCooldown,
                _gameplayRandom);
            EmptyState emptyState = new EmptyState();

            TimerService movementTimer = _timerServiceFactory.Create(config.MovementPhaseDuration);
            disposables.Add(movementTimer);
            disposables.Add(randomMovementState.Entered.Subscribe(movementTimer.Restart));

            TimerService idleTimer = _timerServiceFactory.Create(config.IdlePhaseDuration);
            disposables.Add(idleTimer);
            disposables.Add(emptyState.Entered.Subscribe(idleTimer.Restart));

            FuncCondition movementTimerEndedCondition = new FuncCondition(() => movementTimer.IsOver);
            FuncCondition idleTimerEndedCondition = new FuncCondition(() => idleTimer.IsOver);

            AIStateMachine stateMachine = new AIStateMachine(disposables);

            stateMachine.AddState(randomMovementState);
            stateMachine.AddState(emptyState);

            stateMachine.AddTransition(randomMovementState, emptyState, movementTimerEndedCondition);
            stateMachine.AddTransition(emptyState, randomMovementState, idleTimerEndedCondition);

            return stateMachine;
        }

        private AIStateMachine CreateAutoAttackStateMachine(Entity entity)
        {
            AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

            // ICondition canAttack = entity.CanStartAttack;
            // ReactiveVariable<bool> inAttackProcess = entity.InAttackProcess;

            // атакуем сразу если можем, возвращаемся ждать после атаки
            // ICondition toAttackCondition = new FuncCondition(() => canAttack.Evaluate());
            // ICondition fromAttackCondition = new FuncCondition(() => inAttackProcess.Value == false);

            // пустой стейт ожидания
            EmptyState waitState = new EmptyState();

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(waitState);
            stateMachine.AddState(attackTriggerState);

            // stateMachine.AddTransition(waitState, attackTriggerState, toAttackCondition);
            // stateMachine.AddTransition(attackTriggerState, waitState, fromAttackCondition);

            return stateMachine;
        }
    }
}
