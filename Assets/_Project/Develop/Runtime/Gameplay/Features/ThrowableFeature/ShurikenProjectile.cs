using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ShurikenProjectile : ThrowableProjectile
    {
        private readonly ShurikenConfig _config;

        public ShurikenProjectile(
            ShurikenConfig config,
            ICoroutinesPerformer coroutinesPerformer) : base(config, coroutinesPerformer)
        {
            _config = config;
        }

        protected override void OnHit(Collider2D hit)
        {
            Debug.Log($"Сюрикен попал в {hit.gameObject.name}, урон: {_config.Damage}");
            Destroy();
        }

        protected override void OnMaxDistanceReached(Vector3 startPosition)
        {
            Destroy();
        }
    }
}