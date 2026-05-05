using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem, IFixedUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private ReactiveVariable<bool> _isGrounded;

        private float _minAngle;
        private float _maxAngle;
        private float _decayRate;
        private float _baseForce;

        private ReactiveVariable<float> _boostMultiplier;
        private ReactiveVariable<float> _magnetForce;
        private ReactiveVariable<float> _maxAccum;
        private ReactiveVariable<float> _gainRate;
        private ReactiveVariable<float> _ejectMult;
        private LayerMask _slopeMask;

        private Vector2 _slopeNormal = Vector2.up;
        private float _defaultGravityScale;

        private const float RayDistance = 1.5f;
        private const float ForwardOffset = 0.4f;

        public Vector2 SlopeNormal => _slopeNormal;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _defaultGravityScale = _rigidbody.gravityScale;

            _isOnSlope = entity.IsOnSlope;
            _isSliding = entity.IsSliding;
            _isGrounded = entity.IsGrounded;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;

            _minAngle = entity.SlopeMinAngle.Value;
            _maxAngle = entity.SlopeMaxAngle.Value;
            _decayRate = entity.SlopeAccumDecayRate.Value;
            _baseForce = entity.SlopeDownhillBaseForce.Value;

            _boostMultiplier = entity.SlopeBoostMultiplier;
            _magnetForce = entity.SlopeMagnetForce;
            _maxAccum = entity.SlopeMaxAccumSpeed;
            _gainRate = entity.SlopeAccumGainRate;
            _ejectMult = entity.SlopeEjectForceMultiplier;
            _slopeMask = entity.SlopeMask;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isOnSlope.Value && _slopeAccumSpeed.Value > 0f)
            {
                _slopeAccumSpeed.Value = Mathf.MoveTowards(_slopeAccumSpeed.Value, 0f, _decayRate * deltaTime);
            }

            if (_isOnSlope.Value && _isSliding.Value && Input.GetButtonDown("Jump"))
            {
                HandleEject(manual: true);
            }
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            CheckSlope();

            if (_isOnSlope.Value && _isSliding.Value)
            {
                ApplySlopePhysics(fixedDeltaTime);
            }
            else
            {
                _rigidbody.gravityScale = _defaultGravityScale;
            }
        }

        private void CheckSlope()
        {
            RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, RayDistance, _slopeMask);

            if (hit.collider != null)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle >= _minAngle && angle <= _maxAngle)
                {
                    _isOnSlope.Value = true;
                    _slopeNormal = hit.normal;
                    return;
                }
            }

            if (_isOnSlope.Value)
            {
                HandleEject(manual: false);
            }
        }

        private void ApplySlopePhysics(float dt)
        {
            _rigidbody.gravityScale = 0f;

            Vector2 downhill = GetDownhill(_slopeNormal);
            float moveDir = Mathf.Sign(_rigidbody.linearVelocity.x);
            Vector2 targetVelocity = downhill * (_baseForce + _slopeAccumSpeed.Value);

            Vector2 magnet = -_slopeNormal * _magnetForce.Value;

            _rigidbody.linearVelocity = Vector2.Lerp(_rigidbody.linearVelocity, targetVelocity + magnet, dt * 10f);

            if (Mathf.Sign(downhill.x) == moveDir || Mathf.Abs(_rigidbody.linearVelocity.x) < 0.1f)
            {
                _slopeAccumSpeed.Value = Mathf.MoveTowards(_slopeAccumSpeed.Value, _maxAccum.Value, _gainRate.Value * dt);
            }
        }

        private void HandleEject(bool manual)
        {
            Vector2 downhill = GetDownhill(_slopeNormal);

            Vector2 ejectDir = manual
                ? (downhill + Vector2.up * 1.2f).normalized
                : (downhill + Vector2.up * 0.4f).normalized;

            float power = _slopeAccumSpeed.Value * _ejectMult.Value;

            power = Mathf.Max(power, 5f);

            _rigidbody.gravityScale = _defaultGravityScale;
            _rigidbody.AddForce(ejectDir * power, ForceMode2D.Impulse);

            ResetSlopeState();
        }

        private void ResetSlopeState()
        {
            _isOnSlope.Value = false;
            _isSliding.Value = false;
            _slopeAccumSpeed.Value = 0f;
            _slopeNormal = Vector2.up;
            _rigidbody.gravityScale = _defaultGravityScale;
        }

        private Vector2 GetDownhill(Vector2 normal)
        {
            Vector2 downhill = new Vector2(normal.y, -normal.x);

            if (downhill.y > 0f) 
                downhill = -downhill;

            return downhill;
        }
    }
}