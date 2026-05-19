using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _intentPlunge; 

        private ICompositeCondition _canPlunge;

        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<float> _plungeSpeed;

        private ReactiveVariable<bool> _isGrounded;

        private Rigidbody2D _rigidbody;

        public void OnInit(Entity entity)
        {
            _canPlunge = entity.CanPlunge;
            _isPlunging = entity.IsPlunging;
            _plungeSpeed = entity.PlungeSpeed;
            _intentPlunge = entity.IntentSlide;

            _isGrounded = entity.IsGrounded;

            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isPlunging.Value)
            {
                UpdatePlunge();
                return;
            }

            if (_intentPlunge.Value && _canPlunge.Evaluate())
            {
                StartPlunge();
            }
        }

        private void StartPlunge()
        {
            _isPlunging.Value = true;

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x * 0.5f, -_plungeSpeed.Value);
        }

        private void UpdatePlunge()
        {
            if (_rigidbody.linearVelocity.y > -_plungeSpeed.Value)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, -_plungeSpeed.Value);
            }

            if (_isGrounded.Value)
            {
                StopPlunge();
                return;
            }


            if (!_intentPlunge.Value)
            {
                StopPlunge();
            }
        }

        private void StopPlunge()
        {
            _isPlunging.Value = false;
        }
    }
}