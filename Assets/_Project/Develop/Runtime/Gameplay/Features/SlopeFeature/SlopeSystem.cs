using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private Rigidbody2D _rigidbody;
        private EntityCollisionProxy _collisionProxy;

        // Состояние
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _slopeAccumSpeed;

        // Параметры (теперь всё из компонентов)
        private float _minAngle;
        private float _maxAngle;
        private float _decayRate;
        private float _offDelay;
        private float _baseForce;
        private ReactiveVariable<float> _boostMultiplier;
        private ReactiveVariable<float> _magnetForce;
        private ReactiveVariable<float> _maxAccum;
        private ReactiveVariable<float> _gainRate;
        private ReactiveVariable<float> _minEjectVel;
        private ReactiveVariable<float> _ejectMult;
        private LayerMask _slopeMask;

        private Vector2 _slopeNormal = Vector2.up;
        private bool _contactThisFrame = false;
        private float _slideOffTimer = 0f;

        public Vector2 SlopeNormal => _slopeNormal;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;

            // Состояние
            _isOnSlope = entity.IsOnSlope;
            _isSliding = entity.IsSliding;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;

            // Чистые значения (если они не меняются в рантайме, можно брать .Value или передавать как float)
            _minAngle = entity.SlopeMinAngle.Value;
            _maxAngle = entity.SlopeMaxAngle.Value;
            _decayRate = entity.SlopeAccumDecayRate.Value;
            _offDelay = entity.SlopeSlideOffDelay.Value;
            _baseForce = entity.SlopeDownhillBaseForce.Value;

            // Реактивные параметры (для настройки в инспекторе на лету)
            _boostMultiplier = entity.SlopeBoostMultiplier;
            _magnetForce = entity.SlopeMagnetForce;
            _maxAccum = entity.SlopeMaxAccumSpeed;
            _gainRate = entity.SlopeAccumGainRate;
            _minEjectVel = entity.SlopeMinEjectVelocity;
            _ejectMult = entity.SlopeEjectForceMultiplier;
            _slopeMask = entity.SlopeMask;

            _collisionProxy = entity.Transform.GetComponent<EntityCollisionProxy>();
            if (_collisionProxy != null)
                _collisionProxy.OnCollisionStayEvent += OnCollisionStay;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_contactThisFrame)
            {
                _slideOffTimer += deltaTime;

                if (_isOnSlope.Value && _slopeAccumSpeed.Value > _minEjectVel.Value)
                    HandleAutoEject();

                if (_slideOffTimer >= _offDelay && _isOnSlope.Value)
                    ResetSlopeState();
            }
            else _slideOffTimer = 0f;

            if (!_isOnSlope.Value && _slopeAccumSpeed.Value > 0f)
            {
                _slopeAccumSpeed.Value = Mathf.MoveTowards(_slopeAccumSpeed.Value, 0f, _decayRate * deltaTime);
            }

            _contactThisFrame = false;
        }

        private void OnCollisionStay(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & _slopeMask) == 0) return;

            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            if (angle < _minAngle || angle > _maxAngle) return;

            // Импакт при приземлении
            if (!_isOnSlope.Value && _rigidbody.linearVelocity.y < -3f)
            {
                float impactEnergy = Mathf.Abs(_rigidbody.linearVelocity.y);
                _slopeAccumSpeed.Value = Mathf.Clamp(_slopeAccumSpeed.Value + impactEnergy, 0, _maxAccum.Value);
            }

            _contactThisFrame = true;
            _isOnSlope.Value = true;
            _slopeNormal = contact.normal;

            if (_isSliding.Value) ApplySlopePhysics(contact.normal);
        }

        private void ApplySlopePhysics(Vector2 normal)
        {
            Vector2 downhill = GetDownhill(normal);
            float velX = _rigidbody.linearVelocity.x;
            bool movingDownhill = Mathf.Sign(velX) == Mathf.Sign(downhill.x) && Mathf.Abs(velX) > 0.5f;

            if (movingDownhill)
            {
                _rigidbody.AddForce(downhill * (_baseForce * _boostMultiplier.Value), ForceMode2D.Force);
                _rigidbody.AddForce(-normal * _magnetForce.Value, ForceMode2D.Force);

                _slopeAccumSpeed.Value = Mathf.Min(
                    _slopeAccumSpeed.Value + _gainRate.Value * Time.fixedDeltaTime,
                    _maxAccum.Value
                );
            }
        }

        private void HandleAutoEject()
        {
            Vector2 downhill = GetDownhill(_slopeNormal);
            Vector2 ejectDir = (downhill + Vector2.up * 0.5f).normalized;
            _rigidbody.AddForce(ejectDir * (_slopeAccumSpeed.Value * _ejectMult.Value), ForceMode2D.Impulse);

            ResetSlopeState();
            _slopeAccumSpeed.Value = 0f;
        }

        private void ResetSlopeState()
        {
            _isOnSlope.Value = false;
            _slopeNormal = Vector2.up;
        }

        private Vector2 GetDownhill(Vector2 normal)
        {
            Vector2 downhill = new Vector2(normal.y, -normal.x);
            if (downhill.y > 0f) downhill = -downhill;
            return downhill;
        }

        public void OnDispose()
        {
            if (_collisionProxy != null) _collisionProxy.OnCollisionStayEvent -= OnCollisionStay;
        }
    }
}