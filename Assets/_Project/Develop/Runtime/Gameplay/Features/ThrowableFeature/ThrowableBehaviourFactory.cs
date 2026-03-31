using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ThrowableBehaviourFactory : IThrowableBehaviourFactory
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly AudioService _audioService;

        public ThrowableBehaviourFactory(ICoroutinesPerformer coroutinesPerformer, AudioService audioService)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _audioService = audioService;
        }

        public ThrowableProjectile Create(ThrowableConfig config, Rigidbody2D rigidbody, Transform transform)
        {
            // Теперь все снаряды создаются единообразно, так как логика притяжения крюка 
            // переехала в ThrowableSystem, а снаряд только детектит попадание.
            return config switch
            {
                GrappleHookConfig grappleConfig => new GrappleHookProjectile(
                    grappleConfig, _coroutinesPerformer),

                ShurikenConfig shurikenConfig => new ShurikenProjectile(
                    shurikenConfig, _coroutinesPerformer, _audioService),

                SleepDartConfig dartConfig => new SleepDartProjectile(
                    dartConfig, _coroutinesPerformer),

                _ => null
            };
        }
    }
}