using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature;
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
            IInputService inputService = _container.Resolve<IInputService>();
            ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            // components
            entity
                // common
                .AddGroundMask(config.GroundMask)
                .AddIsGrounded(new ReactiveVariable<bool>(false))

                // intents
                .AddIntentJump(new ReactiveVariable<bool>(false))
                .AddIntentDash(new ReactiveVariable<bool>(false))

                // movement
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
                .AddMoveSpeedMin(new ReactiveVariable<float>(config.MoveSpeedMin))
                .AddAcceleration(new ReactiveVariable<float>(config.Acceleration))
                .AddDeceleration(new ReactiveVariable<float>(config.Deceleration))
                .AddMoveDirection(new ReactiveVariable<Vector2>())
                .AddIsMoving()

                // jump
                .AddMaxExtraJumps(new ReactiveVariable<int>(config.MaxExtraJumps))
                .AddExtraJumpsAvailable(new ReactiveVariable<int>(config.MaxExtraJumps))
                .AddJumpForce(new ReactiveVariable<float>(config.JumpForceBase))
                .AddJumpForceMax(new ReactiveVariable<float>(config.JumpForceMax))
                .AddJumpChargeTime(new ReactiveVariable<float>(config.JumpChargeTime))
                .AddJumpEvent(new ReactiveEvent())
                .AddDoubleJumpEvent(new ReactiveEvent())
                .AddJumpRequest(new ReactiveEvent())

                // dash
                .AddIsDashing()
                .AddDashForceMin(new ReactiveVariable<float>(config.DashForceMin))
                .AddDashForceMax(new ReactiveVariable<float>(config.DashForceMax))
                .AddDashChargeTime(new ReactiveVariable<float>(config.DashChargeTime))
                .AddDashCooldown(new ReactiveVariable<float>(config.DashCooldown))
                .AddDashDuration(new ReactiveVariable<float>(config.DashDuration))
                .AddAirDashMultiplier(new ReactiveVariable<float>(config.AirDashMultiplier))
                .AddAirDashVerticalBoost(new ReactiveVariable<float>(config.AirDashVerticalBoost))
                .AddDashDamage(new ReactiveVariable<float>(config.DashDamage))
                .AddDashHitboxSize(new ReactiveVariable<Vector2>(config.DashHitboxSize))
                .AddDashRequest(new ReactiveEvent())

                // slide system
                .AddIsSliding()
                .AddSlideRequest(new ReactiveEvent())
                .AddSlideDuration(new ReactiveVariable<float>(config.SlideDuration))
                .AddSlideSpeed(new ReactiveVariable<float>(config.SlideSpeed))

                // glide system
                .AddIsGliding()
                .AddMinFallVelocityForAction(new ReactiveVariable<float>(config.MinFallVelocity))
                .AddGlideMaxFallSpeed(new ReactiveVariable<float>(config.GlideMaxFallSpeed))
                .AddGlideSpeedDamping(new ReactiveVariable<float>(config.GlideSpeedDamping))
                .AddGlideBounceForce(new ReactiveVariable<float>(config.GlideBounceForce))
                .AddGlideSnapSpeed(new ReactiveVariable<float>(config.GlideSnapSpeed))
                .AddGlideSnapDuration(new ReactiveVariable<float>(config.GlideSnapDuration))
                .AddGlideHorizontalDrag(new ReactiveVariable<float>(config.GlideHorizontalDrag))
                ;

                // conditions

                // move condition
                ICompositeCondition canMove = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                    .Add(new FuncCondition(() => entity.IsSliding.Value == false));

                entity.AddCanMove(canMove);

                // jump condition
                ICompositeCondition canJump = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                    .Add(new FuncCondition(() => entity.IsGrounded.Value == true))
                    .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                    ;

                entity.AddCanJump(canJump);

                // extra jump condition
                ICompositeCondition canExtraJump = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                    .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                    .Add(new FuncCondition(() => entity.MinFallVelocityForAction.Value < entity.Rigidbody.linearVelocityY))
                    .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                    ;

                entity.AddCanExtraJump(canJump);

                // dash condition
                ICompositeCondition canDash = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsGrounded.Value == true))
                    .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                    .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                    ;

                entity.AddCanDash(canDash);

                // slide condition
                ICompositeCondition canSlide = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsGrounded.Value == true))
                    .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                    .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                    ;

                entity.AddCanSlide(canSlide);

                // must restore extra jumps conditions
                ICompositeCondition mustRestoreExtraJumps = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsGrounded.Value == true))
                    .Add(new FuncCondition(() => entity.ExtraJumpsAvailable.Value != entity.MaxExtraJumps.Value))
                    ;
                entity.AddMustRestoreExtraJumps(mustRestoreExtraJumps);

                // glide condition
                ICompositeCondition canGlide = new CompositeCondition()
                    .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                    ;
                entity.AddCanGlide(canGlide);

                // systems
                entity
                    .AddSystem(new PlayerInputSystem(inputService))
                    .AddSystem(new GroundCheckSystem())
                    .AddSystem(new RigidbodyMovementSystem())
                    .AddSystem(new JumpSystem())
                    .AddSystem(new SlideSystem(coroutinesPerformer))
                    // .AddSystem(new DashSystem(coroutinesPerformer))
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