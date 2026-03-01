using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class SelfExplodeSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ExplosionsFactory _explosionsFactory;

        private Transform _transform;
        private ReactiveVariable<bool> _isDead; 
        private ICompositeCondition _mustExplode;

        public SelfExplodeSystem(ExplosionsFactory explosionsFactory)
        {
            _explosionsFactory = explosionsFactory;
        }

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _isDead = entity.IsDead;
            _mustExplode = entity.MustExplode;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mustExplode.Evaluate() == false)
                return;

            // Debug.Log("SELF EXPLODE!");
            ExplosionView explosion = _explosionsFactory.Create(ExplosionType.Large, _transform.position);
            explosion.ExplosionEffect.Activate(_transform.position, 10);

            _isDead.Value = true;
        }
    }
}