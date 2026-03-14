using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class SleepDartProjectile : ThrowableProjectile
    {
        private readonly SleepDartConfig _config;

        public SleepDartProjectile(
            SleepDartConfig config,
            ICoroutinesPerformer coroutinesPerformer) : base(config, coroutinesPerformer)
        {
            _config = config;
        }

        protected override void OnHit(Collider2D hit)
        {
            Debug.Log($"Дротик попал в {hit.gameObject.name}, усыпляет на {_config.SleepDuration}с");
            Destroy();
        }

        protected override void OnMaxDistanceReached(Vector3 startPosition)
        {
            Destroy();
        }
    }
}