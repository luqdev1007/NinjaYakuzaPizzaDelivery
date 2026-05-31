using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootLifeCycleSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly float _spawnDuration;
        private readonly float _lifeTime;
        private readonly EntitiesLifeContext _entitiesLifeContext; 

        private Entity _entity;
        private float _spawnTimer;
        private bool _isDestroyed; 

        public LootLifeCycleSystem(float spawnDuration, float lifeTime, EntitiesLifeContext entitiesLifeContext)
        {
            _spawnDuration = spawnDuration;
            _lifeTime = lifeTime;
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _entity.LootInitialLifeTime.Value = _lifeTime;
            _entity.LootCurrentLifeTime.Value = _lifeTime;
            _entity.InSpawnProcess.Value = true;
            _spawnTimer = _spawnDuration;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDestroyed || _entity.LootIsCollected.Value || _entity.Transform == null)
                return;

            if (_entity.InSpawnProcess.Value)
            {
                _spawnTimer -= deltaTime;

                if (_spawnTimer <= 0)
                {
                    _entity.InSpawnProcess.Value = false;
                }
            }

            if (!_entity.InSpawnProcess.Value && _entity.CurrentTarget.Value == null)
            {
                _entity.LootCurrentLifeTime.Value -= deltaTime;

                if (_entity.LootCurrentLifeTime.Value <= 0)
                {
                    _isDestroyed = true;

                    _entitiesLifeContext.Release(_entity);

                    Object.Destroy(_entity.Transform.gameObject);
                }
            }
        }
    }
}