using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ShurikenProjectile : ThrowableProjectile
    {
        private readonly ShurikenConfig _config;

        private readonly ConfigsProviderService _configsProviderService;
        private readonly DropLootService _dropLootService;

        private bool _isStuck;

        private const float RotationSpeed = 360 * 5f;

        public ShurikenProjectile(ShurikenConfig config, ICoroutinesPerformer coroutinesPerformer,
            ConfigsProviderService configsProviderService, 
            DropLootService dropLootService)
            : base(config, coroutinesPerformer)
        {
            _config = config;
            _configsProviderService = configsProviderService;
            _dropLootService = dropLootService;
        }

        protected override void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            base.OnHitAtPoint(point, hit);

            if (_isStuck) return;

            var monoEntity = hit.GetComponentInParent<MonoEntity>();

            if (monoEntity != null)
            {
                var target = monoEntity.LinkedEntity;

                if (target != null && target.HasComponent<TakeDamageRequest>())
                {
                    /*
                    // Передаем Type = DamageType.Cut, чтобы сработала гибкая логика звука у врага
                    target.TakeDamageRequest.Invoke(new DamageData
                    {
                        Amount = _config.Damage,
                        SourcePosition = hit.ClosestPoint(Instance.transform.position),
                        Type = DamageType.Cut
                    });
                    */
                }

                Destroy(); // Мясо — уничтожаем сразу
            }
            // test
            else if (hit.gameObject.layer == LayersAPI.LayerProps)
            {
                // ИСПРАВЛЕНО: Мастер-провайдер
                var lootProvider = _configsProviderService.GetConfig<MasterLootProviderConfig>();
                // _dropLootService.DropLootFor(hit.transform.position, lootProvider.PropsLoot);

                Object.Destroy(hit.gameObject);
            }
            else
            {
                // Стена — фиксируем
                _isStuck = true;

                CoroutinesPerformer.StartPerform(StickInSurfaceCoroutine());
            }
        }

        private IEnumerator StickInSurfaceCoroutine()
        {
            if (Instance == null) yield break;

            // Отключаем физику и коллизии мгновенно
            var col = Instance.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            var rb = Instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = false;
            }

            // Висим 3 секунды и исчезаем
            yield return new WaitForSeconds(3f);
            Destroy();
        }

        protected override void ApplyRotation(Vector3 direction)
        {
            // Пока не воткнулись — крутимся
            if (Instance != null && !_isStuck)
            {
                Instance.transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
            }
        }
    }
}