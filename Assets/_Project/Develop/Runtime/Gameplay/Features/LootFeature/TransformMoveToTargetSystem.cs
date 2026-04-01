using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class TransformMoveToTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;

        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<float> _movementSpeed;
        private ICompositeCondition _canMove;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _currentTarget = entity.CurrentTarget;
            _canMove = entity.CanMove;
            _movementSpeed = entity.MoveSpeed;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false || _currentTarget.Value == null) 
                return;

            _transform.position = Vector3
                .MoveTowards(_transform.position, _currentTarget.Value.Transform.position, _movementSpeed.Value * deltaTime);
        }
    }
}