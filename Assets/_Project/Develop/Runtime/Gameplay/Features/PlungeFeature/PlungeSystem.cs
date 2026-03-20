using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly LayerMask _enemyMask;

        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _plungeAOERadius;
        private ReactiveVariable<float> _plungeAOEDamage;
        private ReactiveVariable<float> _plungeKnockbackForce;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        public PlungeSystem(IInputService inputService, LayerMask enemyMask)
        {
            _inputService = inputService;
            _enemyMask = enemyMask;
        }

        public void OnInit(Entity entity)
        {
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
                UpdatePlunge();
                return;
            }

            if (_inputService.IsSlideKeyPressed && _canPlunge.Evaluate())
                StartPlunge();
        }

        private void StartPlunge()
        {
            _isPlunging.Value = true;
            _rigidbody.linearVelocity = new Vector2(
                _rigidbody.linearVelocity.x,
                -_plungeSpeed.Value);
        }

        private void UpdatePlunge()
        {
            if (_rigidbody.linearVelocity.y > -_plungeSpeed.Value)
                _rigidbody.linearVelocity = new Vector2(
                    _rigidbody.linearVelocity.x,
                    -_plungeSpeed.Value);

            if (_isGrounded.Value)
            {
                LandPlunge();
                return;
            }

            if (_inputService.IsSlideKeyReleased)
                StopPlunge();
        }

        private void LandPlunge()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                _transform.position,
                _plungeAOERadius.Value,
                _enemyMask);

            foreach (Collider2D hit in hits)
            {
                if (hit == null || !hit.gameObject.activeSelf)
                    continue;

                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    Vector2 knockbackDir = ((Vector2)hit.transform.position - (Vector2)_transform.position).normalized;
                    rb.AddForce(knockbackDir * _plungeKnockbackForce.Value, ForceMode2D.Impulse);
                }
            }

            StopPlunge();
        }

        private void StopPlunge()
        {
            _isPlunging.Value = false;
        }
    }
}