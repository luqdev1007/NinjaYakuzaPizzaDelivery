using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canDash;

        private ReactiveVariable<bool> _intentDash;
        private ReactiveVariable<bool> _isDashing;
        private ReactiveVariable<bool> _isGrounded;

        private ReactiveVariable<float> _lookDirectionX;

        private ReactiveVariable<float> _dashForceMin;
        private ReactiveVariable<float> _dashForceMax;

        private ReactiveVariable<float> _dashDuration;
        private ReactiveVariable<float> _dashChargeTime;
        private ReactiveVariable<float> _dashCooldown;

        private ReactiveVariable<float> _airDashMultiplier;
        private ReactiveVariable<float> _airDashVerticalBoost;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private const float DashBufferTimeMax = 0.15f;
        private float _dashBufferTimer;

        private float _chargeTimer;
        private float _cooldownTimer;

        private bool _isCharging;
        private bool _wasDashIntendedLastFrame;

        public DashSystem(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _canDash = entity.CanDash;

            _intentDash = entity.IntentDash;
            _isDashing = entity.IsDashing;
            _isGrounded = entity.IsGrounded;

            _lookDirectionX = entity.LookDirectionX;

            _dashForceMin = entity.DashForceMin;
            _dashForceMax = entity.DashForceMax;

            _dashChargeTime = entity.DashChargeTimeMax;
            _dashCooldown = entity.DashCooldown;
            _dashDuration = entity.DashDuration;

            _airDashMultiplier = entity.AirDashMultiplier;
            _airDashVerticalBoost = entity.AirDashVerticalBoost;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            float unscaledDt = Time.unscaledDeltaTime;
            bool currentIntent = _intentDash.Value;
            bool isPressedDown = currentIntent && !_wasDashIntendedLastFrame;
            bool isReleased = !currentIntent && _wasDashIntendedLastFrame;

            _wasDashIntendedLastFrame = currentIntent;

            if (_cooldownTimer > 0f)
                _cooldownTimer -= unscaledDt;

            if (isPressedDown)
                _dashBufferTimer = DashBufferTimeMax;
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
                if (!_canDash.Evaluate())
                {
                    _isCharging = false;
                    return;
                }

                if (currentIntent)
                {
                    _chargeTimer += deltaTime;

                    if (_chargeTimer >= _dashChargeTime.Value)
                    {
                        _chargeTimer = _dashChargeTime.Value;
                        ExecuteDash();
                        return;
                    }
                }
                else if (isReleased)
                {
                    ExecuteDash();
                }
            }
        }

        private void ExecuteDash()
        {
            float chargeRatio = _dashChargeTime.Value > 0f ? _chargeTimer / _dashChargeTime.Value : 1f;
            float force = Mathf.Lerp(_dashForceMin.Value, _dashForceMax.Value, chargeRatio);

            bool inAir = !_isGrounded.Value;

            if (inAir)
                force *= _airDashMultiplier.Value;

            _isDashing.Value = true;
            _cooldownTimer = _dashCooldown.Value;
            _isCharging = false;

            _coroutinesPerformer.StartPerform(DashCoroutine(force, _lookDirectionX.Value, inAir));
        }

        private IEnumerator DashCoroutine(float force, float direction, bool inAir)
        {
            float elapsed = 0f;
            float duration = _dashDuration.Value;

            _rigidbody.linearVelocity = new Vector2(direction * force, inAir ? _airDashVerticalBoost.Value : 0f);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float speedCurve = 1f - t * t;
                float currentSpeed = force * speedCurve;

                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(direction * 2f, _rigidbody.linearVelocity.y);
            _isDashing.Value = false;
        }
    }
}