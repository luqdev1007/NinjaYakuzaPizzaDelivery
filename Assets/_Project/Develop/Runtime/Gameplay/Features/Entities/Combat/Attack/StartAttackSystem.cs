using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private ReactiveVariable<bool> _intentAttack;
        private ICompositeCondition _canStartAttack;

        public void OnInit(Entity entity)
        {
            _intentAttack = entity.IntentAttack;
            _startAttackEvent = entity.StartAttackEvent;
            _inAttackProcess = entity.InAttackProcess;
            _canStartAttack = entity.CanStartAttack;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_intentAttack.Value == true && _canStartAttack.Evaluate())
            {
                _inAttackProcess.Value = true;
                _startAttackEvent.Invoke();
            }
        }
    }
}