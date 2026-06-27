using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootLifeCycleSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private Entity _entity;
        private bool _isDestroyed;

        public LootLifeCycleSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isDestroyed || _entity.LootIsCollected.Value || _entity.Transform == null)
                return;

            if (_entity.InSpawnProcess.Value)
                return;

            if (_entity.CurrentTarget.Value == null)
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