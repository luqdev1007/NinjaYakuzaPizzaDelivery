using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class GenerateMoveDirectionToTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Entity> _target;
        private Transform _transform;
        private ReactiveVariable<Vector2> _moveDirection;

        public void OnInit(Entity entity)
        {
            _target = entity.CurrentTarget;
            _transform = entity.Transform;
            _moveDirection = entity.MoveDirection;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_target.Value != null)
            {
                // Направление к цели для 2D
                Vector3 direction = _target.Value.Transform.position - _transform.position;
                _moveDirection.Value = direction.normalized;
            }
            else
            {
                _moveDirection.Value = Vector3.zero;
            }
        }
    }
}