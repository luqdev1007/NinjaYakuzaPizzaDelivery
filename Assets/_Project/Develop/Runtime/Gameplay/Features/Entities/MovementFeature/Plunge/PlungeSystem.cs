using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Plunge
{
    public class PlungeSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float AccelerationRampPerSecond = 2.0f; // config
        private const float MaxTerminalSpeedFactor = 2.5f; // config

        private Entity _entity;
        private ReactiveVariable<bool> _intentPlunge;
        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _minImpactSpeedThreshold;
        private ReactiveVariable<float> _accelerationMultiplier;
        private ReactiveVariable<bool> _isGrounded;
        private Rigidbody2D _rigidbody;

        private float _lastVerticalSpeed;
        private float _plungeTime; 
        private float _originalGravityScale; 

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _canPlunge = entity.CanPlunge;
            _isPlunging = entity.IsPlunging;
            _plungeSpeed = entity.PlungeSpeed;
            _intentPlunge = entity.IntentSlide;
            _isGrounded = entity.IsGrounded;
            _minImpactSpeedThreshold = entity.MinPlungeImpactSpeedThreshold;
            _accelerationMultiplier = entity.PlungeAccelerationMultiplier;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isPlunging.Value)
            {
                UpdatePlunge(deltaTime);
                return;
            }

            if (_intentPlunge.Value && _canPlunge.Evaluate())
            {
                StartPlunge();
            }
        }

        private void StartPlunge()
        {
            _isPlunging.Value = true;
            _plungeTime = 0f; 
            _lastVerticalSpeed = Mathf.Abs(_rigidbody.linearVelocity.y);

            _originalGravityScale = _rigidbody.gravityScale;
            _rigidbody.gravityScale = 0f;

            float startingY = Mathf.Min(_rigidbody.linearVelocity.y, -2f);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.5f, startingY);
        }

        private void UpdatePlunge(float deltaTime)
        {
            _plungeTime += deltaTime; 

            float baseAcceleration = _plungeSpeed.Value * _accelerationMultiplier.Value;
            float rampedAcceleration = baseAcceleration * (1f + AccelerationRampPerSecond * _plungeTime);

            float newVelocityY = _rigidbody.linearVelocity.y - rampedAcceleration * deltaTime;

            float terminalSpeed = _plungeSpeed.Value * MaxTerminalSpeedFactor;
            newVelocityY = Mathf.Max(newVelocityY, -terminalSpeed);

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, newVelocityY);

            _lastVerticalSpeed = Mathf.Abs(_rigidbody.linearVelocity.y);

            if (_isGrounded.Value)
            {
                HandleLanding();
                StopPlunge();
                return;
            }

            if (!_intentPlunge.Value)
            {
                StopPlunge();
            }
        }

        private void HandleLanding()
        {
            if (_lastVerticalSpeed >= _minImpactSpeedThreshold.Value)
            {
                _entity.PlungeImpactEvent?.Invoke(_lastVerticalSpeed);
            }
        }

        private void StopPlunge()
        {
            _isPlunging.Value = false;
            _lastVerticalSpeed = 0f;
            _plungeTime = 0f; 

            _rigidbody.gravityScale = _originalGravityScale; 
        }
    }
}