using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private Entity _entity;

        private ICompositeCondition _canStartAttack;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;

        private float _chargeTimer;
        private bool _isCharging;
        private const float ChargeThreshold = 0.2f;

        private bool _isAttackBuffered;
        private const float BufferTimeThreshold = 0.15f;

        public StartAttackSystem( ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            /*
            _canStartAttack = entity.CanStartAttack;
            _startAttackEvent = entity.StartAttackEvent;
            _inAttackProcess = entity.InAttackProcess;
            */
        }

        public void OnUpdate(float deltaTime)
        {
          
           
        }
    }
}