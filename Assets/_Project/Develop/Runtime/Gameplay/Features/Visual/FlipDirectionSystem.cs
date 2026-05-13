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

            Vector3 scale = _transform.localScale;
            float targetScaleX = _lookDirectionX.Value > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);

            if (!Mathf.Approximately(scale.x, targetScaleX))
            {
                scale.x = targetScaleX;
                _transform.localScale = scale;
            }
        }
    }
}