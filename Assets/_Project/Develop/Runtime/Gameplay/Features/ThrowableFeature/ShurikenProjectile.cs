using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ShurikenProjectile : ThrowableProjectile
    {
        private readonly ShurikenConfig _config;

        public ShurikenProjectile(ShurikenConfig config, ICoroutinesPerformer coroutinesPerformer)
            : base(config, coroutinesPerformer)
        {
            _config = config;
        }

        protected override void OnHit(Collider2D hit)
        {
            var monoEntity = hit.GetComponentInParent<MonoEntity>();

            if (monoEntity != null)
            {
                // Попали во врага: наносим урон и исчезаем
                var target = monoEntity.LinkedEntity;
                if (target != null && target.HasComponent<CurrentHealth>())
                    target.CurrentHealth.Value -= _config.Damage;

                Destroy(); // Исчезает мгновенно
            }
            else
            {
                // Попали в стену: втыкаемся
                CoroutinesPerformer.StartPerform(StickInSurfaceCoroutine());
            }
        }

        private IEnumerator StickInSurfaceCoroutine()
        {
            if (Instance == null) yield break;

            var col = Instance.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            yield return new WaitForSeconds(3f);
            Destroy();
        }

        protected override void ApplyRotation(Vector3 direction)
        {
            if (Instance != null)
                Instance.transform.Rotate(0, 0, 1200f * Time.deltaTime);
        }
    }
}