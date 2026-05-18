using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump
{
    public class SurfaceCheckSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float CoyoteTime = 0.1f;
        private float _coyoteTimer;

        private ReactiveVariable<bool> _isGrounded;
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<Vector2> _slopeNormal;
        private ReactiveVariable<float> _slopeAngle;
        private ReactiveVariable<float> _slopeMinAngle;
        private ReactiveVariable<float> _slopeMaxAngle;

        private Collider2D _body;
        private LayerMask _groundMask;
        private ContactFilter2D _contactFilter;

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _isOnSlope = entity.IsOnSlope;
            _slopeNormal = entity.SlopeNormal;
            _slopeAngle = entity.SlopeAngle;
            _slopeMinAngle = entity.SlopeMinAngle;
            _slopeMaxAngle = entity.SlopeMaxAngle;

            _body = entity.BodyCollider;
            _groundMask = entity.GroundMask;

            _contactFilter = new ContactFilter2D();
            _contactFilter.SetLayerMask(_groundMask);
            _contactFilter.useTriggers = true;
        }

        public void OnUpdate(float deltaTime)
        {
            float castDistance = 0.15f;
            RaycastHit2D[] results = new RaycastHit2D[4];
            int hitCount = _body.Cast(Vector2.down, _contactFilter, results, castDistance);

            bool foundValidGround = false;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = results[i];

                if (hit.distance <= 0.001f)
                {
                    continue;
                }

                if (hit.normal.y < 0.6f)
                {
                    continue;
                }

                foundValidGround = true;
                _coyoteTimer = CoyoteTime;
                _isGrounded.Value = true;

                Vector2 normal = hit.normal;
                float angle = Vector2.Angle(Vector2.up, normal);

                _slopeNormal.Value = normal;
                _slopeAngle.Value = angle;

                _isOnSlope.Value = angle > _slopeMinAngle.Value && angle < _slopeMaxAngle.Value;

                break;
            }

            if (!foundValidGround)
            {
                _isOnSlope.Value = false;
                _slopeNormal.Value = Vector2.up;
                _slopeAngle.Value = 0f;

                if (_coyoteTimer > 0f)
                {
                    _coyoteTimer -= deltaTime;
                }

                _isGrounded.Value = _coyoteTimer > 0f;
            }
        }
    }
}