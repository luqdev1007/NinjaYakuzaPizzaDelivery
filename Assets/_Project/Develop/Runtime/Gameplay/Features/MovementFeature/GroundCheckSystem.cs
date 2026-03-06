using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class GroundCheckSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _isGrounded;
        private Collider2D _body;
        private LayerMask _groundMask;

        public void OnInit(Entity entity)
        {
            _isGrounded = entity.IsGrounded;
            _body = entity.BodyCollider;
            _groundMask = entity.GroundMask; // новый компонент ниже
        }

        public void OnUpdate(float deltaTime)
        {
            // кастуем маленький бокс под ногами
            Vector2 origin = _body.bounds.center;
            Vector2 size = new Vector2(_body.bounds.size.x * 0.9f, 0.1f);
            float castDistance = _body.bounds.extents.y + 0.05f;

            RaycastHit2D hit = Physics2D.BoxCast(
                origin, size, 0f, Vector2.down, castDistance, _groundMask);

            _isGrounded.Value = hit.collider != null;
            // Debug.Log("OnGround: " + _isGrounded.Value);
        }
    }
}