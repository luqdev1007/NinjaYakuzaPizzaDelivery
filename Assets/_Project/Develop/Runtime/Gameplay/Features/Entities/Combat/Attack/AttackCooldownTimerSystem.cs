using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackCooldownTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<float> _initialTime;
        private ReactiveVariable<bool> _inAttackCooldown;
        private ReactiveEvent _endAttackEvent;

        private IDisposable _endAttackDisposable;

        public void OnInit(Entity entity)
        {
            _currentTime = entity.AttackCooldownCurrentTime;
            _initialTime = entity.AttackCooldownInitialTime;
            _inAttackCooldown = entity.InAttackCooldown;
            _endAttackEvent = entity.EndAttackEvent;

            _endAttackDisposable = _endAttackEvent.Subscribe(OnCooldownBegan);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inAttackCooldown.Value == false) 
                return;

            _currentTime.Value -= deltaTime;

            if (_currentTime.Value <= 0)
            {
                _inAttackCooldown.Value = false;
            }
        }

        private void OnCooldownBegan()
        {
            _currentTime.Value = _initialTime.Value;
            _inAttackCooldown.Value = true;
        }

        public void OnDispose()
        {
            _endAttackDisposable?.Dispose();
        }
    }
}