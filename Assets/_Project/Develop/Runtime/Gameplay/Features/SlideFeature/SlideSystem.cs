using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly LayerMask _enemyMask;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canSlide;
        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _slideDuration;
        private ReactiveVariable<float> _slideSpeed;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _plungeAOERadius;
        private ReactiveVariable<float> _plungeAOEDamage;
        private ReactiveVariable<float> _plungeKnockbackForce;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private float _defaultGravityScale;
        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;
        private Vector2 _slideColliderSize;
        private Vector2 _slideColliderOffset;

        public SlideSystem(IInputService inputService, LayerMask enemyMask, ICoroutinesPerformer coroutinesPerformer)
        {
            _inputService = inputService;
            _enemyMask = enemyMask;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _canSlide = entity.CanSlide;
            _canPlunge = entity.CanPlunge;
            _isSliding = entity.IsSliding;
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;
            _slideDuration = entity.SlideDuration;
            _slideSpeed = entity.SlideSpeed;
            _plungeSpeed = entity.PlungeSpeed;
            _plungeAOERadius = entity.PlungeAOERadius;
            _plungeAOEDamage = entity.PlungeAOEDamage;
            _plungeKnockbackForce = entity.PlungeKnockbackForce;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.Transform.GetComponent<Collider2D>();
            _defaultGravityScale = _rigidbody.gravityScale;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.25f));
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isPlunging.Value)
            {
                UpdatePlunge();
                return;
            }

            if (_isSliding.Value)
                return;

            if (_inputService.IsSlideKeyPressed && _canSlide.Evaluate() && _isGrounded.Value)
                _coroutinesPerformer.StartPerform(SlideCoroutine());
            else if (_inputService.IsSlideKeyPressed && _canPlunge.Evaluate() && !_isGrounded.Value)
                StartPlunge();
        }

        // ─── SLIDE ───────────────────────────────────────────

        private IEnumerator SlideCoroutine()
        {
            _isSliding.Value = true;
            SetSlideCollider(true);

            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            float elapsed = 0f;
            float duration = _slideDuration.Value;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t * t);
                _rigidbody.linearVelocity = new Vector2(
                    direction * currentSpeed,
                    _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            SetSlideCollider(false);
            _isSliding.Value = false;
        }

        private void SetSlideCollider(bool sliding)
        {
            if (_collider is not CapsuleCollider2D capsule)
                return;

            capsule.size = sliding ? _slideColliderSize : _defaultColliderSize;
            capsule.offset = sliding ? _slideColliderOffset : _defaultColliderOffset;
        }

        // ─── PLUNGE ──────────────────────────────────────────

        private void StartPlunge()
        {
            _isPlunging.Value = true;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, -_plungeSpeed.Value);
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
            _rigidbody.gravityScale = _defaultGravityScale;
        }
    }
}