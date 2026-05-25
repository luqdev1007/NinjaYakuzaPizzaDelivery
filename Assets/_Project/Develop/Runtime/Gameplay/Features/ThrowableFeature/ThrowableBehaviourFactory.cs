using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ThrowableBehaviourFactory : IThrowableBehaviourFactory
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly ConfigsProviderService _configsProviderService;

        public ThrowableBehaviourFactory(
            ICoroutinesPerformer coroutinesPerformer, 
            ConfigsProviderService configsProviderService)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _configsProviderService = configsProviderService;
        }

        public ThrowableProjectile Create(ThrowableItemConfig config)
        {
            return config switch
            {
                GrappleHookConfig grappleConfig => new GrappleHookProjectile(
                    grappleConfig, _coroutinesPerformer),

                    /*
                ShurikenConfig shurikenConfig => new ShurikenProjectile(
                    shurikenConfig, _coroutinesPerformer, _configsProviderService, _dropLootService),
                    */

                SleepDartConfig dartConfig => new SleepDartProjectile(
                    dartConfig, _coroutinesPerformer),

                _ => null
            };
        }

        public object Create(ThrowableItemConfig throwableConfig, Rigidbody2D rigidbody, Transform transform)
        {
            throw new System.NotImplementedException();
        }
    }
}