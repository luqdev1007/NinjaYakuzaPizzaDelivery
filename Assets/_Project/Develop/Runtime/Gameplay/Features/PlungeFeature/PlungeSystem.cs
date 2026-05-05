using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _plungeActive;

        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _plungeAOERadius;
        private ReactiveVariable<float> _plungeAOEDamage;
        private ReactiveVariable<float> _plungeKnockbackForce;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private LayerMask _enemyMask;

        private float _currentFlightTime;

        private const float FlightCheckWidth = 1.5f;
        private const float FlightCheckHeight = 1.0f;
        private const float FlightKnockbackMultiplier = 0.3f;
        private const float BaseChargeTime = 0.5f;
        private const float MaxDamageMultiplier = 2.5f;
        private const float MaxRadiusMultiplier = 1.4f;

        public void OnInit(Entity entity)
        {
            _plungeActive = entity.PlungeActive;
            _enemyMask = entity.AttackEnemyMask.Value;

            _canPlunge = entity.CanPlunge;
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;
            _plungeSpeed = entity.PlungeSpeed;
            _plungeAOERadius = entity.PlungeAOERadius;
            _plungeAOEDamage = entity.PlungeAOEDamage;
            _plungeKnockbackForce = entity.PlungeKnockbackForce;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isPlunging.Value)
            {
                _currentFlightTime += deltaTime;
                UpdatePlunge();
                return;
            }

            if (_plungeActive.Value && _canPlunge.Evaluate())
            {
                StartPlunge();
            }
        }

        private void StartPlunge()
        {
            _isPlunging.Value = true;
            _currentFlightTime = 0f;
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.5f, -_plungeSpeed.Value);
        }

        private void UpdatePlunge()
        {
            if (_rigidbody.linearVelocity.y > -_plungeSpeed.Value)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, -_plungeSpeed.Value);
            }

            ApplyFlightDamage();

            if (_isGrounded.Value)
            {
                LandPlunge();
                return;
            }

            if (!_plungeActive.Value)
            {
                StopPlunge();
            }
        }

        private void ApplyFlightDamage()
        {
            Vector2 checkPos = (Vector2)_transform.position + Vector2.down * 0.5f;
            Collider2D[] hits = Physics2D.OverlapBoxAll(checkPos, new Vector2(FlightCheckWidth, FlightCheckHeight), 0, _enemyMask);

            foreach (var hit in hits)
            {
                ExecuteHit(hit, _plungeAOEDamage.Value * 0.5f, _plungeKnockbackForce.Value * FlightKnockbackMultiplier);
            }
        }

        private void LandPlunge()
        {
            float intensityRatio = Mathf.Clamp(_currentFlightTime / BaseChargeTime, 0f, 1.0f);

            float damageMultiplier = Mathf.Lerp(1.0f, MaxDamageMultiplier, intensityRatio);
            float radiusMultiplier = Mathf.Lerp(1.0f, MaxRadiusMultiplier, intensityRatio);

            float finalDamage = _plungeAOEDamage.Value * damageMultiplier;
            float finalRadius = _plungeAOERadius.Value * radiusMultiplier;
            float finalForce = _plungeKnockbackForce.Value * damageMultiplier;

            Collider2D[] hits = Physics2D.OverlapCircleAll(_transform.position, finalRadius, _enemyMask);

            foreach (Collider2D hit in hits)
            {
                ExecuteHit(hit, finalDamage, finalForce);
            }

            StopPlunge();
        }

        private void ExecuteHit(Collider2D hit, float damage, float force)
        {
            if (hit == null || !hit.gameObject.activeSelf)
                return;

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = ((Vector2)hit.transform.position - (Vector2)_transform.position);
                direction.y += 0.5f;
                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            }

            var monoEntity = hit.GetComponentInParent<MonoEntity>();

            if (monoEntity != null)
            {
                var target = monoEntity.LinkedEntity;

                target.TakeDamageEvent?.Invoke(new DamageData
                {
                    Amount = damage,
                    SourcePosition = _transform.position
                });
            }
        }

        private void StopPlunge()
        {
            _isPlunging.Value = false;
            _currentFlightTime = 0f;
            _plungeActive.Value = false;
        }
    }
}