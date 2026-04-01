using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class CollectedOnNearToTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Entity> _target;
        private Transform _transform;
        private ReactiveVariable<bool> _isCollected;

        public void OnInit(Entity entity)
        {
            _target = entity.CurrentTarget;
            _transform = entity.Transform;
            _isCollected = entity.IsCollected;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isCollected.Value == false && _target.Value != null)
            {
                // В 2D 0.5f - хороший радиус для "подбора"
                float distance = Vector2.Distance(_target.Value.Transform.position, _transform.position);
                if (distance < 0.5f)
                {
                    _isCollected.Value = true;
                }
            }
        }
    }
}