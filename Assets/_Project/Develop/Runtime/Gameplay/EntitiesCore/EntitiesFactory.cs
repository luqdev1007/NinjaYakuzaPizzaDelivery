using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HangWall;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
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

            AddHeroComponents(entity, config);
            AddHeroConditions(entity, config);
            AddHeroSystems(entity, config);

            return entity;
        }

        private void AddHeroComponents(Entity entity, MaiHeroConfig config)
        {
            entity
                // — общее —
                .AddMinFallVelocityForAction(new ReactiveVariable<float>(config.MinFallVelocityForAction))
                .AddIsGrounded()
                .AddGroundMask(config.GroundMask)

                // — движение —
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
                .AddMoveSpeedMin(new ReactiveVariable<float>(config.MoveSpeedMin))
                .AddAcceleration(new ReactiveVariable<float>(config.Acceleration))
                .AddDeceleration(new ReactiveVariable<float>(config.Deceleration))
                .AddIsMoving()

                // — прыжок —
                .AddJumpForce(new ReactiveVariable<float>(config.JumpForce))
                .AddJumpForceMax(new ReactiveVariable<float>(config.JumpForceMax))
                .AddJumpChargeTime(new ReactiveVariable<float>(config.JumpChargeTime))
                .AddJumpsAvailable(new ReactiveVariable<int>(config.MaxJumps))
                .AddMaxJumps(new ReactiveVariable<int>(config.MaxJumps))

                // — рывок —
                .AddIsDashing()
                .AddDashForceMin(new ReactiveVariable<float>(config.DashForceMin))
                .AddDashForceMax(new ReactiveVariable<float>(config.DashForceMax))
                .AddDashChargeTime(new ReactiveVariable<float>(config.DashChargeTime))
                .AddDashCooldown(new ReactiveVariable<float>(config.DashCooldown))
                .AddDashDuration(new ReactiveVariable<float>(config.DashDuration))

                // — планирование —
                .AddIsGliding()
                .AddGlideMaxFallSpeed(new ReactiveVariable<float>(config.GlideMaxFallSpeed))
                .AddGlideSpeedDamping(new ReactiveVariable<float>(config.GlideSpeedDamping))
                .AddGlideBounceForce(new ReactiveVariable<float>(config.GlideBounceForce))
                .AddGlideSnapSpeed(new ReactiveVariable<float>(config.GlideSnapSpeed))
                .AddGlideSnapDuration(new ReactiveVariable<float>(config.GlideSnapDuration))

                // — атака —
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInAttackProcess()
                .AddAttackProcessInitialTime(new ReactiveVariable<float>(config.AttackProcessTime))
                .AddAttackProcessCurrentTime()
                .AddAttackDelayTime(new ReactiveVariable<float>(config.AttackDelayTime))
                .AddAttackDelayEndEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.InstantAttackDamage))
                .AddAttackCanceledEvent()
                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.AttackCooldown))
                .AddAttackCooldownCurrentTime()
                .AddInAttackCooldown()
                .AddAttackRange(new ReactiveVariable<float>(config.AttackRange))

                // — броски —
                .AddIsThrowing()
                .AddIsGrappling()
                .AddCurrentThrowableIndex(new ReactiveVariable<int>(0))
                .AddGrappleCharges(new ReactiveVariable<int>(config.GrappleConfig.MaxCharges))
                .AddShurikenCharges(new ReactiveVariable<int>(config.ShurikenConfig.MaxCharges))
                .AddSleepDartCharges(new ReactiveVariable<int>(config.SleepDartConfig.MaxCharges))

                // — вис на стене —
                .AddIsWallHanging()
                .AddWallHangLayer(config.WallHangLayer)
                .AddWallHangSlideSpeed(new ReactiveVariable<float>(config.WallHangSlideSpeed))
                .AddWallJumpForce(new ReactiveVariable<Vector2>(config.WallJumpForce))
                .AddWallDirection()

                // — слайд —
                .AddIsSliding()
                .AddSlideDuration(new ReactiveVariable<float>(config.SlideDuration))
                .AddSlideSpeed(new ReactiveVariable<float>(config.SlideSpeed))

                // — пике —
                .AddIsPlunging()
                .AddPlungeSpeed(new ReactiveVariable<float>(config.PlungeSpeed))
                .AddPlungeAOERadius(new ReactiveVariable<float>(config.PlungeAOERadius))
                .AddPlungeAOEDamage(new ReactiveVariable<float>(config.PlungeAOEDamage))
                .AddPlungeKnockbackForce(new ReactiveVariable<float>(config.PlungeKnockbackForce))

                // — наклонные поверхности —
                .AddIsOnSlope()
                .AddSlopeBoostMultiplier(new ReactiveVariable<float>(config.SlopeBoostMultiplier))
                .AddSlopeJumpForce(new ReactiveVariable<Vector2>(config.SlopeJumpForce))
                .AddSlopeMask(config.SlopeMask)

                // — жизненный цикл —
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddIsDead()
                .AddInDeathProcess()
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessCurrentTime()
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddSpawnInitialTime(new ReactiveVariable<float>(config.SpawnProcessTime))
                .AddSpawnCurrentTime()
                .AddInSpawnProcess()
                ;
        }

        private void AddHeroConditions(Entity entity, MaiHeroConfig config)
        {
            // — движение —
            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsOnSlope.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // — прыжок —
            ICompositeCondition canJump = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.JumpsAvailable.Value > 0));

            // — рывок —
            ICompositeCondition canDash = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() =>
                    entity.IsGrounded.Value ||
                    entity.Rigidbody.linearVelocity.y >= entity.MinFallVelocityForAction.Value));

            // — планирование —
            ICompositeCondition canGlide = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsThrowing.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // — атака —
            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false))
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition mustCancelAttack = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == true))
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == true))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == true));

            // — броски —
            ICompositeCondition canGrapple = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsThrowing.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // — вис на стене —
            ICompositeCondition canWallHang = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == true))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // — слайд —
            ICompositeCondition canSlide = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // — пике —
            ICompositeCondition canPlunge = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // — жизненный цикл —
            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            entity
                .AddCanMove(canMove)
                .AddCanJump(canJump)
                .AddCanDash(canDash)
                .AddCanGlide(canGlide)
                .AddCanStartAttack(canStartAttack)
                .AddMustCancelAttack(mustCancelAttack)
                .AddCanGrapple(canGrapple)
                .AddCanWallHang(canWallHang)
                .AddCanSlide(canSlide)
                .AddCanPlunge(canPlunge)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;
        }

        private void AddHeroSystems(Entity entity, MaiHeroConfig config)
        {
            IInputService inputService = _container.Resolve<IInputService>();
            ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            var throwableBehaviourFactory = new ThrowableBehaviourFactory(coroutinesPerformer);

            entity
                // — инициализация —
                .AddSystem(new SpawnProcessTimerSystem())
                .AddSystem(new PlayerInputSystem(inputService))
                .AddSystem(new GroundCheckSystem(coyoteTime: 0.1f))

                // — движение —
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new JumpSystem(inputService))
                .AddSystem(new DashSystem(inputService, coroutinesPerformer))
                .AddSystem(new GlideSystem(inputService))
                .AddSystem(new WallHangSystem(inputService))
                .AddSystem(new SlideSystem(inputService, coroutinesPerformer))
                .AddSystem(new PlungeSystem(inputService, config.EnemyMask))
                .AddSystem(new SlopeSystem(inputService, coroutinesPerformer))

                // — броски —
                .AddSystem(new ThrowableSystem(
                    inputService,
                    coroutinesPerformer,
                    new ThrowableConfig[] { config.GrappleConfig, config.ShurikenConfig, config.SleepDartConfig },
                    throwableBehaviourFactory))

                // — атака —
                .AddSystem(new AttackCancelSystem())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new MeleeAttackHitSystem(config.EnemyMask, config.HitBounceForce))

                // — урон / жизненный цикл —
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())

                // — визуал —
                .AddSystem(new FlipDirectionSystem())

                // — последней всегда —
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                ;
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