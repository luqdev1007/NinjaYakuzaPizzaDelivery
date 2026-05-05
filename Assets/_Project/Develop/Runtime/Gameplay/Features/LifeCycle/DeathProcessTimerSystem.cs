using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    public class DeathProcessTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private IDisposable _isDeadSubscription;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _isDeadSubscription = _entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.InDeathProcess.Value == false)
                return;

            _entity.DeathProcessCurrentTime.Value -= deltaTime;

            if (_entity.DeathProcessCurrentTime.Value <= 0)
            {
                _entity.InDeathProcess.Value = false;
            }
        }

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead && _entity.InDeathProcess.Value == false)
            {
                _entity.DeathProcessCurrentTime.Value = _entity.DeathProcessInitialTime.Value;
                _entity.InDeathProcess.Value = true;
            }
        }

        public void OnDispose()
        {
            _isDeadSubscription?.Dispose();
        }
    }
}