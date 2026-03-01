using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroHolderService : IInitializable, IDisposable
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private ReactiveEvent<Entity> _heroRegistred = new();

        private Entity _mainHero;

        public MainHeroHolderService(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public IReadOnlyEvent<Entity> HeroRegistred => _heroRegistred;
        public Entity MainHero => _mainHero;

        public void Initialize()
        {
            _entitiesLifeContext.Added += OnEntityAdded;
            Debug.Log("main heroes holder service init");
        }

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
        }

        private void OnEntityAdded(Entity entity)
        {
            if (entity.HasComponent<MainHeroTag>())
            {
                _mainHero = entity;
                _heroRegistred?.Invoke(entity);
                _entitiesLifeContext.Added -= OnEntityAdded;
            }
        }
    }
}
