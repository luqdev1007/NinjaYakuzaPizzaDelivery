using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private const float MinSlopeAngle = 15f;
        private const float MaxSlopeAngle = 75f;
        private const float DownhillAccelForce = 8f;
        private const float MaxAccumSpeed = 12f;
        private const float AccumGainRate = 4f;
        private const float UphillSlideForce = 12f;
        private const float MagnetForce = 15f;
        private const float AccumDecayRate = 10f;
        private const float SlideOffDelay = 0.1f;

        private Entity _entity;
        private Rigidbody2D _rigidbody;
        private EntityCollisionProxy _collisionProxy;

        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<bool> _isSliding; // Добавили зависимость
        private ReactiveVariable<float> _slopeAccumSpeed;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private LayerMask _slopeMask;

        private Vector2 _slopeNormal = Vector2.up;
        private bool _contactThisFrame = false;
        private float _slideOffTimer = 0f;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _rigidbody = entity.Rigidbody;
            _isOnSlope = entity.IsOnSlope;
            _isSliding = entity.IsSliding;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;
            _slopeBoostMultiplier = entity.SlopeBoostMultiplier;
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
                if (_isOnSlope.Value && _slopeAccumSpeed.Value > 6f) HandleAutoEject();
                if (_slideOffTimer >= SlideOffDelay && _isOnSlope.Value) ResetSlopeState();
            }
            else _slideOffTimer = 0f;

            if (!_isOnSlope.Value && _slopeAccumSpeed.Value > 0f)
            {
                _slopeAccumSpeed.Value = Mathf.MoveTowards(_slopeAccumSpeed.Value, 0f, AccumDecayRate * deltaTime);
            }

            // ПОВОРОТ ТОЛЬКО В СЛАЙДЕ
            UpdateViewRotation(deltaTime);

            _contactThisFrame = false;
        }

        private void OnCollisionStay(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & _slopeMask) == 0) return;

            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            if (angle < MinSlopeAngle || angle > MaxSlopeAngle) return;

            if (!_isOnSlope.Value && _rigidbody.linearVelocity.y < -3f)
            {
                float impactEnergy = Mathf.Abs(_rigidbody.linearVelocity.y);
                _slopeAccumSpeed.Value = Mathf.Clamp(_slopeAccumSpeed.Value + impactEnergy, 0, MaxAccumSpeed);
            }

            _contactThisFrame = true;
            _isOnSlope.Value = true;
            _slopeNormal = contact.normal;

            // ФИЗИКА СКЛОНА РАБОТАЕТ ТОЛЬКО ЕСЛИ МЫ В СЛАЙДЕ
            if (_isSliding.Value)
            {
                Vector2 downhill = GetDownhill(contact.normal);
                float velX = _rigidbody.linearVelocity.x;

                bool movingDownhill = Mathf.Sign(velX) == Mathf.Sign(downhill.x) && Mathf.Abs(velX) > 0.5f;

                if (movingDownhill)
                {
                    float boost = DownhillAccelForce * _slopeBoostMultiplier.Value;
                    _rigidbody.AddForce(downhill * boost, ForceMode2D.Force);
                    _rigidbody.AddForce(-contact.normal * MagnetForce, ForceMode2D.Force);
                    _slopeAccumSpeed.Value = Mathf.Min(_slopeAccumSpeed.Value + AccumGainRate * Time.fixedDeltaTime, MaxAccumSpeed);
                }
            }
        }

        private void UpdateViewRotation(float deltaTime)
        {
            Transform view = _entity.Transform.Find("ViewContainer");
            if (view == null) return;

            // Если мы скользим — наклоняемся по нормали, если нет — выравниваемся в 0
            float targetZ = _isSliding.Value ? Vector2.SignedAngle(Vector2.up, _slopeNormal) : 0f;
            view.rotation = Quaternion.Lerp(view.rotation, Quaternion.Euler(0f, 0f, targetZ), 0.15f);
        }

        private void HandleAutoEject()
        {
            Vector2 downhill = GetDownhill(_slopeNormal);
            Vector2 ejectDir = (downhill + Vector2.up * 0.5f).normalized;
            _rigidbody.AddForce(ejectDir * (_slopeAccumSpeed.Value * 0.8f), ForceMode2D.Impulse);
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

        public Vector2 SlopeNormal => _slopeNormal;
        public void OnDispose() { if (_collisionProxy != null) _collisionProxy.OnCollisionStayEvent -= OnCollisionStay; }
    }
}