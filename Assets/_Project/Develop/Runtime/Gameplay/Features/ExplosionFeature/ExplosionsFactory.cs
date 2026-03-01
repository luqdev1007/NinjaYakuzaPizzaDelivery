using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Explosions;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class ExplosionsFactory
    {
        private readonly ExplosionsListConfig _configs;

        private readonly DIContainer _container;

        public ExplosionsFactory(DIContainer container)
        {
            _container = container;
            _configs = _container.Resolve<ConfigsProviderService>().GetConfig<ExplosionsListConfig>();
        }

        public ExplosionView Create(ExplosionType type, Vector3 at, bool activateOnCreate = false)
        {
            var config = _configs.GetBy(type);

            if (config == null)
                throw new ArgumentException($"no config for {type} type explosion");

            ExplosionView instance = Object.Instantiate(config.ViewPrefab, at, Quaternion.identity);
            instance.Initialize(new Explosion(config.Range, config.Power), activateOnCreate);

            return instance;
        }
    }
}