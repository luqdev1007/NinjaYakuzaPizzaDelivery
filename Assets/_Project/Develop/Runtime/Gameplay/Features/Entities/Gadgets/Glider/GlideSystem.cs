using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Gadgets.Glider
{
    public class GlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canGlide;
        private ReactiveVariable<bool> _intentJump;
        private ReactiveVariable<bool> _isGliding;
        private ReactiveVariable<float> _baseGravityScale;

        private ReactiveVariable<float> _glideMaxFallSpeed;
        private ReactiveVariable<float> _glideSpeedDamping;
        private ReactiveVariable<float> _glideSnapSpeed;
        private ReactiveVariable<float> _glideSnapDuration;
        private ReactiveVariable<float> _glideHorizontalDrag;
        private ReactiveVariable<float> _glideGravityScale;

        private Rigidbody2D _rigidbody;
        private float _snapTimer;
        private bool _isSnapActive;

        private float _holdTimer;

        private const float GlideActivationDelay = 0.15f; // config?

        public void OnInit(Entity entity)
        {
            _canGlide = entity.CanGlide;

            _intentJump = entity.IntentJump;

            _isGliding = entity.IsGliding;

            _baseGravityScale = entity.BaseGravityScale;

            _glideMaxFallSpeed = entity.GlideMaxFallSpeed;
            _glideSpeedDamping = entity.GlideSpeedDamping;

            _glideSnapSpeed = entity.GlideSnapSpeed;
            _glideSnapDuration = entity.GlideSnapDuration;

            _glideHorizontalDrag = entity.GlideHorizontalDrag;
            _glideGravityScale = entity.GlideGravityScale;

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            bool basicConditionsMet = _intentJump.Value && _canGlide.Evaluate();

            if (_rigidbody.linearVelocity.y > 0.1f)
            {
                basicConditionsMet = false;
            }

            if (basicConditionsMet)
            {
                if (!_isGliding.Value)
                {
                    _holdTimer += deltaTime;

                    if (_holdTimer >= GlideActivationDelay)
                    {
                        StartGlide();
                    }
                }
                else
                {
                    ProcessGlidePhysics(deltaTime);
                }
            }
            else
            {
                _holdTimer = 0f;

                if (_isGliding.Value)
                {
                    StopGlide();
                }
            }
        }

        private void StartGlide()
        {
            _isGliding.Value = true;
            _isSnapActive = true;
            _snapTimer = _glideSnapDuration.Value;

            _rigidbody.gravityScale = _glideGravityScale.Value;
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _glideSnapSpeed.Value);
        }

        private void ProcessGlidePhysics(float deltaTime)
        {
            Vector2 velocity = _rigidbody.linearVelocity;

            if (_isSnapActive)
            {
                _snapTimer -= deltaTime;

                velocity.y = Mathf.MoveTowards(velocity.y, _glideSnapSpeed.Value, _glideSpeedDamping.Value * 2f * deltaTime);

                if (_snapTimer <= 0f)
                {
                    _isSnapActive = false;
                }
            }
            else
            {
                float targetY = -Mathf.Abs(_glideMaxFallSpeed.Value);

                if (velocity.y < targetY)
                {
                    velocity.y = Mathf.MoveTowards(velocity.y, targetY, _glideSpeedDamping.Value * deltaTime);
                }
            }

            if (Mathf.Abs(velocity.x) > 0.01f)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0f, _glideHorizontalDrag.Value * deltaTime);
            }

            _rigidbody.linearVelocity = velocity;
        }

        private void StopGlide()
        {
            _isGliding.Value = false;
            _isSnapActive = false;
            _rigidbody.gravityScale = _baseGravityScale.Value;
        }
    }
}