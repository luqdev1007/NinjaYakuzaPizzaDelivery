using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HangWall;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesFactory
    {
        private readonly DIContainer _container;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly MonoEntitiesFactory _monoEntitiesFactory;
        private readonly CollidersRegistryService _collidersRegistryService;

        public EntitiesFactory(DIContainer container)
        {
            _container = container;
            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = container.Resolve<MonoEntitiesFactory>();
            _collidersRegistryService = container.Resolve<CollidersRegistryService>();
        }

        // ─── HERO ────────────────────────────────────────────────────────────

        public Entity CreateHero(Vector3 position, MaiHeroConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            // components
            entity
                // movement
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
                .AddMoveSpeedMin(new ReactiveVariable<float>(config.MoveSpeedMin))
                .AddAcceleration(new ReactiveVariable<float>(config.Acceleration))
                .AddDeceleration(new ReactiveVariable<float>(config.Deceleration))
                .AddIsMoving()

                // jump
                ;

            // conditions

            // move condition
            ICompositeCondition canMove = new CompositeCondition().Add(new FuncCondition(() => true));
            entity.AddCanMove(canMove);

            // jump condition
            ICompositeCondition canJump = new CompositeCondition().Add(new FuncCondition(() => true));


            // systems
            entity
                .AddSystem(new RigidbodyMovementSystem(_container.Resolve<IInputService>()))
            ;

            return entity;
        }

        // ─── OTHER ──────────────────────────────────────────────────────────

        public Entity CreateContactTrigger(Vector3 position)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, position, "Entities/ContactTrigger");

            entity
                .AddContactsDetectingMask(LayersAPI.LayerMaskCharacters)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))
                ;

            entity
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                ;

            _entitiesLifeContext.Add(entity);
            return entity;
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────

        private Entity CreateEmpty() => new Entity();
    }
}