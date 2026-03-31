using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    public class DeathSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private readonly string _entityId;
        private readonly AudioService _audioService;

        private ReactiveVariable<bool> _isDead;
        private ICompositeCondition _mustDie;
        private IDisposable _deathSubscription;

        public DeathSystem(string entityId, AudioService audioService)
        {
            _entityId = entityId;
            _audioService = audioService;
        }

        public void OnInit(Entity entity)
        {
            _isDead = entity.IsDead;
            _mustDie = entity.MustDie;
            _deathSubscription = _isDead.Subscribe(OnDeathChanged);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDead.Value) return;

            if (_mustDie.Evaluate())
                _isDead.Value = true;
        }

        private void OnDeathChanged(bool old, bool isDead)
        {
            if (isDead)
            {
                // _audioService.PlaySfxByPrefix(_entityId + "Death", true);
            }
        }

        public void OnDispose() => _deathSubscription?.Dispose();
    }
}