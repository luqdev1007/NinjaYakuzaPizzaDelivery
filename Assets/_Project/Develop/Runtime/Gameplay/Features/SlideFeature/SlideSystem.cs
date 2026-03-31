using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature
{
    public class SlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float SlopeSlideMaxDuration = 2.5f;
        private const float SlopeSlideSpeedBonus = 0.7f; // Чуть увеличил для сочности
        private const float GroundSlideDuration = 0.6f;
        private const float SlopeDownForce = 15f; // Прижимная сила к склону

        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly SlopeSystem _slopeSystem;

        private ICompositeCondition _canSlide;
        private ReactiveVariable<bool> _isSliding;
        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<float> _slideSpeed;
        private ReactiveVariable<float> _slopeAccumSpeed;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Collider2D _collider;

        private Vector2 _defaultColliderSize, _defaultColliderOffset;
        private Vector2 _slideColliderSize, _slideColliderOffset;

        private Coroutine _activeSlideCoroutine;

        public SlideSystem(IInputService inputService, ICoroutinesPerformer coroutinesPerformer, SlopeSystem slopeSystem)
        {
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _slopeSystem = slopeSystem;
        }

        public void OnInit(Entity entity)
        {
            _canSlide = entity.CanSlide;
            _isSliding = entity.IsSliding;
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _slideSpeed = entity.SlideSpeed;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;

            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;
            _collider = entity.BodyCollider;

            // Настройка уменьшенного коллайдера для проскальзывания в узкие проемы
            if (_collider is CapsuleCollider2D capsule)
            {
                _defaultColliderSize = capsule.size;
                _defaultColliderOffset = capsule.offset;

                // Уменьшаем высоту вдвое и смещаем вниз
                _slideColliderSize = new Vector2(capsule.size.x, capsule.size.y * 0.5f);
                _slideColliderOffset = new Vector2(0f, -(capsule.size.y * 0.25f));
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isSliding.Value) return;

            // Активация подката при нажатии клавиши и выполнении условий (земля/склон + отсутствие запретов)
            if (_inputService.IsSlideKeyPressed && _canSlide.Evaluate())
            {
                if (_isOnSlope.Value)
                {
                    _activeSlideCoroutine = _coroutinesPerformer.StartPerform(SlopeSlideCoroutine());
                }
                else if (_isGrounded.Value)
                {
                    _activeSlideCoroutine = _coroutinesPerformer.StartPerform(SlideCoroutine());
                }
            }
        }

        /// <summary>
        /// Обычный подкат по горизонтальной поверхности (затухающий)
        /// </summary>
        private IEnumerator SlideCoroutine()
        {
            StartSlide();

            float direction = Mathf.Sign(_transform.localScale.x);
            float elapsed = 0f;

            while (elapsed < GroundSlideDuration)
            {
                // Если заехали на склон во время обычного подката — бесшовно переходим в логику склона
                if (_isOnSlope.Value)
                {
                    yield return SlopeSlideCoroutine();
                    yield break;
                }

                float t = elapsed / GroundSlideDuration;
                // Квадратичное замедление для более естественного стопа
                float currentSpeed = Mathf.Lerp(_slideSpeed.Value, 0f, t * t);

                _rigidbody.linearVelocity = new Vector2(direction * currentSpeed, _rigidbody.linearVelocity.y);

                elapsed += Time.deltaTime;
                yield return null;
            }

            EndSlide();
        }

        /// <summary>
        /// Подкат по наклонной поверхности (с ускорением вниз)
        /// </summary>
        private IEnumerator SlopeSlideCoroutine()
        {
            // Если мы перешли сюда из обычного слайда, StartSlide() уже вызван, но повторный вызов не страшен
            StartSlide();

            float elapsed = 0f;

            while (_isOnSlope.Value && elapsed < SlopeSlideMaxDuration)
            {
                Vector2 slopeNormal = _slopeSystem.SlopeNormal;
                Vector2 downhill = GetDownhillDirection(slopeNormal);

                // Рассчитываем скорость с учетом накопленного бонуса со склона
                float totalSpeed = _slideSpeed.Value + (_slopeAccumSpeed.Value * SlopeSlideSpeedBonus);

                // Толкаем персонажа вниз по вектору склона
                _rigidbody.AddForce(downhill * totalSpeed, ForceMode2D.Force);

                // Прижимаем к склону, чтобы не "взлетать" на кочках
                _rigidbody.AddForce(-slopeNormal * SlopeDownForce, ForceMode2D.Force);

                // Разворачиваем спрайт в сторону движения, если скорость достаточна
                HandleSpriteRotation();

                elapsed += Time.deltaTime;
                yield return null;
            }

            EndSlide();
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
            _activeSlideCoroutine = null;
        }

        private void SetSlideCollider(bool isSliding)
        {
            if (_collider is CapsuleCollider2D capsule)
            {
                capsule.size = isSliding ? _slideColliderSize : _defaultColliderSize;
                capsule.offset = isSliding ? _slideColliderOffset : _defaultColliderOffset;
            }
        }

        private void HandleSpriteRotation()
        {
            if (Mathf.Abs(_rigidbody.linearVelocity.x) > 0.5f)
            {
                float direction = _rigidbody.linearVelocity.x > 0 ? 1f : -1f;
                Vector3 scale = _transform.localScale;
                scale.x = direction * Mathf.Abs(scale.x);
                _transform.localScale = scale;
            }
        }

        private Vector2 GetDownhillDirection(Vector2 normal)
        {
            // Математически находим вектор, перпендикулярный нормали и направленный вниз
            Vector2 downhill = new Vector2(normal.y, -normal.x);
            if (downhill.y > 0f) downhill = -downhill;
            return downhill;
        }
    }
}