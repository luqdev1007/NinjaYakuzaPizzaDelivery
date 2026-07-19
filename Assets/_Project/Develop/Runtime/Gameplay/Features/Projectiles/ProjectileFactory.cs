using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities;
using UnityEngine;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles
{
    public class ProjectileFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly MonoEntitiesFactory _monoEntitiesFactory;
        private readonly CollidersRegistryService _collidersRegistryService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public ProjectileFactory(DIContainer container)
        {
            _container = container;

            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = container.Resolve<MonoEntitiesFactory>();
            _collidersRegistryService = container.Resolve<CollidersRegistryService>();
            _coroutinesPerformer = container.Resolve<ICoroutinesPerformer>();
        }

        public Entity CreateChargedSlashProjectile(Transform parent, Entity owner)
        {
            // settings (config)
            string prefabPath = "Entities/Projectiles/ChargedSlashProjectile";
            float damage = 10;
            float speed = 15;
            float lifeTime = 2;
            LayerMask hitMask = LayersAPI.LayerMaskEnemies;

            Entity entity = new Entity();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, prefabPath);
            mono.transform.SetParent(null);

            entity
                .AddMoveDirection(new ReactiveVariable<Vector2>(new Vector2(owner.LookDirectionX.Value, 0)))
                .AddLookDirectionX()
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(speed))

                .AddLifeTime(new ReactiveVariable<float>(lifeTime))

                .AddBodyContactDamage(new ReactiveVariable<float>(damage))

                .AddContactsDetectingMask(hitMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))

                .AddTeam(owner.Team)
                ;

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => true))
                ;

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.LifeTime.Value <= 0))
                ;

            entity
                .AddCanMove(canMove)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new DealDamageOnContactSystem())

                .AddSystem(new TransformMovementSystem())

                .AddSystem(new TimedLifeTimeSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        /// <summary>
        /// Язык слайма. Отдельная сущность, а НЕ дочерний узел слайма: катана
        /// мапит цель через GetComponentInParent&lt;MonoEntity&gt;, поэтому удар по
        /// дочернему языку был бы неотличим от удара по слайму. Плюс
        /// CollidersRegistryService.Register использует Dictionary.Add — повторная
        /// регистрация того же коллайдера бросила бы ArgumentException.
        ///
        /// Уставки СПЕЦИАЛЬНО не хардкодятся в теле метода (в соседних методах
        /// этой фабрики они захардкожены под комментарием "// settings (config)" —
        /// эту практику не наследуем): всё, что нужно знать языку, приезжает
        /// параметрами, остальным рулит TongueSystem на стороне слайма.
        ///
        /// Состав сущности намеренно минимальный. Здоровья, смерти, лута,
        /// самоосвобождения и контактной цепочки у языка нет: разрубаемость
        /// катаной обеспечивает один TakeDamageRequest, а MeleeAttackHitSystem
        /// на такой сущности отрабатывает штатно — Invoke по пустому списку
        /// подписчиков проходит, CurrentHealth никто не читает.
        /// Жизненным циклом языка управляет TongueSystem через EntitiesLifeContext.
        /// </summary>
        public Entity CreateSlimeTongue(Vector2 startPosition, string prefabPath)
        {
            Entity entity = new Entity();

            // Перегрузка с позицией уже инстанцирует без родителя, поэтому
            // SetParent(null) соседних методов здесь не нужен.
            _monoEntitiesFactory.Create(entity, (Vector3)startPosition, prefabPath);

            entity
                .AddTakeDamageRequest()
                .AddTongueOriginPoint(new ReactiveVariable<Vector2>(startPosition))
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public void CreateThrowableProjectile(ThrowableItemConfig throwableConfig, Transform parent, Vector2 aimDirection, Entity playerEntity)
        {
            Entity entity = new Entity();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, throwableConfig.PrefabPath);
            Vector3 startPosition = mono.transform.position;
            mono.transform.SetParent(null);

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            mono.transform.rotation = Quaternion.Euler(0, 0, angle);

            entity
                .AddMoveDirection(new ReactiveVariable<Vector2>(aimDirection))
                .AddLookDirectionX()
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(throwableConfig.ProjectileSpeed))

                .AddContactsDetectingMask(throwableConfig.HitMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))

                .AddTeam(playerEntity.Team);

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            ICompositeCondition mustSelfRelease = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => Vector3.Distance(startPosition, entity.Transform.position) >= throwableConfig.MaxFlyDistance));

            entity
                .AddCanMove(canMove)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new TransformMovementSystem())                                  
                .AddSystem(new BodyContactDetectingSystem())                        
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService)); 

            if (throwableConfig is ShurikenConfig shurikenConfig)
            {
                entity
                    .AddBodyContactDamage(new ReactiveVariable<float>(shurikenConfig.Damage))
                    .AddIsTouchDeathMask()
                    .AddDeathMask(shurikenConfig.HitMask);

                mustSelfRelease.Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true));

                entity
                    .AddSystem(new DealDamageOnContactSystem())        
                    .AddSystem(new DeathMaskTouchDetectorSystem());     
            }

            entity.AddSystem(new SelfReleaseSystem(_entitiesLifeContext)); 

            _entitiesLifeContext.Add(entity);
        }
    }
}
