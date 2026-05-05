using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    public class DeathSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _healthSubscription;

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _healthSubscription = _entity.CurrentHealth.Subscribe(OnHealthChanged);
        }

        private void OnHealthChanged(float oldHealth, float currentHealth)
        {
            if (_entity.IsDead.Value)
                return;

            if (_entity.MustDie.Evaluate())
            {
                ExecuteDeath();
            }
        }

        private void ExecuteDeath()
        {
            _entity.IsDead.Value = true;

            _entity.DeathEvent?.Invoke();

            if (_entity.Rigidbody != null)
            {
                _entity.Rigidbody.simulated = false;
            }
        }

        public void OnDispose()
        {
            _healthSubscription?.Dispose();
        }
    }
}