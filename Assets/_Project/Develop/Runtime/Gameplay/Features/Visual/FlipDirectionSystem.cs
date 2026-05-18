using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Visual
{
    public class FlipDirectionSystem : IInitializableSystem, IUpdatableSystem
    {
        private Transform _transform;
        private ICompositeCondition _canFlip;
        private ReactiveVariable<float> _lookDirectionX;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _canFlip = entity.CanFlip;
            _lookDirectionX = entity.LookDirectionX;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canFlip.Evaluate() == false)
                return;

            float targetYRotation = _lookDirectionX.Value > 0 ? 0f : 180f;
            float currentYRotation = _transform.localRotation.eulerAngles.y;

            if (!Mathf.Approximately(currentYRotation, targetYRotation))
            {
                Vector3 currentAngles = _transform.localRotation.eulerAngles;
                _transform.localRotation = Quaternion.Euler(currentAngles.x, targetYRotation, currentAngles.z);
            }
        }
    }
}