using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.RandomManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        private readonly MainHeroHolderService _mainHeroHolderService;

        public BrainsFactory(DIContainer container)
        {
            _container = container;
            _timerServiceFactory = _container.Resolve<TimerServiceFactory>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _inputService = _container.Resolve<IInputService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _gameplayRandom = _container.Resolve<IGameplayRandom>();
            _mainHeroHolderService = _container.Resolve<MainHeroHolderService>();
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

        /// <summary>
        /// Мозг призрака-камикадзе: блуждание -> погоня -> взведение.
        /// </summary>
        /// <remarks>
        /// Конфиг типизирован AngryGhostConfig, но во вложенную машину блуждания
        /// уезжает как GhostConfig — наследование покрывает это без правки
        /// сигнатуры CreateRandomMovementStateMachine.
        ///
        /// ПОРЯДОК AddState ЗНАЧИМ: явного SetInitialState в StateMachine нет,
        /// стартовым становится _states[0] (см. StateMachine.Enter). Блуждание
        /// обязано быть первым.
        ///
        /// ПОРЯДОК AddTransition ЗНАЧИМ: приоритет = порядок вызова, срабатывает
        /// первый истинный переход (foreach + break в StateMachine.Update).
        /// Механизма AnyState в реализации нет, поэтому все пары заданы явно.
        ///
        /// Обратного перехода Chase -> Wander нет намеренно: агро необратимо.
        /// Помимо дизайна, такой переход был бы ещё и некорректен технически —
        /// StateMachine.Enter() не зовёт Enter() текущего состояния при повторном
        /// входе (проверка if (_currentState == null)), и машина блуждания
        /// возобновилась бы, не взведя фазовые таймеры.
        /// </remarks>
        public StateMachineBrain CreateAngryGhostBrain(Entity entity, AngryGhostConfig config)
        {
            AIStateMachine wanderStateMachine = CreateRandomMovementStateMachine(entity, config);
            ChaseTargetState chaseState = new ChaseTargetState(entity, _mainHeroHolderService);
            ArmingState armingState = new ArmingState(entity);

            AIStateMachine rootStateMachine = new AIStateMachine();

            rootStateMachine.AddState(wanderStateMachine);
            rootStateMachine.AddState(chaseState);
            rootStateMachine.AddState(armingState);

            Transform selfTransform = entity.Transform;
            ReactiveVariable<bool> isAgro = entity.IsAgro;
            ReactiveVariable<float> armingRadius = entity.ArmingRadius;
            ReactiveVariable<float> disarmRadius = entity.DisarmRadius;

            ICondition agroTriggeredCondition = new FuncCondition(() => isAgro.Value);

            // Сравниваем квадраты расстояний, без sqrt — образец
            // NearestDamagableTargetSelector.
            ICondition heroInArmingRangeCondition = new FuncCondition(() =>
                GetSqrDistanceToHeroFrom(selfTransform) <= armingRadius.Value * armingRadius.Value);

            ICondition heroOutOfDisarmRangeCondition = new FuncCondition(() =>
                GetSqrDistanceToHeroFrom(selfTransform) > disarmRadius.Value * disarmRadius.Value);

            rootStateMachine.AddTransition(wanderStateMachine, chaseState, agroTriggeredCondition);
            rootStateMachine.AddTransition(chaseState, armingState, heroInArmingRangeCondition);
            rootStateMachine.AddTransition(armingState, chaseState, heroOutOfDisarmRangeCondition);

            StateMachineBrain brain = new StateMachineBrain(rootStateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        /// <summary>
        /// Мозг наземного патрульного: одно состояние, ping-pong между двумя
        /// точками маршрута.
        /// </summary>
        /// <remarks>
        /// Конфиг сюда НЕ передаётся, в отличие от CreateGhostBrain. Всё, что
        /// нужно состоянию (точки, границы паузы, радиус достижения), уже лежит
        /// на сущности компонентами — их положила EntitiesFactory.CreateSlime.
        /// Тянуть конфиг вторым путём значило бы завести второй источник истины.
        ///
        /// Переходов нет вовсе: состояние одно. Место под состояния этапа 3
        /// (прицеливание / выстрел языком / притяг) остаётся чистым, и когда они
        /// появятся, порядок AddState/AddTransition придётся задавать здесь по
        /// тем же правилам, что описаны в CreateAngryGhostBrain.
        /// </remarks>
        public StateMachineBrain CreateSlimeBrain(Entity entity)
        {
            PatrolState patrolState = new PatrolState(entity, _gameplayRandom);

            AIStateMachine rootStateMachine = new AIStateMachine();
            rootStateMachine.AddState(patrolState);

            StateMachineBrain brain = new StateMachineBrain(rootStateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        /// <summary>
        /// Квадрат расстояния до героя. Героя нет или его Transform уже уничтожен —
        /// возвращает float.MaxValue, то есть «бесконечно далеко».
        /// </summary>
        /// <remarks>
        /// Такой fallback выбран не произвольно, он даёт корректное поведение на
        /// обоих переходах: взведение не начнётся (условие <= не выполнится), а
        /// уже начатое сорвётся обратно в погоню (условие > выполнится). Призрак,
        /// потерявший цель, не зависает взведённым.
        /// </remarks>
        private float GetSqrDistanceToHeroFrom(Transform selfTransform)
        {
            Entity mainHero = _mainHeroHolderService.MainHero;

            if (mainHero == null)
            {
                return float.MaxValue;
            }

            if (mainHero.Transform == null)
            {
                return float.MaxValue;
            }

            Vector2 offset = mainHero.Transform.position - selfTransform.position;

            return offset.sqrMagnitude;
        }

        private AIStateMachine CreateRandomMovementStateMachine(Entity entity, GhostConfig config)
        {
            List<IDisposable> disposables = new List<IDisposable>();

            // Тайминги фаз были захардкожены (0.5f / 2f / 3f) — теперь из конфига.
            // DirectionChangeCooldown до этого лежал в конфиге мёртвым грузом.
            //
            // ДЕСИНХРОН СТАИ. Все призраки спавнятся в одном кадре (синхронный foreach
            // в GameplayBootstrap и в ClearAllEnemiesStage) и входят в первую фазу
            // одновременно, а длительности брали идентичные из конфига — отсюда
            // «синхронные клоны». Лечится двумя независимыми вещами ниже: разбросом
            // длительностей и стартовым оффсетом первой фазы.
            //
            // Источник — ЗАСЕЯННЫЙ поток, а не глобальный Random: длительности фаз
            // определяют, когда призрак движется и где окажется, то есть это
            // реплей-чувствительный путь наравне с направлением. Десинхрон от этого не
            // страдает — разным призракам выпадают разные значения и из seeded-потока,
            // а забег остаётся воспроизводимым.
            RandomMovementState randomMovementState = new RandomMovementState(
                entity,
                Jitter(config.DirectionChangeCooldown, config.PhaseDurationJitter),
                _gameplayRandom);
            EmptyState emptyState = new EmptyState();

            TimerService movementTimer = _timerServiceFactory.Create(config.MovementPhaseDuration);
            disposables.Add(movementTimer);

            // Первая фаза движения стартует со случайного ОСТАТКА длительности — это и
            // есть стартовый фазовый оффсет: призрак входит в фазу так, будто она уже
            // частично прошла. Каждый следующий заход берёт свежую разбросанную
            // длительность, поэтому стая не сходится обратно со временем.
            bool isFirstMovementPhase = true;

            disposables.Add(randomMovementState.Entered.Subscribe(() =>
            {
                float duration = Jitter(config.MovementPhaseDuration, config.PhaseDurationJitter);

                if (isFirstMovementPhase)
                {
                    duration *= _gameplayRandom.Range(0f, 1f);
                    isFirstMovementPhase = false;
                }

                movementTimer.Restart(duration);
            }));

            TimerService idleTimer = _timerServiceFactory.Create(config.IdlePhaseDuration);
            disposables.Add(idleTimer);

            // Простою отдельный стартовый оффсет не нужен: первый заход сюда случится
            // уже после смещённой фазы движения.
            disposables.Add(emptyState.Entered.Subscribe(() =>
            {
                idleTimer.Restart(Jitter(config.IdlePhaseDuration, config.PhaseDurationJitter));
            }));

            FuncCondition movementTimerEndedCondition = new FuncCondition(() => movementTimer.IsOver);
            FuncCondition idleTimerEndedCondition = new FuncCondition(() => idleTimer.IsOver);

            AIStateMachine stateMachine = new AIStateMachine(disposables);

            stateMachine.AddState(randomMovementState);
            stateMachine.AddState(emptyState);

            stateMachine.AddTransition(randomMovementState, emptyState, movementTimerEndedCondition);
            stateMachine.AddTransition(emptyState, randomMovementState, idleTimerEndedCondition);

            return stateMachine;
        }

        /// <summary>
        /// Разброс значения на долю jitter: 0.25 => [0.75x, 1.25x]. jitter = 0
        /// возвращает исходное значение, то есть выключает десинхрон целиком.
        /// Тянет из засеянного потока — см. комментарий в CreateRandomMovementStateMachine.
        /// </summary>
        private float Jitter(float value, float jitter)
        {
            if (jitter <= 0f)
            {
                return value;
            }

            return value * _gameplayRandom.Range(1f - jitter, 1f + jitter);
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
