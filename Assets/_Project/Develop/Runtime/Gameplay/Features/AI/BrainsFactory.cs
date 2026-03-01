using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Timer;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class BrainsFactory
    {
        private readonly DIContainer _container;
        private readonly TimerServiceFactory _timerServiceFactory;
        private readonly AIBrainsContext _brainsContext;
        private readonly IInputService _inputService;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public BrainsFactory(DIContainer container)
        {
            _container = container;
            _timerServiceFactory = _container.Resolve<TimerServiceFactory>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _inputService = _container.Resolve<IInputService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        // examples
        /*
        private AIStateMachine CreateRandomMovementStateMachine(Entity entity)
        {
            List<IDisposable> disposables = new List<IDisposable>();

            RandomMovementState randomMovementState = new RandomMovementState(entity, 0.5f);
            EmptyState emptyState = new EmptyState();

            TimerService movementTimer = _timerServiceFactory.Create(0.5f);
            disposables.Add(movementTimer);
            disposables.Add(randomMovementState.Entered.Subscribe(movementTimer.Restart));

            TimerService idleTimer = _timerServiceFactory.Create(10f);
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
        */
        
        /*
        public StateMachineBrain CreateCaptainBrain(Entity entity)
        {
            AIStateMachine stateMachine = CreateRandomMovementStateMachine(entity);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }
        */
        
        /*
        public StateMachineBrain CreateWizardBrain(Entity entity, ITargetSelector targetSelector)
        {
            AIStateMachine combatState = CreateAutoAttackStateMachine(entity);

            EmptyState idleState = new EmptyState();

            ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

            ICompositeCondition fromIdleToCombatStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() => currentTarget.Value != null));

            ICompositeCondition fromCombatToIdleStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() => currentTarget.Value == null));

            AIStateMachine behaviour = new AIStateMachine();

            behaviour.AddState(idleState);
            behaviour.AddState(combatState);

            behaviour.AddTransition(idleState, combatState, fromIdleToCombatStateCondition);
            behaviour.AddTransition(combatState, idleState, fromCombatToIdleStateCondition);

            FindTargetState findTargetState = new FindTargetState(targetSelector, _entitiesLifeContext, entity);
            AIParallelState parallelState = new AIParallelState(findTargetState, behaviour);

            AIStateMachine rootStateMachine = new AIStateMachine();
            rootStateMachine.AddState(parallelState);

            StateMachineBrain brain = new StateMachineBrain(rootStateMachine);
            _brainsContext.SetFor(entity, brain);

            return brain;
        }
        */
        
        /*
        private AIStateMachine CreateAutoAttackStateMachine(Entity entity)
        {
            RotateToTargetState rotateToTargetState = new RotateToTargetState(entity);

            AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

            ICondition canStartAttack = entity.CanStartAttack;
            Transform transform = entity.Transform;
            ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

            ICompositeCondition fromRotateToAttackCondition = new CompositeCondition()
                .Add(canStartAttack)
                .Add(new FuncCondition(() =>
                {
                    Entity target = currentTarget.Value;

                    if (target == null || target.Transform == null)
                        return false;

                    Vector3 direction = target.Transform.position - transform.position;

                    direction.y = 0;

                    if (direction.sqrMagnitude < 0.001f)
                        return true;

                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);

                    float minThreshold = 5f;

                    return angleToTarget < minThreshold;
                }));

            ReactiveVariable<bool> inAttackProcess = entity.InAttackProcess;
            ICondition fromAttackToRotateStateCondition = new FuncCondition(() => inAttackProcess.Value == false);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(rotateToTargetState);
            stateMachine.AddState(attackTriggerState);

            stateMachine.AddTransition(rotateToTargetState, attackTriggerState, fromRotateToAttackCondition);
            stateMachine.AddTransition(attackTriggerState, rotateToTargetState, fromAttackToRotateStateCondition);

            return stateMachine;
        }
        */
        
        /*
        public StateMachineBrain CreateMoveToClosestTargetStateMachine(Entity entity)
        {
            FindTargetState findTargetState = new FindTargetState(new NearestDamagableTargetSelector(entity), _entitiesLifeContext, entity);
            MoveToClosestTargetState moveToTargetState = new MoveToClosestTargetState(entity);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(findTargetState);
            stateMachine.AddState(moveToTargetState);

            ICompositeCondition fromFindTargetToMoveToTargetStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentTarget.Value != null));

            ICompositeCondition fromMoveToTargetToFindTargetState = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentTarget.Value == null));


            stateMachine.AddTransition(findTargetState, moveToTargetState, fromFindTargetToMoveToTargetStateCondition);
            stateMachine.AddTransition(moveToTargetState, findTargetState, fromMoveToTargetToFindTargetState);

            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }
        */
        
        public void CreateEmptyBrain(Entity entity)
        {
            // Debug.Log("no brains for " + entity.Transform.gameObject.name + " yet");
        }
    }
}

