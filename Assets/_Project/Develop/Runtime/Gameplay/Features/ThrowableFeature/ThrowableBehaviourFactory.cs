using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ThrowableBehaviourFactory : IThrowableBehaviourFactory
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public ThrowableBehaviourFactory(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public ThrowableProjectile Create(ThrowableConfig config, Rigidbody2D rigidbody, Transform transform)
        {
            return config switch
            {
                GrappleHookConfig grappleConfig => new GrappleHookProjectile(
                    grappleConfig, _coroutinesPerformer, rigidbody, transform),
                ShurikenConfig shurikenConfig => new ShurikenProjectile(
                    shurikenConfig, _coroutinesPerformer),
                SleepDartConfig dartConfig => new SleepDartProjectile(
                    dartConfig, _coroutinesPerformer),

                _ => null
            };
        }
    }
}