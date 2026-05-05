using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntitiesFactory : IInitializable, IDisposable
    {
        private readonly DIContainer _container;
        private readonly ResourcesAssetsLoader _resources;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly CollidersRegistryService _collidersRegistryService;

        private readonly Dictionary<Entity, MonoEntity> _entityToMono = new();

        public MonoEntitiesFactory(
            DIContainer container,
            ResourcesAssetsLoader resources,
            EntitiesLifeContext entitiesLifeContext,
            CollidersRegistryService collidersRegistryService)
        {
            _container = container;
            _resources = resources;
            _entitiesLifeContext = entitiesLifeContext;
            _collidersRegistryService = collidersRegistryService;
        }

        public void Initialize()
        {
            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public MonoEntity Create(Entity entity, Vector3 position, string path)
        {
            MonoEntity prefab = _resources.Load<MonoEntity>(path);
            MonoEntity viewInstance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            SetupMonoEntity(entity, viewInstance);

            _entityToMono.Add(entity, viewInstance);
            return viewInstance;
        }

        public MonoEntity Create(Entity entity, Transform parent, string path)
        {
            MonoEntity prefab = _resources.Load<MonoEntity>(path);
            MonoEntity viewInstance = Object.Instantiate(prefab, parent);

            SetupMonoEntity(entity, viewInstance);

            _entityToMono.Add(entity, viewInstance);
            return viewInstance;
        }

        private void SetupMonoEntity(Entity entity, MonoEntity viewInstance)
        {
            InjectDependencies(viewInstance);

            viewInstance.Initialize(_collidersRegistryService);

            viewInstance.Link(entity);
        }

        private void InjectDependencies(MonoEntity monoEntity)
        {
            EntityView[] views = monoEntity.GetComponentsInChildren<EntityView>(true);

            if (views == null) return;

            foreach (EntityView view in views)
            {
                view.ResolveDependencies(_container);
            }
        }

        public void Dispose()
        {
            _entitiesLifeContext.Released -= OnEntityReleased;

            foreach (Entity entity in _entityToMono.Keys)
            {
                CleanupFor(entity);
            }

            _entityToMono.Clear();
        }

        private void OnEntityReleased(Entity entity)
        {
            if (_entityToMono.ContainsKey(entity))
            {
                CleanupFor(entity);
                _entityToMono.Remove(entity);
            }
        }

        private void CleanupFor(Entity entity)
        {
            MonoEntity monoEntity = _entityToMono[entity];

            if (monoEntity != null)
            {
                monoEntity.Cleanup(entity);
                Object.Destroy(monoEntity.gameObject);
            }
        }
    }
}