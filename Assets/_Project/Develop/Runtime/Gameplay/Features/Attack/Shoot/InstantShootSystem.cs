using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Shoot
{
    public class InstantShootSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ProjectilesFactory _projectilesFactory;
        private readonly ConfigsProviderService _configsProviderService;

        private Entity _entity;

        private ReactiveEvent _attackDelayEndEvent;
        private ReactiveVariable<float> _damage;
        private Transform _shootPoint;

        private IDisposable _attackDelayDisposable;

        public InstantShootSystem(ProjectilesFactory projectilesFactory, ConfigsProviderService configsProviderService)
        {
            _projectilesFactory = projectilesFactory;
            _configsProviderService = configsProviderService;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _attackDelayEndEvent = entity.AttackDelayEndEvent;
            _damage = entity.AttackDamage;
            _shootPoint = entity.ShootPoint;

            _attackDelayDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }

        public void OnDispose()
        {
            _attackDelayDisposable.Dispose();
        }

        private void OnAttackDelayEnd()
        {
            if (_entity.CurrentTarget == null || _entity.CurrentTarget.Value == null || _entity.CurrentTarget.Value.Transform == null)
                return;

            Vector3 targetPos = _entity.CurrentTarget.Value.Transform.position;
            Vector3 originPos = _shootPoint.position;

            Vector3 direction = (targetPos - originPos).normalized;

            /*
            SimpleProjectileConfig config = _configsProviderService.GetConfig<SimpleProjectileConfig>();
            
            config.GravityScale = 0;

            _projectilesFactory.Create(
                _shootPoint,
                new ProjectileCreationContext(_entity,
                launchPower: 15,
                finalDamage: _damage.Value, 
                launchDelay: 0.05f,
                shootDirection: direction),
                config
                );
            */
        }
    }
}
