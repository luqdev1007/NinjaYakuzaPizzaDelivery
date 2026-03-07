using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{

    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;

        private ICompositeCondition _canSlide;
        private ICompositeCondition _canPlunge;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<float> _slideDuration;
        private ReactiveVariable<float> _slideSpeed;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private ReactiveVariable<Vector2> _slopeJumpForce;
        private ReactiveVariable<float> _plungeSpeed;
        private ReactiveVariable<float> _plungeAOERadius;
        private ReactiveVariable<float> _plungeAOEDamage;
        private ReactiveVariable<float> _plungeKnockbackForce;
        private LayerMask _slopeMask;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private float _slideTimer;
        private bool _isOnSlope;
        private float _defaultGravityScale;
        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;
        private Vector2 _slideColliderSize;
        private Vector2 _slideColliderOffset;

        private readonly LayerMask _enemyMask;

        public SlideSystem(IInputService inputService, LayerMask enemyMask)
        {
            _inputService = inputService;
            _enemyMask = enemyMask;
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
            _slopeBoostMultiplier = entity.SlopeBoostMultiplier;
            _slopeJumpForce = entity.SlopeJumpForce;
            _plungeSpeed = entity.PlungeSpeed;
            _plungeAOERadius = entity.PlungeAOERadius;
            _plungeAOEDamage = entity.PlungeAOEDamage;
            _plungeKnockbackForce = entity.PlungeKnockbackForce;
            _slopeMask = entity.SlopeMask;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.Transform.GetComponent<Collider2D>();
            _defaultGravityScale = _rigidbody.gravityScale;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(capsule.offset.x, 0f); // попробуй 0
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
            {
                UpdateSlide(deltaTime);
                return;
            }

            if (_inputService.IsSlideKeyPressed && _canSlide.Evaluate() && _isGrounded.Value)
                StartSlide();
            else if (_inputService.IsSlideKeyPressed && _canPlunge.Evaluate() && !_isGrounded.Value)
                StartPlunge();
        }

        // ─── SLIDE ───────────────────────────────────────────

        private void StartSlide()
        {
            _isOnSlope = CheckSlope();
            _isSliding.Value = true;
            _slideTimer = 0f;
            SetSlideCollider(true);

            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            float speed = _isOnSlope
                ? _slideSpeed.Value * _slopeBoostMultiplier.Value
                : _slideSpeed.Value;

            _rigidbody.linearVelocity = new Vector2(direction * speed, _rigidbody.linearVelocity.y);
        }

        private void UpdateSlide(float deltaTime)
        {
            _isOnSlope = CheckSlope();

            if (_isOnSlope && _inputService.IsSlideKeyHeld && _isGrounded.Value)
            {
                // на наклонной — держим скорость с буcтом
                float direction = _transform.localScale.x > 0 ? 1f : -1f;
                _rigidbody.linearVelocity = new Vector2(
                    direction * _slideSpeed.Value * _slopeBoostMultiplier.Value,
                    _rigidbody.linearVelocity.y);

                // поверхность кончилась или отпустили — рывок
                if (!_isGrounded.Value || !_inputService.IsSlideKeyHeld)
                    EndSlopeSlide();

                return;
            }

            // обычный слайд — по таймеру
            _slideTimer += deltaTime;

            if (_slideTimer >= _slideDuration.Value)
                StopSlide();
        }

        private void EndSlopeSlide()
        {
            StopSlide();
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f); // ← сброс
            float direction = _transform.localScale.x > 0 ? 1f : -1f;
            _rigidbody.AddForce(new Vector2(
                direction * _slopeJumpForce.Value.x,
                _slopeJumpForce.Value.y),
                ForceMode2D.Impulse);
        }

        private void StopSlide()
        {
            _isSliding.Value = false;
            _slideTimer = 0f;
            SetSlideCollider(false);
        }

        private bool CheckSlope()
        {
            RaycastHit2D hit = Physics2D.Raycast(
                _transform.position,
                Vector2.down,
                2f, // ← увеличили
                _slopeMask);

            if (hit.collider == null)
                return false;

            float angle = Vector2.Angle(hit.normal, Vector2.up);
            return angle > 10f;
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
            // удерживаем скорость падения
            if (_rigidbody.linearVelocity.y > -_plungeSpeed.Value)
                _rigidbody.linearVelocity = new Vector2(
                    _rigidbody.linearVelocity.x,
                    -_plungeSpeed.Value);

            // приземлились
            if (_isGrounded.Value)
                LandPlunge();

            // отпустили клавишу
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

                // кнокбэк
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
