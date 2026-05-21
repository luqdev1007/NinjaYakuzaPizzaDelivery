using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackProcessTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<bool> _inAttackProcess;
        private ReactiveEvent _startAttackEvent;

        private IDisposable _startAttackDisposable;

        public void OnInit(Entity entity)
        {
            _currentTime = entity.AttackProcessCurrentTime;
            _inAttackProcess = entity.InAttackProcess;
            _startAttackEvent = entity.StartAttackEvent;

            _startAttackDisposable = _startAttackEvent.Subscribe(OnStartAttackProcess);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inAttackProcess.Value)
            {
                _currentTime.Value += deltaTime;
            }
        }

        private void OnStartAttackProcess()
        {
            _currentTime.Value = 0f;
        }

        public void OnDispose()
        {
            _startAttackDisposable?.Dispose();
        }
    }
}