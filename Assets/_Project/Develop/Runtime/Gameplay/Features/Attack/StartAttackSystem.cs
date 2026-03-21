using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private ICompositeCondition _canStartAttack;

        public StartAttackSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnInit(Entity entity)
        {
            _startAttackEvent = entity.StartAttackEvent;
            _inAttackProcess = entity.InAttackProcess;
            _canStartAttack = entity.CanStartAttack;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inputService.IsAttackKeyPressed && _canStartAttack.Evaluate())
            {
                _inAttackProcess.Value = true;
                _startAttackEvent.Invoke();
            }
        }
    }
}