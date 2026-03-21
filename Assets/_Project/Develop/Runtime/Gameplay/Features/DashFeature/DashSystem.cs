using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly LayerMask _enemyMask;

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

        private const float DashBufferTime = 0.1f;

        public DashSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer, LayerMask enemyMask)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _enemyMask = enemyMask;
        }

        public void OnInit(Entity entity)
        {
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
            if (_cooldownTimer > 0f)
                _cooldownTimer -= deltaTime;

            if (_inputService.IsDashKeyPressed)
                _dashBufferTimer = DashBufferTime;
            else
                _dashBufferTimer -= deltaTime;

            if (_dashBufferTimer > 0f && _canDash.Evaluate() && !_isCharging)
            {
                _isCharging = true;
                _chargeTimer = 0f;
                _dashBufferTimer = 0f;
            }

            if (_isCharging && _inputService.IsDashKeyHeld)
            {
                _chargeTimer = Mathf.Min(
                    _chargeTimer + deltaTime,
                    _dashChargeTime.Value);
            }

            if (_isCharging && _inputService.IsDashKeyReleased)
            {
                if (_canDash.Evaluate())
                    ExecuteDash();
                else
                    _isCharging = false;
            }
        }

        private void ExecuteDash()
        {
            float chargeRatio = _dashChargeTime.Value > 0f
                ? _chargeTimer / _dashChargeTime.Value
                : 1f;

            float force = Mathf.Lerp(
                _dashForceMin.Value,
                _dashForceMax.Value,
                chargeRatio);

            bool inAir = !_isGrounded.Value;

            if (inAir)
                force *= _airDashMultiplier.Value;

            float direction = _transform.localScale.x > 0 ? 1f : -1f;

            _isDashing.Value = true;
            _cooldownTimer = _dashCooldown.Value;
            _isCharging = false;
            _chargeTimer = 0f;

            _coroutinesPerformer.StartPerform(DashCoroutine(force, direction, inAir));
        }

        private IEnumerator DashCoroutine(float force, float direction, bool inAir)
        {
            float elapsed = 0f;
            float duration = _dashDuration.Value;
            float gravityScale = _rigidbody.gravityScale;
            HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

            _rigidbody.gravityScale = 0f;

            if (inAir)
                _rigidbody.linearVelocity = new Vector2(
                    _rigidbody.linearVelocity.x,
                    _airDashVerticalBoost.Value);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(force, 0f, t * t);

                float verticalVelocity = inAir
                    ? Mathf.Lerp(_airDashVerticalBoost.Value, 0f, t)
                    : 0f;

                _rigidbody.linearVelocity = new Vector2(
                    direction * currentSpeed,
                    verticalVelocity);

                ApplyDashHit(hitEnemies);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            _rigidbody.gravityScale = gravityScale;
            _isDashing.Value = false;
        }

        private void ApplyDashHit(HashSet<Collider2D> hitEnemies)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                _transform.position,
                _dashHitboxSize.Value,
                0f,
                _enemyMask);

            foreach (Collider2D hit in hits)
            {
                if (hit == null || !hit.gameObject.activeSelf)
                    continue;

                if (hitEnemies.Contains(hit))
                    continue;

                hitEnemies.Add(hit);
                hit.gameObject.SetActive(false);
            }
        }
    }
}

