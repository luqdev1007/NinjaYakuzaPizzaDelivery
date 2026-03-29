using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class SleepDartProjectile : ThrowableProjectile
    {
        private readonly SleepDartConfig _config;

        public SleepDartProjectile(SleepDartConfig config, ICoroutinesPerformer coroutinesPerformer)
            : base(config, coroutinesPerformer)
        {
            _config = config;
        }

        protected override void OnHit(Collider2D hit)
        {
            var monoEntity = hit.GetComponentInParent<MonoEntity>();
            if (monoEntity != null)
            {
                var entity = monoEntity.LinkedEntity;

                if (!entity.HasComponent<IsAsleep>())
                    entity.AddIsAsleep();

                entity.IsAsleep.Value = true;
                
                Debug.Log($"Entity {hit.name} уснул на {_config.SleepDuration}с");
            }

            Destroy();
        }
    }
}