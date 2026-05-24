using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canDash;

        private ReactiveVariable<bool> _isDashing;
        private ReactiveVariable<bool> _isGrounded;

        private ReactiveVariable<float> _dashForceMin;
        private ReactiveVariable<float> _dashForceMax;

        private ReactiveVariable<float> _dashChargeTime;
        private ReactiveVariable<float> _dashCooldown;
        private ReactiveVariable<float> _dashDuration;

        private ReactiveVariable<bool> _intentDash;

        private ReactiveVariable<float> _airDashMultiplier;
        private ReactiveVariable<float> _airDashVerticalBoost;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _chargeTimer;
        private float _cooldownTimer;
        private float _dashBufferTimer;
        private bool _isCharging;
        private bool _wasDashIntendedLastFrame;

        private const float DashBufferTime = 0.1f;

        public DashSystem(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
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

            _intentDash = entity.IntentDash;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

        }

        public void OnUpdate(float deltaTime)
        {
            bool isDashIntented = _intentDash.Value;
            bool isDashReleased = !isDashIntented && _wasDashIntendedLastFrame;

            _wasDashIntendedLastFrame = isDashIntented;

            if (isDashIntented)
            {
                ExecuteDash();
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

            //_rigidbody.gravityScale = 0f;

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

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            //_rigidbody.gravityScale = gravityScale;
            _isDashing.Value = false;
        }
    }
}
