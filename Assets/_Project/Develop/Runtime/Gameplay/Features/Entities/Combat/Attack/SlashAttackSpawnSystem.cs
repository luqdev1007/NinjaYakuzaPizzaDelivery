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

        private Rigidbody2D _rigidbody;

        private ReactiveVariable<Vector2> _recoilForce;

        private IDisposable _disposable;

        public SlashAttackSpawnSystem(ProjectileFactory projectileFactory)
        {
            _projectileFactory = projectileFactory;
        }

        public void OnInit(Entity entity)
        {
            _owner = entity;
            _shootPoint = entity.ShootPoint;

            _rigidbody = entity.Rigidbody;
            _recoilForce = entity.RecoilForce;

            _disposable = entity.SpawnChargedSlashAtackEvent.Subscribe(SpawnSlashProjectile);
        }

        private void SpawnSlashProjectile()
        {
            _projectileFactory.CreateChargedSlashProjectile(_shootPoint, _owner);
            ApplyRecoil();
        }

        private void ApplyRecoil()
        {
            float lookDir = _owner.LookDirectionX.Value;

            Vector2 recoilForce = new Vector2(-lookDir * _recoilForce.Value.x, _recoilForce.Value.y);

            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocityX, 0f);
            _rigidbody.AddForce(recoilForce, ForceMode2D.Impulse);
        }

        public void OnDispose()
        {
            _disposable?.Dispose();
        }
    }
}