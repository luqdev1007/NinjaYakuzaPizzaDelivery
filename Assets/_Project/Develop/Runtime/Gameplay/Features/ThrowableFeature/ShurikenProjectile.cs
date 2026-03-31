using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature
{
    public class ShurikenProjectile : ThrowableProjectile
    {
        private readonly ShurikenConfig _config;
        private bool _isStuck;

        // Скорость вращения (градусы в секунду)
        private const float RotationSpeed = 360 * 5f;

        public ShurikenProjectile(ShurikenConfig config, ICoroutinesPerformer coroutinesPerformer)
            : base(config, coroutinesPerformer)
        {
            _config = config;
        }

        protected override void OnHitAtPoint(Vector2 point, Collider2D hit)
        {
            if (_isStuck) return;

            var monoEntity = hit.GetComponentInParent<MonoEntity>();

            if (monoEntity != null)
            {
                var target = monoEntity.LinkedEntity;

                // Вместо проверки здоровья проверяем наличие компонента запроса урона
                if (target != null && target.HasComponent<TakeDamageRequest>())
                {
                    // Формируем запрос, который активирует ApplyDamageSystem
                    target.TakeDamageRequest.Invoke(new DamageData
                    {
                        Amount = _config.Damage,
                        SourcePosition = hit.ClosestPoint(Instance.transform.position)
                    });
                }

                Destroy(); // Мясо — уничтожаем сразу
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
            // Как только _isStuck станет true, этот метод перестанет крутить объект,
            // и сюрикен застынет под тем углом, под которым ударился.
        }
    }
}