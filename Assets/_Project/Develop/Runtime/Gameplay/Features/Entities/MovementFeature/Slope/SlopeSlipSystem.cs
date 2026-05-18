using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope
{
    public class SlopeSlipSystem : IInitializableSystem, IUpdatableSystem
    {
        private ICompositeCondition _canSlip;

        private ReactiveVariable<bool> _isOnSlope;
        private ReactiveVariable<float> _slopeAngle;
        private ReactiveVariable<Vector2> _slopeNormal;
        private ReactiveVariable<Vector2> _intentMovement;
        private ReactiveVariable<float> _maxStableAngle;
        private ReactiveVariable<float> _slipForce;

        private Rigidbody2D _rigidbody;

        public void OnInit(Entity entity)
        {
            _canSlip = entity.CanSlopeSlip;

            _isOnSlope = entity.IsOnSlope;
            _slopeAngle = entity.SlopeAngle;
            _slopeNormal = entity.SlopeNormal;
            _intentMovement = entity.IntentMovement;
            _rigidbody = entity.Rigidbody;
            _slipForce = entity.SlopeSlipForce;
            _maxStableAngle = entity.SlopeMaxStableAngle;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_canSlip.Evaluate())
                return;

            if (_isOnSlope.Value && _slopeAngle.Value > _maxStableAngle.Value && Mathf.Abs(_intentMovement.Value.x) < 0.01f)
            {
                Vector2 slopeTangent = new Vector2(_slopeNormal.Value.y, -_slopeNormal.Value.x).normalized;
                Vector2 downSlopeDirection = slopeTangent.y < 0 ? slopeTangent : -slopeTangent;

                _rigidbody.linearVelocity += downSlopeDirection * (_slipForce.Value * deltaTime);
            }
        }
    }
}