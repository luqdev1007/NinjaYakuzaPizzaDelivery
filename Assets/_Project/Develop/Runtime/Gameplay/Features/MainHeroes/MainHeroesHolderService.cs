using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroesHolderService : IInitializable, IDisposable
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private ReactiveEvent<Entity> _heroRegistred = new();
        private ReactiveEvent<Entity> _mainShipRegistred = new();

        private List<Entity> _mainHeroes = new();
        private Entity _mainShip;

        public MainHeroesHolderService(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public IReadOnlyEvent<Entity> HeroRegistred => _heroRegistred;
        public IReadOnlyEvent<Entity> MainShipRegistred => _mainShipRegistred;
        public IReadOnlyCollection<Entity> MainHeroes => _mainHeroes;
        public Entity MainShip => _mainShip;

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
                _mainHeroes.Add(entity);
                _heroRegistred?.Invoke(entity);
            }

            if (entity.HasComponent<MainShipTag>())
            {
                _mainShip = entity;
                _mainShipRegistred?.Invoke(entity);
            }

            if (_mainHeroes.Count == 3 && _mainShip != null)
                _entitiesLifeContext.Added -= OnEntityAdded;
        }

        public bool IsAllHeroesDead()
        {
            if (_mainHeroes.Count == 0) 
                return false;

            bool isConditionCompleted = true;

            foreach (Entity hero in _mainHeroes)
            {
                if (hero.MustDie.Evaluate() == false)
                {
                    isConditionCompleted = false;
                    return isConditionCompleted;
                }
            }

            return isConditionCompleted;
        }

        public bool IsMainShipDestroyed()
        {
            return _mainShip.InDeathProcess.Value == true;
        }
    }
}
