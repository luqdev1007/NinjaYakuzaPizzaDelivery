using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Combat.HitImpact
{
    public class AerialHitSuspensionSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _entity;
        private IDisposable _successfulHitDisposable;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _successfulHitDisposable = _entity.SuccessfulHitEvent.Subscribe(OnSuccessfulHit);
        }

        private void OnSuccessfulHit()
        {
            if (_entity.IsGrounded.Value)
                return;

            if (_entity.Rigidbody == null)
                return;

            _entity.Rigidbody.linearVelocity = new Vector2(_entity.Rigidbody.linearVelocity.x, 0f);

            _entity.Rigidbody.AddForce(_entity.AerialHangForce.Value, ForceMode2D.Impulse);
        }

        public void OnDispose() => _successfulHitDisposable?.Dispose();
    }
}