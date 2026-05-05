using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private InputState _dashInput;
        private ICompositeCondition _canDash;
        private ReactiveVariable<bool> _isDashing;
        private ReactiveVariable<bool> _isGrounded;

        private DashFeature.CometDashStateComponent _cometState;

        private ReactiveVariable<float> _dashForceMin;
        private ReactiveVariable<float> _dashForceMax;
        private ReactiveVariable<float> _dashChargeTime;
        private ReactiveVariable<float> _dashDuration;
        private ReactiveVariable<float> _airDashVerticalBoost;
        private ReactiveVariable<float> _dashDamage;
        private ReactiveVariable<Vector2> _dashHitboxSize;
        private LayerMask _enemyMask;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _chargeTimer;
        private float _dashBufferTimer;
        private bool _isCharging;

        private const float DashBufferTime = 0.15f;

        public DashSystem(ICoroutinesPerformer coroutinesPerformer) =>
            _coroutinesPerformer = coroutinesPerformer;

        public void OnInit(Entity entity)
        {
            _dashInput = entity.DashInput;
            _canDash = entity.CanDash;
            _isDashing = entity.IsDashing;
            _isGrounded = entity.IsGrounded;

            _cometState = entity.GetComponent<DashFeature.CometDashStateComponent>();

            _dashForceMin = entity.DashForceMin;
            _dashForceMax = entity.DashForceMax;
            _dashChargeTime = entity.DashChargeTime;
            _dashDuration = entity.DashDuration;
            _airDashVerticalBoost = entity.AirDashVerticalBoost;
            _dashDamage = entity.DashDamage;
            _dashHitboxSize = entity.DashHitboxSize;
            _enemyMask = entity.AttackEnemyMask.Value;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            HandleInputBuffer(deltaTime);

            if (_dashBufferTimer > 0f && CanStartCharging())
                StartCharging();

            if (_isCharging)
                UpdateCharging(deltaTime);
        }

        private bool CanStartCharging() =>
            _canDash.Evaluate() && !_isCharging && _cometState.CurrentCharges.Value > 0;

        private void StartCharging()
        {
            _isCharging = true;
            _chargeTimer = 0f;
            _dashBufferTimer = 0f;
        }

        private void UpdateCharging(float deltaTime)
        {
            if (_dashInput.IsHeld.Value)
                _chargeTimer = Mathf.Min(_chargeTimer + deltaTime, _dashChargeTime.Value);

            if (_dashInput.IsReleased.Value)
            {
                if (_canDash.Evaluate() && _cometState.CurrentCharges.Value > 0)
                    ExecuteDash();
                else
                    _isCharging = false;
            }
        }

        private void ExecuteDash()
        {
            float chargeRatio = _dashChargeTime.Value > 0f ? _chargeTimer / _dashChargeTime.Value : 1f;

            float baseForce = Mathf.Lerp(_dashForceMin.Value, _dashForceMax.Value, chargeRatio);
            float finalForce = baseForce * _cometState.CurrentMultiplier.Value;

            bool inAir = !_isGrounded.Value;
            float direction = Mathf.Sign(_transform.localScale.x);

            // Тратим ресурс
            _cometState.CurrentCharges.Value--;
            _cometState.CurrentMultiplier.Value = Mathf.Max(0.5f, _cometState.CurrentMultiplier.Value - _cometState.Config.MultiplierDegradation);

            // Сбрасываем таймер в Recovery системе через стейт
            _cometState.CooldownTimer.Value = _cometState.Config.BaseCooldown;

            _isDashing.Value = true;
            _isCharging = false;

            _coroutinesPerformer.StartPerform(DashCoroutine(finalForce, direction, inAir));
        }

        private IEnumerator DashCoroutine(float force, float direction, bool inAir)
        {
            Physics2D.IgnoreLayerCollision(LayersAPI.LayerCharacters, LayersAPI.LayerEnemies, true);
            float originalGravity = _rigidbody.gravityScale;

            try
            {
                _rigidbody.gravityScale = 0f;
                float elapsed = 0f;
                float duration = _dashDuration.Value;
                HashSet<Entity> hitEntities = new HashSet<Entity>();
                Vector2 lastFramePos = _transform.position;

                _rigidbody.linearVelocity = new Vector2(direction * force, inAir ? _airDashVerticalBoost.Value : 0f);

                while (elapsed < duration)
                {
                    elapsed += Time.fixedDeltaTime;
                    float t = elapsed / duration;

                    float speedCurve = Mathf.Cos(t * Mathf.PI * 0.5f);
                    float currentSpeed = force * speedCurve;

                    _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, inAir ? _rigidbody.linearVelocity.y : 0f);

                    ApplyDashDamage(hitEntities, inAir, lastFramePos);
                    lastFramePos = _transform.position;

                    yield return new WaitForFixedUpdate();
                }
            }
            finally
            {
                _rigidbody.gravityScale = originalGravity;
                _isDashing.Value = false;
                Physics2D.IgnoreLayerCollision(LayersAPI.LayerCharacters, LayersAPI.LayerEnemies, false);

                _rigidbody.linearVelocity = new Vector2(direction * 2f, _rigidbody.linearVelocity.y);
            }
        }

        private void ApplyDashDamage(HashSet<Entity> hitEntities, bool inAir, Vector2 lastPosition)
        {
            Vector2 currentPos = _transform.position;
            float direction = Mathf.Sign(_transform.localScale.x);

            Vector2 hitboxOffset = new Vector2(direction * (_dashHitboxSize.Value.x * 0.5f), 0f);
            Vector2 origin = lastPosition + hitboxOffset;
            Vector2 target = currentPos + hitboxOffset;

            float distance = Vector2.Distance(origin, target);
            Vector2 castDir = distance > 0.01f ? (target - origin).normalized : Vector2.right * direction;

            RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, _dashHitboxSize.Value, 0f, castDir, distance, _enemyMask);

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                if (hit.collider.TryGetComponent<MonoEntity>(out var mono))
                {
                    Entity targetEntity = mono.LinkedEntity;
                    if (targetEntity != null && !hitEntities.Contains(targetEntity))
                    {
                        hitEntities.Add(targetEntity);
                        DealDamageToEntity(targetEntity, currentPos, inAir);
                    }
                }
            }
        }

        private void DealDamageToEntity(Entity targetEntity, Vector2 currentPos, bool inAir)
        {
            float damage = _dashDamage.Value * _cometState.CurrentMultiplier.Value;
            if (inAir) damage *= 1.2f;

            if (targetEntity.HasComponent<TakeDamageRequest>())
            {
                var damageData = new DamageData
                {
                    Amount = damage,
                    SourcePosition = currentPos,
                    Type = DamageType.Cut
                };
                targetEntity.TakeDamageRequest.Invoke(damageData);
            }
        }

        private void HandleInputBuffer(float deltaTime)
        {
            if (_dashInput.IsPressed.Value) _dashBufferTimer = DashBufferTime;
            else if (_dashBufferTimer > 0f) _dashBufferTimer -= deltaTime;
        }
    }
}