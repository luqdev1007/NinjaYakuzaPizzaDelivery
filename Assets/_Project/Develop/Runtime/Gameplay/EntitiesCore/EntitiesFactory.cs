using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HangWall;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
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

                // — драйв (баг-фича) —
                .AddIsDriveActive(new ReactiveVariable<bool>(false))
                .AddDriveAccumulatedTime(0f)

                // — движение —
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(config.Movement.MoveSpeed))
                .AddMoveSpeedMin(new ReactiveVariable<float>(config.Movement.MoveSpeedMin))
                .AddAcceleration(new ReactiveVariable<float>(config.Movement.Acceleration))
                .AddDeceleration(new ReactiveVariable<float>(config.Movement.Deceleration))
                .AddIsMoving()

                // — прыжок —
                .AddJumpForce(new ReactiveVariable<float>(config.Jump.JumpForce))
                .AddJumpForceMax(new ReactiveVariable<float>(config.Jump.JumpForceMax))
                .AddJumpChargeTime(new ReactiveVariable<float>(config.Jump.JumpChargeTime))
                .AddJumpsAvailable(new ReactiveVariable<int>(config.Jump.MaxJumps))
                .AddMaxJumps(new ReactiveVariable<int>(config.Jump.MaxJumps))
                .AddJumpEvent()
                .AddDoubleJumpEvent()

                // — рывок —
                .AddIsDashing()
                .AddDashForceMin(new ReactiveVariable<float>(config.Dash.ForceMin))
                .AddDashForceMax(new ReactiveVariable<float>(config.Dash.ForceMax))
                .AddDashChargeTime(new ReactiveVariable<float>(config.Dash.ChargeTime))
                .AddDashCooldown(new ReactiveVariable<float>(config.Dash.Cooldown))
                .AddDashDuration(new ReactiveVariable<float>(config.Dash.Duration))
                .AddAirDashMultiplier(new ReactiveVariable<float>(config.Dash.AirMultiplier))
                .AddAirDashVerticalBoost(new ReactiveVariable<float>(config.Dash.VerticalBoost))
                .AddDashDamage(new ReactiveVariable<float>(config.Dash.Damage))
                .AddDashHitboxSize(new ReactiveVariable<Vector2>(config.Dash.HitboxSize))

                // — планирование —
                .AddIsGliding()
                .AddGlideHorizontalDrag(new ReactiveVariable<float>(config.Glide.HorizontalDrag))
                .AddGlideMaxFallSpeed(new ReactiveVariable<float>(config.Glide.MaxFallSpeed))
                .AddGlideSpeedDamping(new ReactiveVariable<float>(config.Glide.SpeedDamping))
                .AddGlideBounceForce(new ReactiveVariable<float>(config.Glide.BounceForce))
                .AddGlideSnapSpeed(new ReactiveVariable<float>(config.Glide.SnapSpeed))
                .AddGlideSnapDuration(new ReactiveVariable<float>(config.Glide.SnapDuration))

                // — атака —
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInAttackProcess()
                .AddAttackProcessInitialTime(new ReactiveVariable<float>(config.Attack.ProcessTime))
                .AddAttackProcessCurrentTime()
                .AddAttackDelayTime(new ReactiveVariable<float>(config.Attack.DelayTime))
                .AddAttackDelayEndEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.Attack.InstantDamage))
                .AddAttackDamage(new ReactiveVariable<float>(config.Attack.InstantDamage))
                .AddAttackCanceledEvent()
                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.Attack.Cooldown))
                .AddAttackCooldownCurrentTime()
                .AddInAttackCooldown()
                .AddAttackRange(new ReactiveVariable<float>(config.Attack.Range))

                .AddAttackInvulnerabilityDuration(new ReactiveVariable<float>(config.Attack.InvulnerabilityDuration))
                .AddAttackInvulnerabilityTimer()
                .AddIsAttackInvulnerable(new ReactiveVariable<bool>(false))

                // — броски —
                .AddIsThrowing()
                .AddIsGrappling()
                .AddCurrentThrowableIndex(new ReactiveVariable<int>(0))
                .AddGrappleCharges(new ReactiveVariable<int>(config.Throwables.GrappleConfig.MaxCharges))
                .AddShurikenCharges(new ReactiveVariable<int>(config.Throwables.ShurikenConfig.MaxCharges))
                .AddSleepDartCharges(new ReactiveVariable<int>(config.Throwables.SleepDartConfig.MaxCharges))

                // — вис на стене —
                .AddIsWallHanging()
                .AddWallHangLayer(config.WallHang.Layer)
                .AddWallHangSlideSpeed(new ReactiveVariable<float>(config.WallHang.SlideSpeed))
                .AddWallJumpForce(new ReactiveVariable<Vector2>(config.WallHang.JumpForce))
                .AddWallDirection()

                // — слайд —
                .AddIsSliding()
                .AddSlideDuration(new ReactiveVariable<float>(config.Slide.Duration))
                .AddSlideSpeed(new ReactiveVariable<float>(config.Slide.Speed))

                // — пике —
                .AddIsPlunging()
                .AddPlungeSpeed(new ReactiveVariable<float>(config.Plunge.Speed))
                .AddPlungeAOERadius(new ReactiveVariable<float>(config.Plunge.AOERadius))
                .AddPlungeAOEDamage(new ReactiveVariable<float>(config.Plunge.AOEDamage))
                .AddPlungeKnockbackForce(new ReactiveVariable<float>(config.Plunge.KnockbackForce))

                // — наклонные поверхности —
                .AddIsOnSlope()
                .AddSlopeBoostMultiplier(new ReactiveVariable<float>(config.Slope.BoostMultiplier))
                .AddSlopeJumpForce(new ReactiveVariable<Vector2>(config.Slope.JumpForce))
                .AddSlopeAccumSpeed(new ReactiveVariable<float>(0f))
                .AddSlopeMask(config.Slope.Mask)

                // — жизненный цикл —
                .AddMaxHealth(new ReactiveVariable<float>(config.LifeCycle.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.LifeCycle.MaxHealth))
                .AddIsDead()
                .AddInDeathProcess()
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.LifeCycle.DeathProcessTime))
                .AddDeathProcessCurrentTime()
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddDamageCooldown(new ReactiveVariable<float>(1.0f))
                .AddDamageCooldownTimer(new ReactiveVariable<float>(0f))
                .AddSpawnInitialTime(new ReactiveVariable<float>(config.LifeCycle.SpawnProcessTime))
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
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition canFlip = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                // .Add(new FuncCondition(() => entity.IsOnSlope.Value == false))
                 ;

            // — прыжок —
            ICompositeCondition canJump = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsDriveActive.Value == false)) 
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
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsAttackInvulnerable.Value == false))
                .Add(new FuncCondition(() => entity.DamageCooldownTimer.Value <= 0))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            entity
                .AddCanMove(canMove)
                .AddCanFlip(canFlip)
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

            ThrowableBehaviourFactory throwableBehaviourFactory = new ThrowableBehaviourFactory(coroutinesPerformer);

            SlopeSystem slopeSystem = new SlopeSystem();

            entity
                // — инициализация —
                .AddSystem(new SpawnProcessTimerSystem())
                .AddSystem(new PlayerInputSystem(inputService))
                .AddSystem(new GroundCheckSystem(coyoteTime: 0.1f))

                // — логика драйва —
                .AddSystem(new DriveSystem()) // Добавляем сюда

                // — движение —
                .AddSystem(new RigidbodyMovementSystem(inputService))
                .AddSystem(new JumpSystem(inputService, slopeSystem))
                .AddSystem(new DashSystem(inputService, coroutinesPerformer, config.Attack.EnemyMask, _container.Resolve<AudioService>()))
                .AddSystem(new GlideSystem(inputService))
                .AddSystem(new WallHangSystem(inputService))
                .AddSystem(new SlideSystem(inputService, coroutinesPerformer, slopeSystem))
                .AddSystem(new PlungeSystem(inputService, config.Attack.EnemyMask, _container.Resolve<AudioService>()))
                .AddSystem(slopeSystem)


                // — броски —
                .AddSystem(new ThrowableSystem(
                    inputService,
                    coroutinesPerformer,
                    new ThrowableConfig[]
                    {
                        config.Throwables.GrappleConfig,
                        config.Throwables.ShurikenConfig,
                        config.Throwables.SleepDartConfig
                    },
                    throwableBehaviourFactory))

                // — атака —
                .AddSystem(new AttackCancelSystem())
                .AddSystem(new StartAttackSystem(inputService))
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new AttackInvulnerabilitySystem())
                .AddSystem(new MeleeAttackHitSystem(config.Attack.EnemyMask, 
                                                    config.Attack.HitBounceForce, 
                                                    _container.Resolve<ICoroutinesPerformer>(),
                                                    _container.Resolve<AudioService>()))

                // — урон / жизненный цикл —
                .AddSystem(new ApplyDamageSystem("MainHero", _container.Resolve<AudioService>()))
                .AddSystem(new DamageKnockbackSystem())
                .AddSystem(new DeathSystem("MainHero", _container.Resolve<AudioService>()))
                .AddSystem(new DeathProcessTimerSystem())

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

        public Entity CreateGhost(Vector3 at, GhostConfig ghostConfig)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, at, ghostConfig.PrefabPath);

            entity
                .AddLinearDrag(new ReactiveVariable<float>(ghostConfig.LinearDrag))
                .AddAngularDrag(new ReactiveVariable<float>(ghostConfig.AngularDrag))

                // — Движение —
                .AddMoveDirection()
                .AddRotationDirection()
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(ghostConfig.MovementSpeed))

                // — Боёвка —
                .AddBodyContactDamage(new ReactiveVariable<float>(ghostConfig.ContactDamage))
                .AddContactsDetectingMask(LayersAPI.LayerMaskCharacters)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(16))
                .AddContactEntitiesBuffer(new Buffer<Entity>(16))

                // — Жизнь —
                .AddMaxHealth(new ReactiveVariable<float>(ghostConfig.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(ghostConfig.MaxHealth))
                .AddIsDead()
                .AddInDeathProcess()
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(ghostConfig.DeathProcessTime))
                .AddDeathProcessCurrentTime()
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                // Возвращаем компоненты кулдауна, так как ApplyDamageSystem ожидает их наличие в OnInit
                .AddDamageCooldown(new ReactiveVariable<float>(0.1f)) // Небольшой кулдаун, чтобы звук не спамил
                .AddDamageCooldownTimer(new ReactiveVariable<float>(0f))

                // — Эффекты (Сон) —
                .AddIsAsleep()
                .AddSleepTimer(new ReactiveVariable<float>(0f))
                ;

            // — Условия —

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.HasComponent<IsGrappledTarget>() == false || entity.IsGrappledTarget.Value == false))
                .Add(new FuncCondition(() => entity.IsAsleep.Value == false))
                ;

            // Условие получения урона: не мертв и кулдаун прошел
            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.DamageCooldownTimer.Value <= 0))
                .Add(new FuncCondition(() => entity.IsAsleep.Value == false))
                ;

            ICompositeCondition canFlip = new CompositeCondition()
                .Add(new FuncCondition(() => true))
                ;

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0))
                ;

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false))
                ;

            entity
                .AddCanMove(canMove)
                .AddCanPhysicalyInteract(canApplyDamage)
                .AddCanFlip(canFlip)
                .AddCanApplyDamage(canApplyDamage)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                ;

            entity
                // Системы логики
                .AddSystem(new PhysicsStabilizationSystem())
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())
                .AddSystem(new TransformMovementSystem())
                .AddSystem(new FlipDirectionSystem())

                // Системы урона
                .AddSystem(new ApplyDamageSystem("Ghost", _container.Resolve<AudioService>()))
                .AddSystem(new DamageKnockbackSystem())
                .AddSystem(new DeathSystem("Ghost", _container.Resolve<AudioService>()))
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))

                // Таймеры эффектов
                .AddSystem(new SleepTimerSystem())
                ;

            return entity;
        }
    }
}