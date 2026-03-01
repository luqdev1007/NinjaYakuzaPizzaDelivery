using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics
{
    public class RigidbodyGravityApplySystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _gravityScale;
        private Rigidbody _rigidbody;

        public void OnInit(Entity entity)
        {
            _gravityScale = entity.GravityScale;
            _rigidbody = entity.Rigidbody;
        }

        public void OnUpdate(float deltaTime)
        {
            _rigidbody.linearVelocity += Vector3.down * _gravityScale.Value * deltaTime;
        }
    }
}