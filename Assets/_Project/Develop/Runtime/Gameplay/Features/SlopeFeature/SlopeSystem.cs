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
        private const float DownhillAccelForce = 10f;
        private const float MaxAccumSpeed = 12f;
        private const float AccumGainRate = 4f;
        private const float MagnetForce = 15f;
        private const float AccumDecayRate = 10f;
        private const float SlideOffDelay = 0.1f;

        private Rigidbody2D _rigidbody;
        private EntityCollisionProxy _collisionProxy;

        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private LayerMask _slopeMask;

        private Vector2 _slopeNormal = Vector2.up;
        private bool _contactThisFrame = false;
        private float _slideOffTimer = 0f;

        public Vector2 SlopeNormal => _slopeNormal;

        public void OnInit(Entity entity)
        {
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
            // Проверка потери контакта со склоном
            if (!_contactThisFrame)
            {
                _slideOffTimer += deltaTime;

                // Если мы вылетели со склона на большой скорости — даем импульс (эффект трамплина)
                if (_isOnSlope.Value && _slopeAccumSpeed.Value > 6f) HandleAutoEject();

                if (_slideOffTimer >= SlideOffDelay && _isOnSlope.Value) ResetSlopeState();
            }
            else _slideOffTimer = 0f;

            // Затухание накопленной скорости, если мы не на склоне
            if (!_isOnSlope.Value && _slopeAccumSpeed.Value > 0f)
            {
                _slopeAccumSpeed.Value = Mathf.MoveTowards(_slopeAccumSpeed.Value, 0f, AccumDecayRate * deltaTime);
            }

            _contactThisFrame = false;
        }

        private void OnCollisionStay(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & _slopeMask) == 0) return;

            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            if (angle < MinSlopeAngle || angle > MaxSlopeAngle) return;

            // Накопление энергии при приземлении на склон (Impact)
            if (!_isOnSlope.Value && _rigidbody.linearVelocity.y < -3f)
            {
                float impactEnergy = Mathf.Abs(_rigidbody.linearVelocity.y);
                _slopeAccumSpeed.Value = Mathf.Clamp(_slopeAccumSpeed.Value + impactEnergy, 0, MaxAccumSpeed);
            }

            _contactThisFrame = true;
            _isOnSlope.Value = true;
            _slopeNormal = contact.normal;

            // Физика ускорения работает только если игрок нажал "Слайд"
            if (_isSliding.Value)
            {
                ApplySlopePhysics(contact.normal);
            }
        }

        private void ApplySlopePhysics(Vector2 normal)
        {
            Vector2 downhill = GetDownhill(normal);
            float velX = _rigidbody.linearVelocity.x;

            // Проверяем, движемся ли мы вниз по склону
            bool movingDownhill = Mathf.Sign(velX) == Mathf.Sign(downhill.x) && Mathf.Abs(velX) > 0.5f;

            if (movingDownhill)
            {
                float boost = DownhillAccelForce * _slopeBoostMultiplier.Value;
                _rigidbody.AddForce(downhill * boost, ForceMode2D.Force);

                // Прижимаем к поверхности, чтобы не терять контакт на перегибах
                _rigidbody.AddForce(-normal * MagnetForce, ForceMode2D.Force);

                // Накапливаем бонусную скорость для SlideSystem
                _slopeAccumSpeed.Value = Mathf.Min(_slopeAccumSpeed.Value + AccumGainRate * Time.fixedDeltaTime, MaxAccumSpeed);
            }
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

        public void OnDispose()
        {
            if (_collisionProxy != null) _collisionProxy.OnCollisionStayEvent -= OnCollisionStay;
        }
    }
}