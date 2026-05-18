using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeRotationSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<Vector2> _slopeNormal;
        private ReactiveVariable<float> _lookDirectionX; 

        private Transform _viewContainer;

        private const float RotationSpeed = 5f;
        private const string ViewContainer = nameof(ViewContainer);

        public void OnInit(Entity entity)
        {
            _isOnSlope = entity.IsOnSlope;
            _slopeNormal = entity.SlopeNormal;
            _lookDirectionX = entity.LookDirectionX;

            _viewContainer = entity.Transform.Find(ViewContainer);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isOnSlope.Value)
            {
                float targetAngle = Mathf.Atan2(_slopeNormal.Value.x, _slopeNormal.Value.y) * -Mathf.Rad2Deg;

                float facingSign = Mathf.Sign(_lookDirectionX.Value);
                targetAngle *= facingSign;

                Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRotation, RotationSpeed * deltaTime);
            }
            else
            {
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, Quaternion.identity, RotationSpeed * deltaTime);
            }
        }
    }
}