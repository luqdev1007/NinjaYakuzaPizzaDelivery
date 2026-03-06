using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class DashSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canDash;
        private ReactiveVariable<bool> _isDashing;
        private ReactiveVariable<float> _dashForceMin;
        private ReactiveVariable<float> _dashForceMax;
        private ReactiveVariable<float> _dashChargeTime;
        private ReactiveVariable<float> _dashCooldown;
        private ReactiveVariable<float> _dashDuration;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private float _chargeTimer;
        private float _cooldownTimer;
        private bool _isCharging;

        public DashSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _canDash = entity.CanDash;
            _isDashing = entity.IsDashing;
            _dashForceMin = entity.DashForceMin;
            _dashForceMax = entity.DashForceMax;
            _dashChargeTime = entity.DashChargeTime;
            _dashCooldown = entity.DashCooldown;
            _dashDuration = entity.DashDuration;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= deltaTime;
                return;
            }

            if (_inputService.IsDashKeyPressed && _canDash.Evaluate() && !_isCharging)
            {
                _isCharging = true;
                _chargeTimer = 0f;
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

            float direction = _transform.localScale.x > 0 ? 1f : -1f;

            _isDashing.Value = true;
            _cooldownTimer = _dashCooldown.Value;
            _isCharging = false;
            _chargeTimer = 0f;

            _coroutinesPerformer.StartPerform(DashCoroutine(force, direction));
        }

        private IEnumerator DashCoroutine(float force, float direction)
        {
            float elapsed = 0f;
            float duration = _dashDuration.Value;
            float gravityScale = _rigidbody.gravityScale;

            _rigidbody.gravityScale = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(force, 0f, t * t);

                _rigidbody.linearVelocity = new Vector2(
                    direction * currentSpeed,
                    0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            _rigidbody.gravityScale = gravityScale;
            _isDashing.Value = false;
        }
    }
}