using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.View
{
    public class FlipDirectionSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;

        public void OnInit(Entity entity)
        {
            _entity = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.Transform == null || !_entity.CanFlip.Evaluate())
                return;

            float moveX = _entity.MoveDirection.Value.x;

            if (Mathf.Abs(moveX) < 0.01f)
                return;

            Vector3 scale = _entity.Transform.localScale;

            float desiredSign = moveX > 0 ? 1f : -1f;
            float currentSign = Mathf.Sign(scale.x);

            if (desiredSign != currentSign)
            {
                scale.x = Mathf.Abs(scale.x) * desiredSign;
                _entity.Transform.localScale = scale;
            }
        }
    }
}