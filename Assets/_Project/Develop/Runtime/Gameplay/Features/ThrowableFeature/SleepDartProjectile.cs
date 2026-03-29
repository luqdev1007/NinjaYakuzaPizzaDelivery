using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Common;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;

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

        protected override void ApplyRotation(Vector3 direction)
        {
            if (Instance == null) return;
            // Направляем нос дротика по вектору полета
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        protected override void OnHit(Collider2D hit)
        {
            var monoEntity = hit.GetComponentInParent<MonoEntity>();
            if (monoEntity != null)
            {
                var entity = monoEntity.LinkedEntity;
                if (!entity.HasComponent<IsAsleep>()) entity.AddIsAsleep();

                entity.IsAsleep.Value = true;

                // Чтобы эффект был "явно заметным", можно сразу стопнуть скорость врага здесь
                if (entity.Transform.GetComponent<Rigidbody2D>())
                    entity.Rigidbody.linearVelocity = Vector2.zero;
            }

            Destroy();
        }
    }
}