using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<float> _slideSpeed;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize, _defaultColliderOffset;
        private Vector2 _slideColliderSize, _slideColliderOffset;

        public SlideSystem(ICoroutinesPerformer coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            /*
            _canSlide = entity.CanSlide;
            _isSliding = entity.IsSliding;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.BodyCollider;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.25f));
            }
            */
        }

        public void OnUpdate(float deltaTime)
        {
            /*
            if (_isSliding.Value) return;

            if (_inputService.IsSlideKeyPressed && _canSlide.Evaluate())
            {
                if (_isOnSlope.Value)
                    _coroutinesPerformer.StartPerform(SlopeSlideCoroutine());
                else if (_isGrounded.Value)
                    _coroutinesPerformer.StartPerform(SlideCoroutine());
            }
            */
        }

        private IEnumerator SlideCoroutine()
        {
            yield return null;

            /*
            StartSlide();
            float direction = Mathf.Sign(_transform.localScale.x);
            float elapsed = 0f;

            while (elapsed < GroundSlideDuration)
            {
                if (_isOnSlope.Value)
                {
                    yield return SlopeSlideCoroutine();
                    yield break;
                }

                float t = elapsed / GroundSlideDuration;
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t * t);
                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }
            EndSlide();
            */
        }

        private IEnumerator SlopeSlideCoroutine()
        {
            yield return null;

            /*
            StartSlide();
            float elapsed = 0f;

            while (_isOnSlope.Value && elapsed < SlopeSlideMaxDuration)
            {
                Vector2 normal = _slopeSystem.SlopeNormal;
                Vector2 downhill = GetDownhillDirection(normal);

                // ФИКС: Если мы стоим на месте, даем начальный импульс вниз
                if (_rigidbody.linearVelocity.magnitude < 1f)
                {
                    _rigidbody.AddForce(downhill * AutoSlidePush, ForceMode2D.Impulse);
                }

                float totalSpeed = _slideSpeed.Value + _slopeAccumSpeed.Value;
                _rigidbody.AddForce(downhill * totalSpeed, ForceMode2D.Force);
                _rigidbody.AddForce(-normal * SlopeDownForce, ForceMode2D.Force);

                // ФИКС ПОВОРОТА: Поворачиваем персонажа по вектору движения вниз
                HandleSpriteRotationOnSlope(downhill);

                elapsed += Time.deltaTime;
                yield return null;
            }
            EndSlide();
            */
        }

        private void StartSlide()
        {
            if (_isSliding.Value) return;
            _isSliding.Value = true;
            SetSlideCollider(true);
        }

        private void EndSlide()
        {
            SetSlideCollider(false);
            _isSliding.Value = false;
        }

        private void SetSlideCollider(bool isSliding)
        {
            if (_collider is CapsuleCollider2D capsule)
            {
                capsule.size = isSliding ? _slideColliderSize : _defaultColliderSize;
                capsule.offset = isSliding ? _slideColliderOffset : _defaultColliderOffset;
            }
        }

        private void HandleSpriteRotationOnSlope(Vector2 downhill)
        {
            // Смотрим туда, куда тянет склон по оси X
            float direction = downhill.x > 0 ? 1f : -1f;
            Vector3 scale = _transform.localScale;
            scale.x = direction * Mathf.Abs(scale.x);
            _transform.localScale = scale;
        }

        private Vector2 GetDownhillDirection(Vector2 normal)
        {
            Vector2 downhill = new Vector2(normal.y, -normal.x);
            if (downhill.y > 0f) downhill = -downhill;
            return downhill;
        }
    }
}