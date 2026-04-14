using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly LayerMask _enemyMask;
        private Entity _entity;
        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _plungeAOERadius;
        private ReactiveVariable<float> _plungeAOEDamage;
        private ReactiveVariable<float> _plungeKnockbackForce;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        // Таймер для расчета прогресса падения
        private float _currentFlightTime;

        // services
        private readonly AudioService _audioService;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly DropLootService _dropLootService;

        // Константы баланса
        private const float FlightCheckWidth = 1.5f;
        private const float FlightCheckHeight = 1.0f;
        private const float FlightKnockbackMultiplier = 0.3f;

        // Настройки прогрессии урона
        private const float BaseChargeTime = 0.5f; // Время, за которое набирается 100% мощи
        private const float MaxDamageMultiplier = 2.5f; // Максимальный бонус урона (x2.5)
        private const float MaxRadiusMultiplier = 1.4f; // Максимальное расширение радиуса (x1.4)

        private readonly CameraService _cameraService;

        public PlungeSystem(IInputService inputService, LayerMask enemyMask, CameraService cameraService, AudioService audioService, ConfigsProviderService configsProviderService, DropLootService dropLootService)
        {
            _inputService = inputService;
            _enemyMask = enemyMask;
            _cameraService = cameraService;
            _audioService = audioService;
            _configsProviderService = configsProviderService;
            _dropLootService = dropLootService;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _canPlunge = entity.CanPlunge;
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;
            _plungeSpeed = entity.PlungeSpeed;
            _plungeAOERadius = entity.PlungeAOERadius;
            _plungeAOEDamage = entity.PlungeAOEDamage;
            _plungeKnockbackForce = entity.PlungeKnockbackForce;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isPlunging.Value)
            {
                _currentFlightTime += deltaTime;
                UpdatePlunge(deltaTime);
                return;
            }

            if (_inputService.IsSlideKeyPressed && _canPlunge.Evaluate())
            {
                StartPlunge();
            }
        }

        private void StartPlunge()
        {
            _isPlunging.Value = true;
            _currentFlightTime = 0f;
            // Резко толкаем вниз, сохраняя часть горизонтальной инерции
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.5f, -_plungeSpeed.Value);
        }

        private void UpdatePlunge(float deltaTime)
        {
            // Поддерживаем стабильную скорость падения
            if (_rigidbody.linearVelocity.y > -_plungeSpeed.Value)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, -_plungeSpeed.Value);
            }

            // Наносим урон врагам, через которых пролетаем (со слабым отбросом)
            ApplyFlightDamage();

            if (_isGrounded.Value)
            {
                LandPlunge();
                return;
            }

            // Прерывание, если отпустили кнопку (если это предусмотрено дизайном)
            if (_inputService.IsSlideKeyReleased)
            {
                StopPlunge();
            }
        }

        private void ApplyFlightDamage()
        {
            Vector2 checkPos = (Vector2)_transform.position + Vector2.down * 0.5f;
            Collider2D[] hits = Physics2D.OverlapBoxAll(checkPos, new Vector2(FlightCheckWidth, FlightCheckHeight), 0, _enemyMask);

            foreach (var hit in hits)
            {
                // Урон в полете всегда фиксированный (50% от базового)
                PushAndDamage(hit, _plungeAOEDamage.Value * 0.5f, _plungeKnockbackForce.Value * FlightKnockbackMultiplier);
            }
        }

        private void LandPlunge()
        {
            float intensityRatio = Mathf.Clamp(_currentFlightTime / BaseChargeTime, 0f, 1.0f);

            // Если падение длилось дольше 0.1 сек, даем встряску
            if (intensityRatio > 0.1f)
            {
                // Передаем интенсивность (например, от 0.3 до 1.0)
                _cameraService.Shake(intensityRatio);
            }

            float damageMultiplier = Mathf.Lerp(1.0f, MaxDamageMultiplier, intensityRatio);
            float radiusMultiplier = Mathf.Lerp(1.0f, MaxRadiusMultiplier, intensityRatio);

            float finalDamage = _plungeAOEDamage.Value * damageMultiplier;
            float finalRadius = _plungeAOERadius.Value * radiusMultiplier;
            float finalForce = _plungeKnockbackForce.Value * damageMultiplier;

            Collider2D[] hits = Physics2D.OverlapCircleAll(_transform.position, finalRadius, _enemyMask);

            foreach (Collider2D hit in hits)
            {
                PushAndDamage(hit, finalDamage, finalForce);
            }

            StopPlunge();
        }

        private void PushAndDamage(Collider2D hit, float damage, float force)
        {
            if (hit == null || !hit.gameObject.activeSelf) return;

            // 1. Физический импульс
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = ((Vector2)hit.transform.position - (Vector2)_transform.position);
                direction.y += 0.5f; // Подбрасываем немного вверх
                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            }

            // 2. Нанесение урона через Entity
            var monoEntity = hit.GetComponentInParent<MonoEntity>();
            if (monoEntity != null)
            {
                var target = monoEntity.LinkedEntity;
                if (target != null && target.HasComponent<CurrentHealth>())
                {
                    target.CurrentHealth.Value -= damage;

                    target.TakeDamageEvent?.Invoke(new DamageData
                    {
                        Amount = damage,
                        SourcePosition = _transform.position
                    });
                }
            }
            // test
            else
            {
                _audioService.PlaySfxByPrefixAuto("Box_Hit", UnityEngine.Random.Range(0.8f, 1.2f));
                LootTableConfig mainLootTableConfig = _configsProviderService.GetConfig<LootTableConfig>();
                _dropLootService.DropLootFor(_entity, mainLootTableConfig);
                Object.Destroy(hit.gameObject);
            }
        }

        private void StopPlunge()
        {
            _isPlunging.Value = false;
            _currentFlightTime = 0f;
        }
    }
}