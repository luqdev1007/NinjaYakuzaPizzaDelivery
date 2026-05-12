using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature
{
    public class GlideSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canGlide;

        private ReactiveVariable<bool> _isGliding;

        private ReactiveVariable<float> _glideMaxFallSpeed;
        private ReactiveVariable<float> _glideBounceForce;

        private ReactiveVariable<float> _glideSnapSpeed;
        private ReactiveVariable<float> _glideSnapDuration;

        private ReactiveVariable<float> _glideHorizontalDrag;
        private ReactiveVariable<float> _glideSpeedDamping;

        private ReactiveVariable<float> _baseGravityScale;
        private ReactiveVariable<float> _glideGravityScale;

        private Rigidbody2D _rigidbody;

        public void OnInit(Entity entity)
        {
            /*
            _canGlide = entity.CanGlide;

            _isGliding = entity.IsGliding;

            _glideMaxFallSpeed = entity.GlideMaxFallSpeed;
            _glideBounceForce = entity.GlideBounceForce;

            _glideSnapSpeed = entity.GlideSnapSpeed;
            _glideSnapDuration = entity.GlideSnapDuration;

            _glideHorizontalDrag = entity.GlideHorizontalDrag;
            _glideSpeedDamping = entity.GlideSpeedDamping;

            _baseGravityScale = entity.BaseGravityScale;
            _glideGravityScale = entity.GlideGravityScale;

            _rigidbody = entity.Rigidbody;
            */
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canGlide.Evaluate() == false)
            {
                if (_isGliding.Value == true)
                    StopGlide();

                return;
            }

            StartGlide();
        }

        private void StartGlide()
        {
            _isGliding.Value = true;
        }

        private void StopGlide()
        {
            _isGliding.Value = false;
        }
    }
}