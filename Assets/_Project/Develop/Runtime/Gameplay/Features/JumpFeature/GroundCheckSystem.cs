using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class GroundCheckSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly float _coyoteTime;

        private ReactiveVariable<bool> _isGrounded;
        private Collider2D _body;
        private LayerMask _groundMask;
        private float _coyoteTimer;

        // Буфер для результатов каста
        private readonly RaycastHit2D[] _results = new RaycastHit2D[1];
        private ContactFilter2D _contactFilter;

        public GroundCheckSystem(float coyoteTime = 0.1f)
        {
            _coyoteTime = coyoteTime;
        }

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _body = entity.BodyCollider;
            _groundMask = entity.GroundMask;

            // Настраиваем фильтр: используем маску и включаем детекцию триггеров
            _contactFilter = new ContactFilter2D();
            _contactFilter.SetLayerMask(_groundMask);
            _contactFilter.useTriggers = true;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector2 origin = _body.bounds.center;
            Vector2 size = new Vector2(_body.bounds.size.x * 0.8f, 0.1f);
            float castDistance = _body.bounds.extents.y + 0.1f;

            // Используем метод с фильтром, чтобы видеть и триггеры, и слои из маски
            int hitCount = Physics2D.BoxCast(
                origin,
                size,
                0f,
                Vector2.down,
                _contactFilter,
                _results,
                castDistance);

            bool isGroundedOnSomething = false;

            if (hitCount > 0)
            {
                RaycastHit2D hit = _results[0];

                if (hit.collider != null)
                {
                    // Проверяем наличие MonoEntity в родителях или на самом объекте
                    var mono = hit.collider.GetComponentInParent<MonoEntity>();

                    if (mono != null && mono.LinkedEntity.HasComponent<ChargedSlashProjectileTag>())
                    {
                        // Стоим на прожектайле
                        isGroundedOnSomething = true;
                    }
                    else
                    {
                        // Стоим на чем-то другом (земля/платформа), что входит в LayerMask
                        isGroundedOnSomething = true;
                    }
                }
            }

            // Логика Coyote Time
            if (isGroundedOnSomething)
            {
                _coyoteTimer = _coyoteTime;
                _isGrounded.Value = true;
            }
            else
            {
                _coyoteTimer -= deltaTime;
                if (_coyoteTimer <= 0f)
                {
                    _isGrounded.Value = false;
                }
            }
        }
    }
}