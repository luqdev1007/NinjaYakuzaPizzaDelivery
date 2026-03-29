using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using System.Collections;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ShurikenProjectile : ThrowableProjectile
    {
        private readonly ShurikenConfig _config;
        private readonly List<Entity> _hitEntities = new();
        private int _pierceLeft = 3;

        public ShurikenProjectile(ShurikenConfig config, ICoroutinesPerformer coroutinesPerformer)
            : base(config, coroutinesPerformer)
        {
            _config = config;
        }

        protected override void ApplyRotation(Vector3 direction)
        {
            if (Instance != null)
                Instance.transform.Rotate(0, 0, 1000f * Time.deltaTime);
        }

        protected override void OnHit(Collider2D hit)
        {
            var monoEntity = hit.GetComponentInParent<MonoEntity>();

            if (monoEntity != null)
            {
                Entity target = monoEntity.LinkedEntity;

                if (target != null && !_hitEntities.Contains(target))
                {
                    _hitEntities.Add(target);

                    if (target.HasComponent<CurrentHealth>())
                        target.CurrentHealth.Value -= _config.Damage;

                    _pierceLeft--;
                    Debug.Log($"Пробитие! Осталось: {_pierceLeft}");

                    // Если пробития кончились — уничтожаем (или втыкаем во врага, если хочешь)
                    if (_pierceLeft <= 0) Destroy();
                }
            }
            else
            {
                // ПОПАЛИ В ЗЕМЛЮ/СТЕНУ
                CoroutinesPerformer.StartPerform(StickInSurfaceCoroutine());
            }
        }

        private IEnumerator StickInSurfaceCoroutine()
        {
            // Останавливаем вращение и движение
            var rb = Instance.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false; // Выключаем физику, чтобы не падал

            // Выключаем коллайдер снаряда, чтобы он больше ничего не задевал
            var col = Instance.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Debug.Log("Сюрикен застрял в стене на 3 секунды...");

            yield return new WaitForSeconds(3f);

            Destroy();
        }
    }
}