using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class ProjectilesFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public ProjectilesFactory(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        public Entity Create(Transform parent, ProjectileCreationContext ctx) // SimpleProjectileConfig config)
        {
            Entity entity = null;

            /*
            switch (config)
            {
                case SimpleProjectileConfig simpleProjectileConfig:
                    entity = _entitiesFactory.CreateSimpleProjectile(parent, ctx, simpleProjectileConfig); 
                    _brainsFactory.CreateEmptyBrain(entity);
                    break;

                default:
                    throw new ArgumentException($"Not support {config.GetType()} type config");
            }
            */

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
