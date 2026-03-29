using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;

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

                // Чтобы не дамажить одного и того же врага каждым кадром пролета
                if (target != null && !_hitEntities.Contains(target))
                {
                    _hitEntities.Add(target);

                    // Наносим урон (замени на свою переменную здоровья)
                    if (target.HasComponent<CurrentHealth>())
                        target.CurrentHealth.Value -= _config.Damage;

                    _pierceLeft--;
                    Debug.Log($"Сюрикен пробил {hit.name}, осталось пробитий: {_pierceLeft}");

                    if (_pierceLeft <= 0) Destroy();
                }
            }
            else
            {
                // Если попали не в Entity (например, стена) — ломаемся сразу
                Destroy();
            }
        }
    }
}