using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeRotationSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<Vector2> _slopeNormal;

        private Transform _viewContainer;

        private const float RotationSpeed = 15f;

        public void OnInit(Entity entity)
        {
            _isOnSlope = entity.IsOnSlope;
            _slopeNormal = entity.SlopeNormal;

            _viewContainer = entity.Transform.Find("ViewContainer");
        }

        public void OnUpdate(float deltaTime)
        {
            if (_viewContainer == null) 
                return;

            if (_isOnSlope.Value)
            {
                float targetAngle = Mathf.Atan2(_slopeNormal.Value.x, _slopeNormal.Value.y) * -Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRotation, RotationSpeed * deltaTime);
            }
            else
            {
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, Quaternion.identity, RotationSpeed * deltaTime);
            }
        }
    }
}