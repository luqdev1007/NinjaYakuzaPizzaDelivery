using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class SlopeSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private Rigidbody2D _rigidbody;
        private LayerMask _slopeMask;
        private EntityCollisionProxy _collisionProxy;

        private const float MinEntrySpeed = 8f;     // Минимальная скорость для активации буста
        private const float SlopeBoostPower = 1.4f; // Множитель скорости при входе на склон
        private const float MagnetForce = 15f;      // Сила прижима к склону (чтобы не подлетал на кочках)

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _rigidbody = entity.Rigidbody;
            _slopeMask = entity.SlopeMask;

            _collisionProxy = _entity.Transform.GetComponent<EntityCollisionProxy>();
            if (_collisionProxy != null)
                _collisionProxy.OnCollisionStayEvent += HandleSlopePhysics;
        }

        private void HandleSlopePhysics(Collision2D collision)
        {
            // 1. Проверка слоя
            if (((1 << collision.gameObject.layer) & _slopeMask) == 0) return;

            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            // Работаем только на средних и крутых склонах (15-70 градусов)
            if (angle < 15f || angle > 75f) return;

            // 2. Направление спуска (вектор вдоль поверхности вниз)
            Vector2 slopeDir = new Vector2(contact.normal.y, -contact.normal.x);
            if (slopeDir.y > 0) slopeDir = -slopeDir;

            // 3. Главная логика: Если мы летим/бежим В СТОРОНУ спуска
            float currentHorizontalMove = _rigidbody.linearVelocity.x;
            bool movingTowardsDownhill = Mathf.Sign(currentHorizontalMove) == Mathf.Sign(slopeDir.x);

            if (movingTowardsDownhill && Mathf.Abs(currentHorizontalMove) > MinEntrySpeed)
            {
                // РАЗОВЫЙ БУСТ (если мы еще не разогнались до предела)
                if (_rigidbody.linearVelocity.magnitude < 25f)
                {
                    _rigidbody.AddForce(slopeDir * SlopeBoostPower, ForceMode2D.Impulse);
                }

                // ПОСТОЯННЫЙ ПРИЖИМ (Magnet)
                // Это самое важное: прижимаем игрока к склону, чтобы он "обтекал" его
                _rigidbody.AddForce(-contact.normal * MagnetForce, ForceMode2D.Force);
            }

            // 4. Визуал (просто и без багов)
            UpdateRotation(contact.normal);
        }

        private void UpdateRotation(Vector2 normal)
        {
            Transform view = _entity.Transform.Find("ViewContainer");
            if (view != null)
            {
                float targetAngle = Vector2.SignedAngle(Vector2.up, normal);
                // Плавно подкручиваем визуал, чтобы не было рывков
                view.rotation = Quaternion.Lerp(view.rotation, Quaternion.Euler(0, 0, targetAngle), 0.2f);
            }
        }

        public void OnDispose()
        {
            if (_collisionProxy != null)
                _collisionProxy.OnCollisionStayEvent -= HandleSlopePhysics;
        }
    }
}