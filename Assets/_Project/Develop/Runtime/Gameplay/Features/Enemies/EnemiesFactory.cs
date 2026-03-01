using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class EnemiesFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly VehiclesFactory _vehiclesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public EnemiesFactory(DIContainer container, VehiclesFactory vehiclesFactory)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _vehiclesFactory = vehiclesFactory;
        }

        public Entity Create(Transform at, EntityConfig config)
        {
            Entity entity;

            switch (config)
            {
                case SoldierConfig soldierConfig:
                    entity = _entitiesFactory.CreateSoldier(at, soldierConfig);
                    _brainsFactory.CreateEmptyBrain(entity);
                    break;

                case ArcherConfig archerConfig:
                    entity = _entitiesFactory.CreateArcher(at, archerConfig);
                    _brainsFactory.CreateEmptyBrain(entity);
                    break;

                case DriverConfig driverConfig:
                    entity = _entitiesFactory.CreateDriver(at, driverConfig);
                    _brainsFactory.CreateEmptyBrain(entity);
                    break;

                default:
                    throw new ArgumentException($"Not support {config.GetType()} type config");
            }

            entity.AddTeam(new ReactiveVariable<Teams>(Teams.Enemies));

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
