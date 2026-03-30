using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    /// <summary>
    /// Отвечает за физику на склонах:
    ///   • Определяет IsOnSlope и SlopeNormal через OnCollisionStay2D
    ///   • Скатывает игрока при беге вверх (против нормали)
    ///   • Накапливает SlopeAccumSpeed при спуске вниз
    ///   • Сбрасывает SlopeAccumSpeed при уходе со склона
    ///
    /// Slope-прыжок и slope-slide реализованы в JumpSystem / SlideSystem
    /// через флаги IsOnSlope + SlopeAccumSpeed.
    /// </summary>
    public class SlopeSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        // ── Настройки ──────────────────────────────────────────────────────
        private const float MinSlopeAngle = 15f;   // минимальный угол для активации склона
        private const float MaxSlopeAngle = 75f;   // максимальный угол (выше = стена)
        private const float UphillSlideForce = 18f;   // сила выталкивания назад при беге вверх
        private const float DownhillAccelForce = 12f;   // сила ускорения при спуске
        private const float MagnetForce = 20f;   // прижим к поверхности
        private const float MaxAccumSpeed = 20f;   // потолок накопленной скорости
        private const float AccumDecayRate = 8f;    // скорость затухания накопленной скорости вне склона
        private const float AccumGainRate = 6f;    // скорость набора накопленной скорости на спуске
        private const float SlideOffDelay = 0.1f;  // задержка перед сбросом IsOnSlope (coyote)

        // ── Зависимости ────────────────────────────────────────────────────
        private Entity _entity;
        private Rigidbody2D _rigidbody;
        private EntityCollisionProxy _collisionProxy;

        // ── Компоненты Entity ───────────────────────────────────────────────
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<float> _slopeAccumSpeed;
        private ReactiveVariable<float> _slopeBoostMultiplier;
        private LayerMask _slopeMask;

        // ── Внутреннее состояние ────────────────────────────────────────────
        private Vector2 _slopeNormal = Vector2.up;
        private bool _contactThisFrame = false;
        private float _slideOffTimer = 0f;

        // ── IInitializableSystem ────────────────────────────────────────────
        public void OnInit(Entity entity)
        {
            _entity = entity;
            _rigidbody = entity.Rigidbody;
            _isOnSlope = entity.IsOnSlope;
            _slopeAccumSpeed = entity.SlopeAccumSpeed;
            _slopeBoostMultiplier = entity.SlopeBoostMultiplier;
            _slopeMask = entity.SlopeMask;

            _collisionProxy = entity.Transform.GetComponent<EntityCollisionProxy>();
            if (_collisionProxy != null)
                _collisionProxy.OnCollisionStayEvent += OnCollisionStay;
        }

        // ── IUpdatableSystem ────────────────────────────────────────────────
        public void OnUpdate(float deltaTime)
        {
            if (!_contactThisFrame)
            {
                // Небольшой coyote-delay перед сбросом IsOnSlope
                _slideOffTimer += deltaTime;
                if (_slideOffTimer >= SlideOffDelay && _isOnSlope.Value)
                {
                    _isOnSlope.Value = false;
                    _slopeNormal = Vector2.up;
                }
            }
            else
            {
                _slideOffTimer = 0f;
            }

            // Плавно сбрасываем накопленную скорость, когда не на склоне
            if (!_isOnSlope.Value && _slopeAccumSpeed.Value > 0f)
            {
                _slopeAccumSpeed.Value = Mathf.MoveTowards(
                    _slopeAccumSpeed.Value, 0f, AccumDecayRate * deltaTime);
            }

            // Сбрасываем флаг контакта — он выставляется заново в OnCollisionStay
            _contactThisFrame = false;
        }

        // ── Физика склона (вызывается из EntityCollisionProxy) ──────────────
        private void OnCollisionStay(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & _slopeMask) == 0)
                return;

            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            if (angle < MinSlopeAngle || angle > MaxSlopeAngle)
                return;

            _contactThisFrame = true;
            _isOnSlope.Value = true;
            _slopeNormal = contact.normal;

            // Вектор "вниз по склону"
            Vector2 downhill = new Vector2(contact.normal.y, -contact.normal.x);
            if (downhill.y > 0f) downhill = -downhill;

            float velX = _rigidbody.linearVelocity.x;
            bool movingDownhill = Mathf.Sign(velX) == Mathf.Sign(downhill.x) && Mathf.Abs(velX) > 0.5f;
            bool movingUphill = Mathf.Sign(velX) != Mathf.Sign(downhill.x) && Mathf.Abs(velX) > 0.5f;

            if (movingDownhill)
            {
                // Ускорение + прижим при спуске
                float boost = DownhillAccelForce * _slopeBoostMultiplier.Value;
                _rigidbody.AddForce(downhill * boost, ForceMode2D.Force);
                _rigidbody.AddForce(-contact.normal * MagnetForce, ForceMode2D.Force);

                // Накапливаем скорость (используется slope jump)
                _slopeAccumSpeed.Value = Mathf.Min(
                    _slopeAccumSpeed.Value + AccumGainRate * Time.fixedDeltaTime,
                    MaxAccumSpeed);
            }
            else if (movingUphill)
            {
                // Скатывание назад при беге вверх — отталкиваем вниз по склону
                _rigidbody.AddForce(downhill * UphillSlideForce, ForceMode2D.Force);

                // На подъёме не накапливаем
                _slopeAccumSpeed.Value = Mathf.MoveTowards(
                    _slopeAccumSpeed.Value, 0f, AccumDecayRate * Time.fixedDeltaTime);
            }

            UpdateViewRotation(contact.normal);
        }

        // ── Визуальный поворот по нормали ────────────────────────────────────
        private void UpdateViewRotation(Vector2 normal)
        {
            Transform view = _entity.Transform.Find("ViewContainer");
            if (view == null) return;

            float targetAngle = Vector2.SignedAngle(Vector2.up, normal);
            view.rotation = Quaternion.Lerp(
                view.rotation,
                Quaternion.Euler(0f, 0f, targetAngle),
                0.15f);
        }

        // Публичный доступ к нормали для JumpSystem / SlideSystem
        public Vector2 SlopeNormal => _slopeNormal;

        // ── IDisposableSystem ───────────────────────────────────────────────
        public void OnDispose()
        {
            if (_collisionProxy != null)
                _collisionProxy.OnCollisionStayEvent -= OnCollisionStay;
        }
    }
}