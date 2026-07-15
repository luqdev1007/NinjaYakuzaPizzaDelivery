using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Common;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash
{
    public class DashSystem : IInitializableSystem, IFixedUpdatableSystem
    {
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

        // Окно движения дэша: бывшая DashCoroutine, развёрнутая в state-машину на
        // физ-тике. Параметры фиксируются на старте (корутина захватывала их так же).
        private bool _isInDashWindow;
        private float _dashWindowElapsed;
        private float _dashWindowDuration;
        private float _dashForce;
        private float _dashDirection;

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

        public void OnFixedUpdate(float deltaTime)
        {
            // Кулдаун и буфер сохраняют иммунитет к хитстопу (Time.timeScale):
            // fixedUnscaledDeltaTime — прямой fixed-эквивалент unscaledDeltaTime.
            float unscaledDt = Time.fixedUnscaledDeltaTime;
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
                if (_canDash.Evaluate() == false)
                {
                    _isCharging = false;
                }
                else if (currentIntent)
                {
                    _chargeTimer += deltaTime;

                    if (_chargeTimer >= _dashChargeTime.Value)
                    {
                        _chargeTimer = _dashChargeTime.Value;
                        ExecuteDash();
                    }
                }
                else if (isReleased)
                {
                    ExecuteDash();
                }
            }

            // Окно прокручивается в том же тике, где стартовало: корутина
            // выполнялась синхронно до первого yield, т.е. её первый проход тела
            // цикла (t=0) и первый инкремент elapsed приходились на кадр старта.
            if (_isInDashWindow)
                AdvanceDashWindow(deltaTime);
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

            _dashForce = force;
            _dashDirection = _lookDirectionX.Value;
            _dashWindowDuration = _dashDuration.Value;
            _dashWindowElapsed = 0f;
            _isInDashWindow = true;

            // Пре-луповая запись корутины: вертикальный буст задаётся один раз на
            // старте, дальше окно ведёт только X и сохраняет Y.
            _rigidbody.linearVelocity = new Vector2(_dashDirection * force, inAir ? _airDashVerticalBoost.Value : 0f);
        }

        private void AdvanceDashWindow(float deltaTime)
        {
            if (_dashWindowElapsed < _dashWindowDuration)
            {
                float t = _dashWindowElapsed / _dashWindowDuration;
                float speedCurve = 1f - t * t;
                float currentSpeed = _dashForce * speedCurve;

                _rigidbody.linearVelocity = new Vector2(_dashDirection * currentSpeed, _rigidbody.linearVelocity.y);

                _dashWindowElapsed += deltaTime;

                return;
            }

            EndDashWindow();
        }

        private void EndDashWindow()
        {
            _isInDashWindow = false;

            _rigidbody.linearVelocity = new Vector2(_dashDirection * 2f, _rigidbody.linearVelocity.y);
            _isDashing.Value = false;
        }
    }
}
