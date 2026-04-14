using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly LayerMask _enemyMask;

        private LayerMask _hitMask;

        private ICompositeCondition _canDash;
        private ReactiveVariable<bool> _isDashing;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _dashForceMin;
        private ReactiveVariable<float> _dashForceMax;
        private ReactiveVariable<float> _dashChargeTime;
        private ReactiveVariable<float> _dashCooldown;
        private ReactiveVariable<float> _dashDuration;
        private ReactiveVariable<float> _airDashMultiplier;
        private ReactiveVariable<float> _airDashVerticalBoost;
        private ReactiveVariable<float> _dashDamage;
        private ReactiveVariable<Vector2> _dashHitboxSize;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _chargeTimer;
        private float _cooldownTimer;
        private float _dashBufferTimer;
        private bool _isCharging;

        private readonly AudioService _audioService;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly DropLootService _dropLootService;


        private const float DashBufferTime = 0.15f;

        public DashSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer, LayerMask enemyMask, AudioService audioService, ConfigsProviderService configsProviderService, DropLootService dropLootService)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _enemyMask = enemyMask;
            _audioService = audioService;
            _configsProviderService = configsProviderService;
            _dropLootService = dropLootService;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _canDash = entity.CanDash;
            _isDashing = entity.IsDashing;
            _isGrounded = entity.IsGrounded;
            _dashForceMin = entity.DashForceMin;
            _dashForceMax = entity.DashForceMax;
            _dashChargeTime = entity.DashChargeTime;
            _dashCooldown = entity.DashCooldown;
            _dashDuration = entity.DashDuration;
            _airDashMultiplier = entity.AirDashMultiplier;
            _airDashVerticalBoost = entity.AirDashVerticalBoost;
            _dashDamage = entity.DashDamage;
            _dashHitboxSize = entity.DashHitboxSize;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

            _hitMask = _enemyMask | LayersAPI.LayerMaskProps;
        }

        public void OnUpdate(float deltaTime)
        {
            float unscaledDt = Time.unscaledDeltaTime;

            if (_cooldownTimer > 0f)
                _cooldownTimer -= unscaledDt;

            if (_inputService.IsDashKeyPressed)
                _dashBufferTimer = DashBufferTime;
            else if (_dashBufferTimer > 0f)
                _dashBufferTimer -= unscaledDt;

            if (_dashBufferTimer > 0f && _canDash.Evaluate() && !_isCharging && _cooldownTimer <= 0)
            {
                _isCharging = true;
                _chargeTimer = 0f;
                _dashBufferTimer = 0f;
            }

            if (_isCharging)
            {
                if (_inputService.IsDashKeyHeld)
                {
                    _chargeTimer = Mathf.Min(_chargeTimer + deltaTime, _dashChargeTime.Value);
                }

                if (_inputService.IsDashKeyReleased)
                {
                    if (_canDash.Evaluate())
                        ExecuteDash();
                    else
                        _isCharging = false;
                }
            }
        }

        private void ExecuteDash()
        {
            float chargeRatio = _dashChargeTime.Value > 0f ? _chargeTimer / _dashChargeTime.Value : 1f;
            float force = Mathf.Lerp(_dashForceMin.Value, _dashForceMax.Value, chargeRatio);

            bool inAir = !_isGrounded.Value;
            if (inAir) force *= _airDashMultiplier.Value;

            float direction = _transform.localScale.x > 0 ? 1f : -1f;

            _isDashing.Value = true;
            _cooldownTimer = _dashCooldown.Value;
            _isCharging = false;

            _coroutinesPerformer.StartPerform(DashCoroutine(force, direction, inAir));
        }

        private IEnumerator DashCoroutine(float force, float direction, bool inAir)
        {
            // В начале DashCoroutine
            Physics2D.IgnoreLayerCollision(LayersAPI.LayerCharacters, LayersAPI.LayerEnemies, true);



            float elapsed = 0f;
            float duration = _dashDuration.Value;
            float gravityScale = _rigidbody.gravityScale;
            HashSet<Entity> hitEntities = new HashSet<Entity>();

            _rigidbody.gravityScale = 0f;
            Vector2 dashVelocity = new Vector2(direction * force, inAir ? _airDashVerticalBoost.Value : 0f);
            _rigidbody.linearVelocity = dashVelocity;

            Vector2 lastFramePos = _transform.position;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                // Плавное затухание скорости для лучшего ощущения контроля
                float speedCurve = 1f - (t * t);
                float currentSpeed = force * speedCurve;

                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                // Проверка урона по траектории от прошлого до текущего кадра
                ApplyDashDamage(hitEntities, inAir, lastFramePos);
                lastFramePos = _transform.position;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Финальная проверка в конце пути
            ApplyDashDamage(hitEntities, inAir, lastFramePos);

            _rigidbody.linearVelocity = new Vector2(direction * 2f, _rigidbody.linearVelocity.y);
            _rigidbody.gravityScale = gravityScale;
            _isDashing.Value = false;

            // В конце DashCoroutine
            Physics2D.IgnoreLayerCollision(LayersAPI.LayerCharacters, LayersAPI.LayerEnemies, false);
        }

        private void ApplyDashDamage(HashSet<Entity> hitEntities, bool inAir, Vector2 lastPosition)
        {
            Vector2 currentPos = (Vector2)_transform.position;
            float direction = Mathf.Sign(_transform.localScale.x);

            Vector2 hitboxOffset = new Vector2(direction * (_dashHitboxSize.Value.x * 0.4f), 0f);
            Vector2 origin = lastPosition + hitboxOffset;
            Vector2 target = currentPos + hitboxOffset;

            float distance = Vector2.Distance(origin, target);
            Vector2 castDir = distance > 0.001f ? (target - origin).normalized : Vector2.right * direction;

            // Используем обновленную _hitMask (враги + пропсы)
            RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, _dashHitboxSize.Value, 0f, castDir, distance, _hitMask);

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                // ПРОВЕРКА НА ПРОПСЫ (Коробки и т.д.) через слой
                if (hit.collider.gameObject.layer == LayersAPI.LayerProps)
                {
                    HandlePropDestruction(hit.collider.gameObject);
                    continue;
                }

                // ПРОВЕРКА НА ВРАГОВ (Entities)
                var mono = hit.collider.GetComponentInParent<MonoEntity>();
                if (mono == null) continue;

                Entity targetEntity = mono.LinkedEntity;
                if (targetEntity == null || hitEntities.Contains(targetEntity)) continue;

                hitEntities.Add(targetEntity);
                DealDamageToEntity(targetEntity, currentPos, inAir);
            }
        }

        private void HandlePropDestruction(GameObject prop)
        {
            _audioService.PlaySfxByPrefixAuto("Box_Hit", UnityEngine.Random.Range(0.8f, 1.2f));

            LootTableConfig mainLootTableConfig = _configsProviderService.GetConfig<LootTableConfig>();
            _dropLootService.DropLootFor(_entity, mainLootTableConfig);

            // Используем Destroy на самом родителе, если скрипт висит выше
            Object.Destroy(prop);
        }

        private void DealDamageToEntity(Entity targetEntity, Vector2 currentPos, bool inAir)
        {
            float damage = _dashDamage.Value;
            if (inAir) damage *= _airDashMultiplier.Value;

            if (targetEntity.HasComponent<TakeDamageRequest>())
            {
                var damageData = new DamageData { Amount = damage, SourcePosition = currentPos, Type = DamageType.Cut };
                targetEntity.TakeDamageRequest.Invoke(damageData);

                // Опционально: добавь звук удара по врагу
                _audioService.PlaySfxByPrefixAuto("Hit_Enemy", 1f);
            }
        }
    }
}