using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics
{
    public class RigidbodyGravityApplySystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody _rigidbody;
        private ReactiveVariable<float> _gravityScale;

        public void OnInit(Entity entity)
        {
            // _rigidbody = entity.Rigidbody;
            _gravityScale = entity.GravityScale;
        }

        public void OnUpdate(float deltaTime)
        {
            _rigidbody.linearVelocity += Vector3.down * _gravityScale.Value * deltaTime;
        }
    }
}