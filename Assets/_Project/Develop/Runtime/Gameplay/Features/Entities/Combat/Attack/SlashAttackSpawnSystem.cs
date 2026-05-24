using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class SlashAttackSpawnSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ProjectileFactory _projectileFactory;

        private Transform _shootPoint;
        private Entity _owner;

        private IDisposable _disposable;

        public SlashAttackSpawnSystem(ProjectileFactory projectileFactory)
        {
            _projectileFactory = projectileFactory;
        }

        public void OnInit(Entity entity)
        {
            _owner = entity;
            _shootPoint = entity.ShootPoint;
            _disposable = entity.SpawnChargedSlashAtackEvent.Subscribe(SpawnSlashProjectile);
        }

        private void SpawnSlashProjectile()
        {
            _projectileFactory.CreateChargedSlashProjectile(_shootPoint, _owner);
        }

        public void OnDispose()
        {
            _disposable?.Dispose();
        }
    }
}