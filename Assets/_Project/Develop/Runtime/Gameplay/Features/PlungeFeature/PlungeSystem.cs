using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly LayerMask _enemyMask;
        private readonly AudioService _audioService;

        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _plungeAOERadius;
        private ReactiveVariable<float> _plungeAOEDamage;
        private ReactiveVariable<float> _plungeKnockbackForce;
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private string _activeLoopId;

        private const float FlightCheckWidth = 1.5f;
        private const float FlightCheckHeight = 1.0f;
        private const float FlightKnockbackMultiplier = 0.3f;

        public PlungeSystem(IInputService inputService, LayerMask enemyMask, AudioService audioService)
        {
            _inputService = inputService;
            _enemyMask = enemyMask;
            _audioService = audioService;
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
            // Гвардия: Если мы не в состоянии пикирования, но звук зациклен — принудительно выключаем.
            // Это лечит баги, когда ввод прерывается или состояние сбрасывается извне.
            if (!_isPlunging.Value && !string.IsNullOrEmpty(_activeLoopId))
            {
                ClearPlungeLoop();
            }

            if (_isPlunging.Value)
            {
                UpdatePlunge(deltaTime);
                return;
            }

            if (_inputService.IsSlideKeyPressed && _canPlunge.Evaluate())
                StartPlunge();
        }

        private void UpdatePlunge(float deltaTime)
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

            if (_inputService.IsSlideKeyReleased)
                StopPlunge();
        }

        private void ApplyFlightDamage()
        {
            Vector2 checkPos = (Vector2)_transform.position + Vector2.down * 0.5f;
            Collider2D[] hits = Physics2D.OverlapBoxAll(checkPos, new Vector2(FlightCheckWidth, FlightCheckHeight), 0, _enemyMask);

            foreach (var hit in hits)
            {
                PushAndDamage(hit, _plungeAOEDamage.Value * 0.5f, _plungeKnockbackForce.Value * FlightKnockbackMultiplier);
            }
        }

        private void LandPlunge()
        {
            // Разовый звук удара с высоким питчем (как в твоем конфиге)
            _audioService.PlaySfxVariation("AbilityImpactPlunge", 1, 3, 1.5f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(_transform.position, _plungeAOERadius.Value, _enemyMask);

            foreach (Collider2D hit in hits)
            {
                PushAndDamage(hit, _plungeAOEDamage.Value, _plungeKnockbackForce.Value);
            }

            StopPlunge();
        }

        private void PushAndDamage(Collider2D hit, float damage, float force)
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
                if (target != null && target.HasComponent<CurrentHealth>())
                {
                    target.CurrentHealth.Value -= damage;

                    target.TakeDamageEvent?.Invoke(new DamageData
                    {
                        Amount = damage,
                        SourcePosition = hit.transform.position
                    });
                }
            }
        }

        // В OnUpdate просто меняем состояние, View само подхватит
        private void StartPlunge()
        {
            _isPlunging.Value = true;
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.5f, -_plungeSpeed.Value);
        }

        private void StopPlunge()
        {
            _isPlunging.Value = false;
        }

        private void ClearPlungeLoop()
        {
            if (!string.IsNullOrEmpty(_activeLoopId))
            {
                // _audioService.StopLoopingSfx(_activeLoopId);
                _activeLoopId = null;
            }
        }
    }
}