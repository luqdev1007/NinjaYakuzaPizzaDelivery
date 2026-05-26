using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Plunge
{
    public class PlungeDamageOnImpactSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly CollidersRegistryService _collidersRegistryService;

        private Transform _entityTransform;

        private ReactiveVariable<float> _impactRange;
        private ReactiveVariable<float> _impactDamage;

        private LayerMask _hitMask;

        private IDisposable _disposable;

        public PlungeDamageOnImpactSystem(CollidersRegistryService collidersRegistryService)
        {
            _collidersRegistryService = collidersRegistryService;
        }

        public void OnInit(Entity entity)
        {
            _entityTransform = entity.Transform;

            _impactRange = entity.PlungeLandImpactRange;
            _impactDamage = entity.PlungeLandImpactDamage;

            _hitMask = entity.PlungeLandImpactHitMask;

            _disposable = entity.PlungeImpactEvent.Subscribe(OnPlungeImpact);
        }

        public void OnDispose()
        {
            _disposable?.Dispose();
        }

        private void OnPlungeImpact(float landVelocity)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_entityTransform.position, _impactRange.Value, _hitMask);

            Debug.Log($"Hit counts: {hitColliders.Length}");

            foreach( Collider2D collider in hitColliders)
            {
                Entity entity = _collidersRegistryService.GetBy(collider);

                if (entity == null)
                {
                    continue;
                }

                entity.TakeDamageRequest.Invoke(new DamageData
                {
                    Amount = _impactDamage.Value,
                    SourcePosition = _entityTransform.position,
                });
            }
        }
    }
}