using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    /// <summary>
    /// Обычный подкат на земле + slope-скатывание на склоне.
    ///
    /// Логика выбора режима:
    ///   • IsOnSlope == true  → SlopeSlideCoroutine (скатывание по склону)
    ///   • IsOnSlope == false → обычный SlideCoroutine (как был раньше)
    ///
    /// Slope-слайд:
    ///   • Даёт импульс вдоль склона вниз
    ///   • Скорость основана на SlopeAccumSpeed + SlideSpeed
    ///   • Коллайдер уменьшается как в обычном слайде
    ///   • Завершается когда IsOnSlope становится false ИЛИ истекает макс. время
    /// </summary>
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float SlopeSlideMaxDuration = 3f;   // страховочное ограничение slope-slide
        private const float SlopeSlideSpeedBonus = 0.7f; // Было 1.5 (снижаем влияние накопленной скорости на импульс)

        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<float> _slideDuration;
        private ReactiveVariable<float> _slideSpeed;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        // Сохраняем ссылку на SlopeSystem чтобы читать нормаль склона
        private SlopeSystem _slopeSystem;

        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;
        private Vector2 _slideColliderSize;
        private Vector2 _slideColliderOffset;

        public SlideSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer, SlopeSystem slopeSystem)
        {
            _slopeSystem = slopeSystem;
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _canSlide = entity.CanSlide;
            _isSliding = entity.IsSliding;
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _slideDuration = entity.SlideDuration;
            _slideSpeed = entity.SlideSpeed;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.BodyCollider;

            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.1f));
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isSliding.Value)
                return;

            bool slidePressed = _inputService.IsSlideKeyPressed;
            bool canSlide = _canSlide.Evaluate();

            if (!slidePressed || !canSlide)
                return;

            // Slope-слайд: только если стоим на склоне
            if (_isOnSlope.Value)
            {
                _coroutinesPerformer.StartPerform(SlopeSlideCoroutine());
                return;
            }

            // Обычный слайд: только если на земле
            if (_isGrounded.Value)
            {
                _coroutinesPerformer.StartPerform(SlideCoroutine());
            }
        }

        // ── Обычный подкат (без изменений в логике) ──────────────────────────
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

        // ── Slope-скатывание ─────────────────────────────────────────────────
        private IEnumerator SlopeSlideCoroutine()
        {
            _isSliding.Value = true;
            SetSlideCollider(true);

            float elapsed = 0f;
            float accumSpeed = _slopeAccumSpeed.Value;

            // Начальный импульс вдоль склона вниз
            Vector2 slopeNormal = _slopeSystem != null ? _slopeSystem.SlopeNormal : Vector2.up;
            Vector2 downhill = GetDownhill(slopeNormal);
            float startSpeed = _slideSpeed.Value + accumSpeed * SlopeSlideSpeedBonus;

            // Даём стартовый импульс
            _rigidbody.AddForce(downhill * startSpeed, ForceMode2D.Impulse);

            // Держим коллайдер уменьшенным пока едем по склону
            while (_isOnSlope.Value && elapsed < SlopeSlideMaxDuration)
            {
                // Обновляем нормаль на случай если склон кривой
                slopeNormal = _slopeSystem != null ? _slopeSystem.SlopeNormal : Vector2.up;
                downhill = GetDownhill(slopeNormal);

                // Лёгкий постоянный прижим чтобы не отлетал
                _rigidbody.AddForce(-slopeNormal * 10f, ForceMode2D.Force);

                elapsed += Time.deltaTime;
                yield return null;
            }

            SetSlideCollider(false);
            _isSliding.Value = false;
        }

        // ── Хелперы ─────────────────────────────────────────────────────────
        private static Vector2 GetDownhill(Vector2 normal)
        {
            Vector2 downhill = new Vector2(normal.y, -normal.x);
            if (downhill.y > 0f) downhill = -downhill;
            return downhill;
        }

        private void SetSlideCollider(bool sliding)
        {
            if (_collider is not CapsuleCollider2D capsule)
                return;

            capsule.size = sliding ? _slideColliderSize : _defaultColliderSize;
            capsule.offset = sliding ? _slideColliderOffset : _defaultColliderOffset;
        }
    }
}