using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs
{
    public class BuffPickupAuthoring : MonoEntity
    {
        [SerializeField] private BuffConfig _config;

        private EntitiesLifeContext _entitiesLifeContext;

        public void Construct(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        private void Start()
        {
            if (_entitiesLifeContext == null)
            {
                Debug.LogError($"[BuffPickup] Контекст не внедрен в {gameObject.name}!");

                return;
            }

            BuildBuffEntity();

            _entitiesLifeContext.Released += OnEntityReleased;
        }

        private void BuildBuffEntity()
        {
            Entity entity = new Entity();

            entity
                .AddTransform(transform)
                .AddBuffIsCollected(new ReactiveVariable<bool>(false))
                .AddCurrentTarget(new ReactiveVariable<Entity>(null))
                .AddBuffPickupConfig(_config);

            entity.AddSystem(new BuffArcMovementSystem(_config.TravelTime, _config.ArcHeight));

            EntityView[] views = GetComponentsInChildren<EntityView>();

            foreach (EntityView view in views)
            {
                view.Link(entity);
            }

            LinkedEntity = entity;
            _entitiesLifeContext.Add(entity);
        }

        private void OnEntityReleased(Entity entity)
        {
            if (entity == LinkedEntity)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_entitiesLifeContext != null)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;
            }
        }
    }
}