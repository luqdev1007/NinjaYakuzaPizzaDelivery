using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class GroundCheckSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly float _coyoteTime;

        private ReactiveVariable<bool> _isGrounded;
        private Collider2D _body;
        private LayerMask _groundMask;
        private float _coyoteTimer;

        public GroundCheckSystem(float coyoteTime = 0.1f)
        {
            _coyoteTime = coyoteTime;
        }

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _body = entity.BodyCollider;
            _groundMask = entity.GroundMask;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector2 origin = _body.bounds.center;
            Vector2 size = new Vector2(_body.bounds.size.x * 0.8f, 0.05f);
            float castDistance = _body.bounds.extents.y + 0.02f;

            RaycastHit2D hit = Physics2D.BoxCast(
                origin, size, 0f, Vector2.down, castDistance, _groundMask);

            bool physicallyGrounded = hit.collider != null;

            if (physicallyGrounded)
            {
                _coyoteTimer = _coyoteTime;
                _isGrounded.Value = true;
            }
            else
            {
                _coyoteTimer -= deltaTime;
                if (_coyoteTimer <= 0f)
                    _isGrounded.Value = false;
            }
        }
    }
}