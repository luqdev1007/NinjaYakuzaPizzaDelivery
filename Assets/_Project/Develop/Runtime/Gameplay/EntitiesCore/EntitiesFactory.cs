using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Inventory;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.GlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HangWall;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.Features.JumpFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlideFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.View;
using Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.WindFeature;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesFactory
    {
        private readonly DIContainer _container;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly MonoEntitiesFactory _monoEntitiesFactory;
        private readonly CollidersRegistryService _collidersRegistryService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly AudioService _audioService;

        private readonly CameraService _cameraService;

        private readonly MasterLootProviderConfig _masterLootProviderConfig;

        public EntitiesFactory(DIContainer container)
        {
            _container = container;

            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = container.Resolve<MonoEntitiesFactory>();
            _collidersRegistryService = container.Resolve<CollidersRegistryService>();
            _audioService = container.Resolve<AudioService>();
            _coroutinesPerformer = container.Resolve<ICoroutinesPerformer>();

            _cameraService = _container.Resolve<CameraService>();

            _masterLootProviderConfig = container.Resolve<ConfigsProviderService>().GetConfig<MasterLootProviderConfig>();
        }


        // ─── HERO ────────────────────────────────────────────────────────────

        // ─── HERO ────────────────────────────────────────────────────────────

        public Entity CreateHero(Vector3 position, MainHeroConfig config)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            AddHeroComponents(entity, config);
            AddHeroConditions(entity, config);
            AddHeroSystems(entity, config);

            return entity;
        }

        private void AddHeroComponents(Entity entity, MainHeroConfig config)
        {
            var cometConfig = new CometDashData
            {
                MaxCharges = 3,
                MultiplierDegradation = 0.9f,
                BaseCooldown = 1f,
                OverheatCooldown = 6.0f
            };

            entity
                // — общее —
                .AddMinFallVelocityForAction(new ReactiveVariable<float>(config.MinFallVelocityForAction))
                .AddIsGrounded()
                .AddAudio(_container.Resolve<AudioService>())
                .AddGroundMask(config.GroundMask)

                // input
                .AddAttackInput()
                .AddDashInput()
                .AddJumpInput()
                .AddGrappleInput()
                .AddMouseWorldPositionInput()
                .AddMoveDirectionInput()
                .AddThrowInput()
                .AddInventoryScrollDelta()
                .AddShowTargetActive()
                .AddAutoTargetToggleRequest()
                .AddCycleTargetRequest(new ReactiveEvent())
                .AddUltimateRequest(new ReactiveEvent())
                .AddThrowProjectileRequest(new ReactiveEvent())

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

                // wall jump params
                .AddWallJumpLockTimer(new ReactiveVariable<float>(0f))
                .AddIsWallJumping(new ReactiveVariable<bool>(false))
                .AddWallJumpParams(
                    config.Wall.VelocityYAbs,
                    config.Wall.JumpForce,
                    config.Wall.ControlLockDuration
                )

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

                .AddCometDashState(
                    cometConfig,
                    new ReactiveVariable<int>(3),
                    new ReactiveVariable<float>(1f),
                    new ReactiveVariable<float>(0f)
                )

                // — планирование —
                .AddIsGliding()
                .AddGlideActive()
                .AddGlideCounterMultiplier(new ReactiveVariable<float>(config.Glide.CounterForceMultiplier))
                .AddGlideHorizontalDrag(new ReactiveVariable<float>(config.Glide.HorizontalDrag))
                .AddGlideMaxFallSpeed(new ReactiveVariable<float>(config.Glide.MaxFallSpeed))
                .AddGlideSpeedDamping(new ReactiveVariable<float>(config.Glide.SpeedDamping))
                .AddGlideBounceForce(new ReactiveVariable<float>(config.Glide.BounceForce))
                .AddGlideSnapSpeed(new ReactiveVariable<float>(config.Glide.SnapSpeed))
                .AddGlideSnapDuration(new ReactiveVariable<float>(config.Glide.SnapDuration))

                // — атака —
                .AddSuccessfulHitEvent()
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddInAttackProcess()
                .AddAttackProcessInitialTime(new ReactiveVariable<float>(config.Combat.ProcessTime))
                .AddAttackProcessCurrentTime()
                .AddAttackDelayTime(new ReactiveVariable<float>(config.Combat.DelayTime))
                .AddAttackDelayEndEvent()
                .AddInstantAttackDamage(new ReactiveVariable<float>(config.Combat.Damage))
                .AddAttackDamage(new ReactiveVariable<float>(config.Combat.Damage))
                .AddAttackCanceledEvent()
                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.Combat.Cooldown))
                .AddAttackCooldownCurrentTime()
                .AddInAttackCooldown()
                .AddAttackRange(new ReactiveVariable<float>(config.Combat.Range))
                .AddAttackEnemyMask(new ReactiveVariable<LayerMask>(config.Combat.EnemyMask))

                // Новые параметры сочности и физики из конфига
                .AddAttackHitStopScale(new ReactiveVariable<float>(config.Combat.HitStopScale))
                .AddAttackHitStopDuration(new ReactiveVariable<float>(config.Combat.HitStopDuration))
                .AddAttackHitBounceForce(new ReactiveVariable<float>(config.Combat.HitBounceForce))
                .AddGroundHitBounceModifiers(new ReactiveVariable<Vector2>(config.Combat.GroundHitBounceModifiers))
                .AddAirHitBounceModifiers(new ReactiveVariable<Vector2>(config.Combat.AirHitBounceModifiers))

                .AddAttackInvulnerabilityDuration(new ReactiveVariable<float>(config.Combat.InvulnerabilityDuration))
                .AddAttackInvulnerabilityTimer()
                .AddIsAttackInvulnerable(new ReactiveVariable<bool>(false))

                // — броски —
                .AddThrowEvent()
                .AddIsThrowing()
                .AddIsGrappling()
                .AddGrapplingHookActive()
                .AddCurrentThrowableIndex(new ReactiveVariable<int>(0))
                .AddGrappleCharges(new ReactiveVariable<int>(config.Throwables.GrappleItem.ProjectileSettings.MaxCharges))
                .AddShurikenCharges(new ReactiveVariable<int>(config.Throwables.ShurikenItem.ProjectileSettings.MaxCharges))
                .AddSleepDartCharges(new ReactiveVariable<int>(config.Throwables.SleepDartItem.ProjectileSettings.MaxCharges))

                // — вис на стене —
                .AddIsWallHanging()
                .AddWallHangLayer(config.Wall.WallLayer)
                .AddWallHangSlideSpeed(new ReactiveVariable<float>(config.Wall.SlideSpeed))
                .AddWallJumpForce(new ReactiveVariable<Vector2>(config.Wall.JumpForce))
                .AddWallDirection()

                // — слайд —
                .AddIsSliding()
                .AddSlideDuration(new ReactiveVariable<float>(config.Slide.Duration))
                .AddSlideSpeed(new ReactiveVariable<float>(config.Slide.Speed))
                .AddSlideRequest()

                 // — пике (Plunge) —
                .AddIsPlunging() 
                .AddPlungeActive(new ReactiveVariable<bool>(false))
                .AddPlungeSpeed(new ReactiveVariable<float>(config.Combat.PlungeSpeed))
                .AddPlungeAOERadius(new ReactiveVariable<float>(config.Combat.PlungeRadius))
                .AddPlungeAOEDamage(new ReactiveVariable<float>(config.Combat.PlungeDamage))
                .AddPlungeKnockbackForce(new ReactiveVariable<float>(config.Combat.PlungeKnockbackForce))

                // — наклонные поверхности —
                .AddIsOnSlope()
                .AddSlopeMask(config.Movement.SlopeMask)
                .AddSlopeMinAngle(new ReactiveVariable<float>(config.Movement.MinSlopeAngle))
                .AddSlopeMaxAngle(new ReactiveVariable<float>(config.Movement.MaxSlopeAngle))
                .AddSlopeDownhillBaseForce(new ReactiveVariable<float>(config.Slope.DownhillBaseForce))
                .AddSlopeBoostMultiplier(new ReactiveVariable<float>(config.Slope.BoostMultiplier))
                .AddSlopeMagnetForce(new ReactiveVariable<float>(config.Slope.MagnetForce))
                .AddSlopeMaxAccumSpeed(new ReactiveVariable<float>(config.Slope.MaxAccumSpeed))
                .AddSlopeAccumGainRate(new ReactiveVariable<float>(config.Slope.AccumGainRate))
                .AddSlopeAccumDecayRate(new ReactiveVariable<float>(config.Slope.AccumDecayRate))
                .AddSlopeSlideOffDelay(new ReactiveVariable<float>(config.Slope.SlideOffDelay))
                .AddSlopeMinEjectVelocity(new ReactiveVariable<float>(config.Slope.MinEjectVelocity))
                .AddSlopeEjectForceMultiplier(new ReactiveVariable<float>(config.Slope.EjectForceMultiplier))
                .AddSlopeAutoSlidePush(new ReactiveVariable<float>(config.Slope.AutoSlidePush))
                .AddSlopeJumpForce(new ReactiveVariable<Vector2>(config.Slope.JumpForce))
                .AddSlopeAccumSpeed(new ReactiveVariable<float>(0f))

                // лут
                .AddCollectRange(new ReactiveVariable<float>(config.LootCollectRange))
                .AddLootPickedEvent(new ReactiveEvent<LootType>())

                // — система стиля —
                .AddStylePoints(new ReactiveVariable<float>(0f))
                .AddStyleRank(new ReactiveVariable<StyleRankEnum>(StyleRankEnum.F))
                .AddStyleMultiplier(new ReactiveVariable<float>(1f))
                .AddStyleDecayTimer(new ReactiveVariable<float>(0f))
                .AddMoveFreshness(new Dictionary<string, float>())
                .AddMaxStylePoints(0f)
                .AddMaxStyleRank(StyleRankEnum.F)

                // — уточнение физики стен —
                .AddWallNormal(new ReactiveVariable<Vector2>(Vector2.zero))

                // — состояние восстановления кометы —
                .AddIsCometRecovering(new ReactiveVariable<bool>(false))

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

        private void AddHeroSystems(Entity entity, MainHeroConfig config)
        {
            IInputService inputService = _container.Resolve<IInputService>();
            ICoroutinesPerformer coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            AudioService audioService = _container.Resolve<AudioService>();
            DropLootService dropLootService = _container.Resolve<DropLootService>();
            ConfigsProviderService configsProviderService = _container.Resolve<ConfigsProviderService>();

            ThrowableBehaviourFactory throwableBehaviourFactory = new ThrowableBehaviourFactory(
                coroutinesPerformer, 
                audioService, 
                dropLootService, 
                configsProviderService);

            ConsumableConfig[] consumables = new ConsumableConfig[]
            {
                config.Throwables.ShurikenItem,
                config.Throwables.SleepDartItem
            };

            entity
                // — init/common —
                .AddSystem(new PlayerInputSystem(inputService))
                .AddSystem(new SpawnProcessTimerSystem())
                .AddSystem(new GroundCheckSystem())
                .AddSystem(new WindSystem())

                // — movements —
                .AddSystem(new JumpSystem())
                .AddSystem(new GlideSystem())
                .AddSystem(new WallHangSystem())
                .AddSystem(new SlideSystem(coroutinesPerformer))
                .AddSystem(new SlopeSystem())
                .AddSystem(new WallJumpSystem())
                .AddSystem(new RigidbodyMovementSystem())

                // combat
                .AddSystem(new DashSystem(coroutinesPerformer))
                .AddSystem(new PlungeSystem())
                .AddSystem(new CometRecoverySystem())

                // — hook —
                .AddSystem(new GrappleSystem(
                    coroutinesPerformer,
                    (GrappleHookConfig)config.Throwables.GrappleItem.ProjectileSettings, 
                    throwableBehaviourFactory,
                    _collidersRegistryService))

                // — inventory —
                .AddSystem(new InventorySystem(
                    consumables,
                    throwableBehaviourFactory,
                    coroutinesPerformer)) 

                // — атака —
                .AddSystem(new AttackCancelSystem())
                .AddSystem(new StartAttackSystem(this, _coroutinesPerformer))
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new AttackInvulnerabilitySystem())

                .AddSystem(new MeleeAttackHitSystem())
                .AddSystem(new HitStopSystem(_coroutinesPerformer))

                // — урон / жизненный цикл —
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DamageKnockbackSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())

                // — визуал —
                .AddSystem(new FlipDirectionSystem())

                // лут
                .AddSystem(new LootMagnetSystem(_collidersRegistryService))
                .AddSystem(new LootDistanceCollectSystem(_entitiesLifeContext))

                // hero style system
                .AddSystem(new HeroStyleSystem(
                    _container.Resolve<StyleEvaluator>(),
                    _container.Resolve<RankStyleService>()))

                // — последней всегда —
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                ;
        }

        private void AddHeroConditions(Entity entity, MainHeroConfig config)
        {
            // — движение —
            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.WallJumpLockTimer.Value <= 0));

            ICompositeCondition canFlip = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.WallJumpLockTimer.Value <= 0));

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
                .Add(new FuncCondition(() => entity.Rigidbody.linearVelocityY < config.MinFallVelocityForAction)) // new
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

        // ENEMIES
        public Entity CreateGhost(Vector3 at, GhostConfig ghostConfig)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, at, ghostConfig.PrefabPath);

            entity
                .AddAudio(_audioService)
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

                .AddDamageCooldown(new ReactiveVariable<float>(0.1f)) 
                .AddDamageCooldownTimer(new ReactiveVariable<float>(0f))

                // — Эффекты (Сон) —
                .AddIsAsleep()
                .AddIsGrappledTarget(new ReactiveVariable<bool>(false))
                .AddSleepTimer(new ReactiveVariable<float>(0f))
                .AddDamageKnockbackForceX(new ReactiveVariable<float>(1f))
                .AddDamageKnockbackForceY(new ReactiveVariable<float>(2f))
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
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DamageKnockbackSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))

                // Таймеры эффектов
                .AddSystem(new SleepTimerSystem())
                ;

            // LOOT

            entity
                .AddLootIsDropped(new ReactiveVariable<bool>(false));

            ICompositeCondition canDropLoot = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            entity.AddCanDropLoot(canDropLoot);

            LootTableConfig lootTable = ghostConfig.LootTable;

            entity.AddSystem(new DropLootSystem(
                _container.Resolve<DropLootService>(), 
                lootTable,
                _container.Resolve<SecretChestCollectService>()));
            // LOOT

            return entity;
        }

        // LOOT
        public Entity CreatePullable(LootConfig config, Vector3 position)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);

            entity
                .AddInSpawnProcess(new ReactiveVariable<bool>(true))
                .AddSpawnCurrentTime(new ReactiveVariable<float>(config.SpawnDuration))
                .AddSpawnInitialTime(new ReactiveVariable<float>(config.SpawnDuration))

                .AddAutoDeleteCurrentTime(new ReactiveVariable<float>(config.LifeTime))
                .AddAutoDeleteInitialTime(new ReactiveVariable<float>(config.LifeTime))

                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
                .AddIsCollected(new ReactiveVariable<bool>(false))
                .AddCurrentTarget(new ReactiveVariable<Entity>(null));

            ICompositeCondition moveCondition = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition mustSelfRelease = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => entity.IsCollected.Value == true))
                .Add(new CompositeCondition(LogicOperations.And)
                    .Add(new FuncCondition(() => entity.AutoDeleteCurrentTime.Value <= 0))
                    .Add(new FuncCondition(() => entity.CurrentTarget.Value == null))
                    .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                );

            entity
                .AddCanMove(moveCondition)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new SpawnProcessTimerSystem())
                .AddSystem(new AutoDeleteTimerSystem())
                .AddSystem(new LootArcMovementSystem(config.TravelTime, config.ArcHeight)) 
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _entitiesLifeContext.Add(entity);
            return entity;
        }

        public Entity CreateChest(Vector3 position, LootTableConfig lootTable)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, position, "Entities/Loot/SecretChest");

            entity.AddAudio(_audioService);

            entity
                .AddCurrentHealth(new ReactiveVariable<float>(1f)) 
                .AddDamageCooldown(new ReactiveVariable<float>(0.5f))
                .AddDamageCooldownTimer(new ReactiveVariable<float>(0f))
                .AddTakeDamageRequest(new ReactiveEvent<DamageData>())
                .AddTakeDamageEvent(new ReactiveEvent<DamageData>())
                .AddLootIsDropped(new ReactiveVariable<bool>(false));

            ICompositeCondition canDropLoot = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.LootIsDropped.Value == false));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.LootIsDropped.Value == true));

            entity.AddMustSelfRelease(mustSelfRelease);

            entity.AddCanDropLoot(canDropLoot);
            entity.AddCanApplyDamage(canApplyDamage);

            entity.AddIsSecretChest();

            entity
                .AddSystem(new ApplyDamageSystem())

                .AddSystem(new DropLootSystem(
                    _container.Resolve<DropLootService>(), 
                    lootTable, 
                    _container.Resolve<SecretChestCollectService>()))
                
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                ;

            _entitiesLifeContext.Add(entity);
            return entity;
        }

        // PROJECTILES
        public Entity CreateChargedSlashProjectile(Transform parent, float damage, Vector2 direction, Entity owner)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, "Entities/Projectiles/ChargedSlashProjectile");

            ParticleSystem slashEffectPS = mono.transform.GetComponentInChildren<ParticleSystem>();

            Vector3 localScale = parent.parent.localScale;
            localScale.y = 1.5f;
            localScale.x *= -1;
            slashEffectPS.transform.localScale = localScale;

            mono.transform.SetParent(null);

            float minSpeed = 20;
            float impulseSpeed = Mathf.Abs(owner.Rigidbody.linearVelocityX) * 2;
            impulseSpeed = minSpeed; //  Mathf.Max(minSpeed, impulseSpeed);

            entity
                .AddAutoDeleteCurrentTime(new ReactiveVariable<float>(3f))
                .AddAutoDeleteInitialTime(new ReactiveVariable<float>(3f))

                .AddMoveDirection(new ReactiveVariable<Vector2>(direction))
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(impulseSpeed))

                .AddContactCollidersBuffer(new Buffer<Collider2D>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))
                .AddBodyContactDamage(new ReactiveVariable<float>(damage))
                .AddContactsDetectingMask(LayersAPI.LayerMaskEnemies)

                .AddChargedSlashProjectileTag()

                /*
                .AddTeam(owner.Team)

                .AddDeathMask(LayersAPI.LayerMaskEnemies)
                .AddIsTouchDeathMask()
                .AddIsTouchAnotherTeam()
                */
                ;

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => true))
                ;

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.AutoDeleteCurrentTime.Value <= 0))
                ;

            entity
                .AddCanMove(canMove)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new DealDamageOnContactSystem())

                .AddSystem(new SimpleRigidbodyMovementSystem())
                .AddSystem(new AutoDeleteTimerSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _entitiesLifeContext.Add(entity);

            Debug.Log("Created, timer: " + entity.AutoDeleteCurrentTime.Value);

            return entity;
        }
    }
}