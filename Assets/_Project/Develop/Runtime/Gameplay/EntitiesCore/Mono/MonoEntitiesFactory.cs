using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntitiesFactory : IInitializable, IDisposable
    {
        private readonly ResourcesAssetsLoader _resources;

        private readonly EntitiesLifeContext _entitiesLifeContext;

        private readonly CollidersRegistryService _collidersRegistryService;
        private readonly IAudioService _audioService;

        private readonly Dictionary<Entity, MonoEntity> _entityToMono = new();

        public MonoEntitiesFactory(
            ResourcesAssetsLoader resources,
            EntitiesLifeContext entitiesLifeContext,
            CollidersRegistryService collidersRegistryService,
            IAudioService audioService)
        {
            _resources = resources;
            _entitiesLifeContext = entitiesLifeContext;
            _collidersRegistryService = collidersRegistryService;
            _audioService = audioService;
        }

        public MonoEntity Create(Entity entity, Vector3 position, string path)
        {
            MonoEntity prefab = _resources.Load<MonoEntity>(path);

            MonoEntity viewInstance = Object.Instantiate(prefab, position, Quaternion.identity, null);

            viewInstance.Initialize(_collidersRegistryService, _audioService);
            viewInstance.Link(entity);

            _entityToMono.Add(entity, viewInstance);

            return viewInstance;
        }

        public MonoEntity Create(Entity entity, Transform parent, string path)
        {
            MonoEntity prefab = _resources.Load<MonoEntity>(path);

            MonoEntity viewInstance = Object.Instantiate(prefab, parent);

            viewInstance.Initialize(_collidersRegistryService, _audioService);
            viewInstance.Link(entity);

            _entityToMono.Add(entity, viewInstance);

            return viewInstance;
        }

        public void Initialize()
        {
            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public void Dispose()
        {
            Entity[] entities = _entityToMono.Keys.ToArray();

            foreach (var entity in entities)
                CleanupFor(entity);

            _entityToMono.Clear();
        }

        private void OnEntityReleased(Entity entity)
        {
            CleanupFor(entity);
        }

        private void CleanupFor(Entity entity)
        {
            if (_entityToMono.TryGetValue(entity, out MonoEntity monoEntity))
            {
                if (monoEntity != null)
                {
                    monoEntity.Cleanup(entity);

                    if (monoEntity.gameObject != null)
                    {
                        Object.Destroy(monoEntity.gameObject);
                    }
                }
                _entityToMono.Remove(entity);
            }
        }
    }
}
