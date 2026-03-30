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
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly LayerMask _enemyMask;
        private readonly AudioService _audioService;

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

        private const float DashBufferTime = 0.15f;

        public DashSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer, LayerMask enemyMask, AudioService audioService)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _enemyMask = enemyMask;
            _audioService = audioService;
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
        }

        public void OnUpdate(float deltaTime)
        {
            // Используем unscaledDeltaTime, чтобы кулдаун шел даже во время хит-стопа
            float unscaledDt = Time.unscaledDeltaTime;

            if (_cooldownTimer > 0f)
                _cooldownTimer -= unscaledDt;

            if (_inputService.IsDashKeyPressed)
                _dashBufferTimer = DashBufferTime;
            else if (_dashBufferTimer > 0f)
                _dashBufferTimer -= unscaledDt;

            // Проверка возможности рывка
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
            // 1. Считаем силу заряда (0.0 - 1.0)
            float chargeRatio = _dashChargeTime.Value > 0f ? _chargeTimer / _dashChargeTime.Value : 1f;
            float force = Mathf.Lerp(_dashForceMin.Value, _dashForceMax.Value, chargeRatio);

            // 2. Считаем питч: от 1.0 (слабый) до 1.3 (максимальный заряд)
            // Это даст ощущение "мощности" без ухода в ультразвук
            float dashPitch = 1f + (chargeRatio * 0.3f);

            // 3. Воспроизводим одну из 5 вариаций "AbilityImpactCharge" (1, 2, 3, 4 или 5)
            _audioService.PlaySfxVariation("AbilityImpactCharge", 1, 5, dashPitch);

            // Дальнейшая логика рывка...
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
            float elapsed = 0f;
            float duration = _dashDuration.Value;
            float gravityScale = _rigidbody.gravityScale;
            HashSet<Entity> hitEntities = new HashSet<Entity>();

            _rigidbody.gravityScale = 0f;

            // Начальный импульс
            Vector2 dashVelocity = new Vector2(direction * force, inAir ? _airDashVerticalBoost.Value : 0f);
            _rigidbody.linearVelocity = dashVelocity;

            while (elapsed < duration)
            {
                // Для плавности можно оставить затухание, но если "пролетает мимо", 
                // лучше держать скорость чуть дольше
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(force, force * 0.2f, t);

                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                ApplyDashDamage(hitEntities, inAir);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(direction * 2f, _rigidbody.linearVelocity.y);
            _rigidbody.gravityScale = gravityScale;
            _isDashing.Value = false;
        }

        private void ApplyDashDamage(HashSet<Entity> hitEntities, bool inAir)
        {
            Vector2 checkPos = (Vector2)_transform.position;
            Collider2D[] hits = Physics2D.OverlapBoxAll(checkPos, _dashHitboxSize.Value, 0f, _enemyMask);

            int newHitsInThisFrame = 0;

            foreach (Collider2D hit in hits)
            {
                if (hit == null) continue;

                var mono = hit.GetComponentInParent<MonoEntity>();
                if (mono == null) continue;

                Entity target = mono.LinkedEntity;
                if (hitEntities.Contains(target)) continue;

                // Нашли новую цель
                hitEntities.Add(target);
                newHitsInThisFrame++;

                float damage = _dashDamage.Value;
                if (inAir) damage *= _airDashMultiplier.Value;

                if (target.HasComponent<TakeDamageRequest>())
                {
                    var damageData = new DamageData { Amount = damage, SourcePosition = checkPos };
                    target.TakeDamageRequest.Invoke(damageData);
                }
            }

            // Если в этом кадре задели кого-то, запускаем очередь звуков
            if (newHitsInThisFrame > 0)
            {
                _coroutinesPerformer.StartPerform(PlayHitSoundsSequence(newHitsInThisFrame));
            }
        }

        private IEnumerator PlayHitSoundsSequence(int count)
        {
            // Чем больше целей, тем выше может быть прогрессия питча, 
            // чтобы создать эффект "нарастания" или просто хаоса
            for (int i = 0; i < count; i++)
            {
                // Базовый высокий питч для Dash (например 1.4f) 
                // + небольшая случайность, чтобы звуки отличались
                float pitch = 1.4f + UnityEngine.Random.Range(-0.1f, 0.1f);

                _audioService.PlayRandomSfx(AudioCategoryType.HeroAttackHit, true, pitch);

                // Ждем крошечное количество времени (0.02 - 0.05 сек) 
                // Этого достаточно, чтобы ухо различило отдельные удары
                if (count > 1)
                    yield return new WaitForSecondsRealtime(0.03f);
            }
        }

        // Добавим маленький локальный хит-стоп для Dash, если нужно
        private IEnumerator DoDashHitStop()
        {
            float originalScale = Time.timeScale;
            Time.timeScale = 0.1f; // Не такой жесткий, как в мели
            yield return new WaitForSecondsRealtime(0.05f);
            Time.timeScale = originalScale;
        }
    }
}