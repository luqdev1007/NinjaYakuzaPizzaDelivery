using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature
{
    public class GroundCheckSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _isGrounded;
        private Collider2D _body;
        private LayerMask _groundMask;

        private Vector2 _boxSize;
        private float _originOffset;
        private float _coyoteTimer;
        private ContactFilter2D _contactFilter;

        private readonly float _coyoteTime;
        private readonly RaycastHit2D[] _results = new RaycastHit2D[1];

        public GroundCheckSystem(float coyoteTime = 0.1f)
        {
            _coyoteTime = coyoteTime;
        }

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _body = entity.BodyCollider;
            _groundMask = entity.GroundMask;

            _boxSize = new Vector2(_body.bounds.size.x * 0.8f, 0.1f);
            _originOffset = _body.bounds.extents.y;

            _contactFilter = new ContactFilter2D();
            _contactFilter.SetLayerMask(_groundMask);
            _contactFilter.useTriggers = true;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector2 origin = (Vector2)_body.transform.position + _body.offset;
            float castDistance = _originOffset + 0.1f;

            int hitCount = Physics2D.BoxCast(
                origin,
                _boxSize,
                0f,
                Vector2.down,
                _contactFilter,
                _results,
                castDistance);

            bool isGrounded = false;

            if (hitCount > 0)
            {
                RaycastHit2D hit = _results[0];
                if (hit.collider != null)
                {
                    isGrounded = true;
                }
            }

            if (isGrounded)
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