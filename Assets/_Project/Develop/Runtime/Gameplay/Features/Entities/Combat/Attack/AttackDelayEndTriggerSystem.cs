using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackDelayEndTriggerSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _attackDelayEndEvent;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<float> _delay;
        private ReactiveVariable<float> _attackProcessCurrentTime;

        private IDisposable _timerDisposable;
        private IDisposable _startAttackDisposable;

        private bool _alreadyAttacked;

        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.AttackDelayEndEvent;
            _startAttackEvent = entity.StartAttackEvent;
            _delay = entity.AttackDelayTime;
            _attackProcessCurrentTime = entity.AttackProcessCurrentTime;

            _timerDisposable = _attackProcessCurrentTime.Subscribe(OnTimerChanged);
            _startAttackDisposable = _startAttackEvent.Subscribe(OnStartAttack);
        }

        private void OnTimerChanged(float old, float currentTime)
        {
            if (_alreadyAttacked) 
                return;

            if (currentTime >= _delay.Value)
            {
                _attackDelayEndEvent.Invoke();
                _alreadyAttacked = true;
            }
        }

        private void OnStartAttack()
        {
            _alreadyAttacked = false;
        }

        public void OnDispose()
        {
            _timerDisposable?.Dispose();
            _startAttackDisposable?.Dispose();
        }
    }
}