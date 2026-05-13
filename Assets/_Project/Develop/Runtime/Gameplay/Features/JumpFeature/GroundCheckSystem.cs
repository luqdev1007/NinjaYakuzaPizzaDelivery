using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class GroundCheckSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float CoyoteTime = 0.1f;
        private float _coyoteTimer;

        private ReactiveVariable<bool> _isGrounded;

        private Collider2D _body;
        private LayerMask _groundMask;

        private ContactFilter2D _contactFilter;
        private readonly RaycastHit2D[] _results = new RaycastHit2D[1];

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _body = entity.BodyCollider;
            _groundMask = entity.GroundMask;

            _contactFilter = new ContactFilter2D();
            _contactFilter.SetLayerMask(_groundMask);
            _contactFilter.useTriggers = true;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector2 origin = _body.bounds.center;
            Vector2 size = new Vector2(_body.bounds.size.x * 0.8f, 0.1f);
            float castDistance = _body.bounds.extents.y + 0.1f;

            int hitCount = Physics2D.BoxCast(
                origin,
                size,
                0f,
                Vector2.down,
                _contactFilter,
                _results,
                castDistance);

            if (hitCount > 0)
            {
                _coyoteTimer = CoyoteTime;
                _isGrounded.Value = true;
            }
            else
            {
                if (_coyoteTimer > 0f)
                {
                    _coyoteTimer -= deltaTime;
                }
                else
                {
                    _isGrounded.Value = false;
                }
            }
        }
    }
}