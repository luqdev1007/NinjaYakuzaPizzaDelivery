using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Combat.HitImpact;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Contact;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Gadgets.Glider;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Explosion;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Plunge;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HitStop;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.Features.InventoryFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Visual;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.RandomManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
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
        private readonly IInputService _inputService;

        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public EntitiesFactory(DIContainer container)
        {
            _container = container;

            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = container.Resolve<MonoEntitiesFactory>();

            _collidersRegistryService = container.Resolve<CollidersRegistryService>();
            _inputService = container.Resolve<IInputService>();

            _coroutinesPerformer = container.Resolve<ICoroutinesPerformer>();
        }

        // HERO
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
            entity
                // intents (input)
                .AddIntentMovement()
                .AddIntentJump()
                .AddIntentDash()
                .AddIntentSlide()
                .AddIntentAttack()
                .AddIntentGrapple()
                .AddIntentUseItem()
                .AddIntentSwitchItemDelta()
                .AddIntentAimDirection()

                // spawn
                .AddInSpawnProcess()
                .AddSpawnInitialTime(new ReactiveVariable<float>(config.LifeCycle.SpawnProcessTime))
                .AddSpawnCurrentTime(new ReactiveVariable<float>())

                // death
                .AddIsDead()
                .AddInDeathProcess()
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.LifeCycle.DeathProcessTime))
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>())

                // health
                .AddCurrentHealth(new ReactiveVariable<float>(config.LifeCycle.MaxHealth))
                .AddMaxHealth(new ReactiveVariable<float>(config.LifeCycle.MaxHealth))

                // common
                .AddBaseGravityScale(new ReactiveVariable<float>(entity.Rigidbody.gravityScale))
                .AddFallActionThreshold(new ReactiveVariable<float>(config.FallActionThreshold))
                .AddGroundMask(config.GroundMask)
                .AddIsGrounded()
                .AddLookDirectionX(new ReactiveVariable<float>(1))
                .AddCurrentMovementState(new ReactiveVariable<MovementStates>(MovementStates.Default))

                // movement
                .AddIsMoving()
                .AddMoveDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(config.Movement.MoveSpeed))
                .AddMoveSpeedMin(new ReactiveVariable<float>(config.Movement.MoveSpeedMin))
                .AddAcceleration(new ReactiveVariable<float>(config.Movement.Acceleration))
                .AddDeceleration(new ReactiveVariable<float>(config.Movement.Deceleration))

                // jump         
                .AddJumpChargeTime(new ReactiveVariable<float>(config.Jump.MaxChargeTime))
                .AddJumpForceMin(new ReactiveVariable<float>(config.Jump.ForceMin))
                .AddJumpForceMax(new ReactiveVariable<float>(config.Jump.ForceMax))
                .AddJumpRequest()
                .AddJumpEvent()

                // air jump         
                .AddAirJumpChargeTime(new ReactiveVariable<float>(config.AirJump.MaxChargeTime))
                .AddAirJumpForceMin(new ReactiveVariable<float>(config.AirJump.ForceMin))
                .AddAirJumpForceMax(new ReactiveVariable<float>(config.AirJump.ForceMax))
                .AddAirJumpsCount(new ReactiveVariable<int>(config.AirJump.JumpsMaxCount))
                .AddAirJumpsMaxCount(new ReactiveVariable<int>(config.AirJump.JumpsMaxCount))
                .AddAirJumpRequest()
                .AddAirJumpEvent()

                // wall jumping
                .AddIsWallJumping()
                .AddWallJumpForceMultiplier(new ReactiveVariable<Vector2>(config.WallJump.ForceMultiplier))
                .AddWallMask(config.WallJump.WallMask)
                .AddLockoutDuration(new ReactiveVariable<float>(config.WallJump.ControlLockDuration))

                // dash
                .AddIsDashing()
                .AddDashChargeTimeMax(new ReactiveVariable<float>(config.Dash.MaxChargeTime))
                .AddDashForceMin(new ReactiveVariable<float>(config.Dash.ForceMin))
                .AddDashForceMax(new ReactiveVariable<float>(config.Dash.ForceMax))
                .AddDashDuration(new ReactiveVariable<float>(config.Dash.Duration))
                .AddDashCooldown(new ReactiveVariable<float>(config.Dash.Cooldown))
                .AddAirDashMultiplier(new ReactiveVariable<float>(config.Dash.AirMultiplier))
                .AddAirDashVerticalBoost(new ReactiveVariable<float>(config.Dash.VerticalBoost))

                // slope
                .AddSlopeAngle()
                .AddSlopeNormal()
                .AddIsOnSlope()
                .AddSlopeMinAngle(new ReactiveVariable<float>(config.Slope.MinAngle))
                .AddSlopeMaxAngle(new ReactiveVariable<float>(config.Slope.MaxAngle))
                .AddSlopeMaxStableAngle(new ReactiveVariable<float>(config.Slope.MaxStableAngle))
                .AddSlopeSlipForce(new ReactiveVariable<float>(config.Slope.SlipForce))
                .AddSlopeBaseSlideSpeed(new ReactiveVariable<float>(config.Slope.BaseSlideSpeed))
                .AddSlopeSlideAcceleration(new ReactiveVariable<float>(config.Slope.SlideAcceleration))
                .AddSlopeMaxSlideSpeed(new ReactiveVariable<float>(config.Slope.MaxSlideSpeed))
                .AddMinFallVelocityForAutoSlide(new ReactiveVariable<float>(config.Slope.MinFallVelocityForAutoSlide))

                // slope jump
                .AddBaseSlopeJumpForce(new ReactiveVariable<float>(config.Slope.BaseJumpForce))
                .AddSlopeJumpForceModifier(new ReactiveVariable<Vector2>(config.Slope.JumpForceModifier))
                .AddSlopeJumpEvent(new ReactiveEvent<float>())

                // bounce (трамплин шлёт сюда просьбу на отскок — см. F5)
                .AddBounceImpulseRequest(new ReactiveEvent<BounceImpulseData>())

                // explosion (взрыв призрака-камикадзе шлёт сюда просьбу на импульс)
                .AddExplosionImpulseRequest(new ReactiveEvent<Vector2>())

                // slide
                .AddIsSliding()
                .AddSlideSpeed(new ReactiveVariable<float>(config.Slide.Speed))
                .AddSlideDuration(new ReactiveVariable<float>(config.Slide.Duration))
                .AddSlideCooldown(new ReactiveVariable<float>(config.Slide.Cooldown))
                .AddSlideHitBoxSize(new ReactiveVariable<Vector2>(config.Slide.HitBoxSize))

                // glide
                .AddIsGliding()
                .AddGlideGravityScale(new ReactiveVariable<float>(config.Glide.GravityScale))
                .AddGlideMaxFallSpeed(new ReactiveVariable<float>(config.Glide.MaxFallSpeed))
                .AddGlideSpeedDamping(new ReactiveVariable<float>(config.Glide.SpeedDamping))
                .AddGlideSnapSpeed(new ReactiveVariable<float>(config.Glide.SnapSpeed))
                .AddGlideSnapDuration(new ReactiveVariable<float>(config.Glide.SnapDuration))
                .AddGlideHorizontalDrag(new ReactiveVariable<float>(config.Glide.HorizontalDrag))

                // plunge
                .AddIsPlunging()
                .AddPlungeAccelerationMultiplier(new ReactiveVariable<float>(config.Plunge.AccelerationMultiplier))
                .AddPlungeSpeed(new ReactiveVariable<float>(config.Plunge.Speed))
                .AddPlungeImpactEvent(new ReactiveEvent<float>())
                .AddMinPlungeImpactSpeedThreshold(new ReactiveVariable<float>(config.Plunge.MinImpactSpeedThreshold))

                .AddPlungeLandImpactRange(new ReactiveVariable<float>(config.Plunge.DamageRange))
                .AddPlungeLandImpactDamage(new ReactiveVariable<float>(config.Plunge.Damage))
                .AddPlungeLandImpactHitMask(config.Plunge.HitLayer)

                // wall hang
                .AddWallHangLayer(config.WallHang.Layer)
                .AddIsWallHanging()
                .AddWallDirection()
                .AddWallHangSlideSpeed(new ReactiveVariable<float>(config.WallHang.SlideSpeed))
                .AddWallJumpForce(new ReactiveVariable<Vector2>(config.WallHang.JumpForce))

                // grapple
                .AddIsGrappling()
                .AddGrappleHookTransform()
                .AddGrappleAnchorPoint()
                .AddGrappleAnchoredEvent()

                // attack
                .AddStartAttackEvent(new ReactiveEvent())
                .AddEndAttackEvent(new ReactiveEvent())
                .AddAttackDelayEndEvent(new ReactiveEvent())
                .AddSuccessfulHitEvent(new ReactiveEvent())
                .AddInAttackProcess(new ReactiveVariable<bool>(false))
                .AddInAttackCooldown(new ReactiveVariable<bool>(false))
                .AddAttackProcessInitialTime(new ReactiveVariable<float>(config.Attack.ProcessTime))
                .AddAttackProcessCurrentTime(new ReactiveVariable<float>(0f))
                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.Attack.Cooldown))
                .AddAttackCooldownCurrentTime(new ReactiveVariable<float>(0f))
                .AddAttackDelayTime(new ReactiveVariable<float>(config.Attack.DelayTime))
                .AddAttackDamage(new ReactiveVariable<float>(config.Attack.Damage))
                .AddAttackRange(new ReactiveVariable<float>(config.Attack.Range))
                .AddAttackHitMask(new ReactiveVariable<LayerMask>(config.Attack.EnemyMask))
                .AddAttackKnocback(new ReactiveVariable<Vector2>(config.Attack.AttackKnockback))

                // juggle
                .AddAerialHangForce(new ReactiveVariable<Vector2>(config.Attack.AerialHangForce))

                // apply damage
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddDamageCooldown(new ReactiveVariable<float>(config.LifeCycle.DamageCooldown))
                .AddDamageCooldownTimer()

                // attack effects
                .AddAttackInvulnerabilityDuration(new ReactiveVariable<float>(config.Attack.InvulnerabilityDuration))
                .AddAttackInvulnerabilityTimer()
                .AddIsAttackInvulnerable()

                // double attack
                .AddDoubleAttackChance(new ReactiveVariable<float>(config.Attack.DoubleAttackChance))
                .AddDoubleAttackCurrentCooldown()
                .AddDoubleAttackInitialCooldown(new ReactiveVariable<float>(config.Attack.DoubleAttackCooldown))

                // attack hit stop
                .AddAttackHitStopDuration(new ReactiveVariable<float>(config.Attack.HitStopDuration))
                .AddAttackHitStopScale(new ReactiveVariable<float>(config.Attack.HitStopScale))

                // charged slash attack
                .AddRecoilForce(new ReactiveVariable<Vector2>(config.Attack.RecoilForce))
                .AddIsChargingSlashAttack()
                .AddSpawnChargedSlashAtackEvent()
                .AddChargeSlashAttackCurrentTimer()
                .AddChargeSlashAttackRequiredTimer(new ReactiveVariable<float>(config.Attack.SlashAttackChargeRequiredTime))

                // body contact
                .AddContactsDetectingMask(config.ContactLayerMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(16))
                .AddContactEntitiesBuffer(new Buffer<Entity>(16))
                .AddSpeedDamageDealtEvent()

                // inventory
                .AddCurrentItemIndex()
                .AddIsUsingItem()
                .AddItemUsedEvent()
                .AddInventoryCharges(new List<ReactiveVariable<int>>())
            ;
        }

        private void AddHeroConditions(Entity entity, MainHeroConfig config)
        {
            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsWallJumping.Value == false))
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canJump = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == true))
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canAirJump = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.AirJumpsCount.Value > 0))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                .Add(new FuncCondition(() => entity.IsWallJumping.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.Rigidbody.linearVelocity.y >= entity.FallActionThreshold.Value))
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canGlide = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                .Add(new FuncCondition(() =>
                entity.IsGliding.Value 
                || entity.Rigidbody.linearVelocity.y < entity.FallActionThreshold.Value));

            ICompositeCondition mustRestoreAirJumpsCount_mustRestoreAirJumpsCount = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsGrounded.Value 
                || entity.IsWallHanging.Value 
                || entity.IsGliding.Value)) // new!
                .Add(new FuncCondition(() => entity.AirJumpsCount.Value < entity.AirJumpsMaxCount.Value));

            ICompositeCondition canDash = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() =>
                    entity.IsGrounded.Value ||
                    entity.Rigidbody.linearVelocity.y >= entity.FallActionThreshold.Value));

            ICompositeCondition canFlip = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            ICompositeCondition canSlide = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == true))
                .Add(new FuncCondition(() => entity.IsOnSlope.Value == false))
                .Add(new FuncCondition(() => entity.IsMoving.Value == true)) // new!
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canSlopeSlip = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.CurrentMovementState.Value == MovementStates.Default));

            ICompositeCondition canSlopeJump = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsOnSlope.Value == true))
                .Add(new FuncCondition(() => entity.CurrentMovementState.Value == MovementStates.Sliding));


            ICompositeCondition canPlunge = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false)) 
                .Add(new FuncCondition(() => entity.IsWallJumping.Value == false)) 
                .Add(new FuncCondition(() => entity.IsGliding.Value == false));

            ICompositeCondition canWallHang = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrounded.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false)) 
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition canGrapple = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false)) 
                .Add(new FuncCondition(() => entity.IsGliding.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false))
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false))
                .Add(new FuncCondition(() => entity.IsChargingSlashAttack.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsAttackInvulnerable.Value == false))
                .Add(new FuncCondition(() => entity.DamageCooldownTimer.Value <= 0))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            ICompositeCondition canDoubleAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.DoubleAttackCurrentCooldown.Value <= 0))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGliding.Value == false));

            ICompositeCondition canChargeSlashAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsPlunging.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsWallHanging.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false));

            entity
                .AddCanPlunge(canPlunge)
                .AddCanMove(canMove)
                .AddCanJump(canJump)
                .AddCanAirJump(canAirJump)
                .AddMustRestoreAirJumpsCount(mustRestoreAirJumpsCount_mustRestoreAirJumpsCount)
                .AddCanDash(canDash)
                .AddCanSlide(canSlide)
                .AddCanSlopeSlip(canSlopeSlip)
                .AddCanFlip(canFlip)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanSlopeJump(canSlopeJump)
                .AddCanGlide(canGlide)
                .AddCanWallHang(canWallHang)
                .AddCanGrapple(canGrapple)
                .AddCanStartAttack(canStartAttack)
                .AddCanApplyDamage(canApplyDamage)
                .AddCanDoubleAttack(canDoubleAttack)
                .AddCanChargeSlashAttack(canChargeSlashAttack)
                ;
        }
        private void AddHeroSystems(Entity entity, MainHeroConfig config)
        {
            GrappleHookConfig grappleConfig = config.Throwables.GrappleConfig;

            entity
                // common
                .AddSystem(new PlayerInputSystem(_inputService)) 

                .AddSystem(new SpawnProcessTimerSystem()) 

                .AddSystem(new SurfaceCheckSystem()) 

                // movement
                .AddSystem(new RigidbodyMovementSystem())

                .AddSystem(new JumpSystem()) 

                .AddSystem(new AirJumpSystem())
                .AddSystem(new AirJumpsRecoverySystem())

                .AddSystem(new WallJumpSystem())

                // bounce
                .AddSystem(new BounceSystem())

                // explosion
                .AddSystem(new ExplosionImpulseSystem())

                .AddSystem(new DashSystem())

                // slope
                .AddSystem(new SlopeSlipSystem())
                .AddSystem(new SlopeSlideSystem())
                .AddSystem(new SlopeJumpSystem())

                // slide
                .AddSystem(new SlideSystem())

                // glide
                .AddSystem(new GlideSystem())

                // plunge
                .AddSystem(new PlungeSystem())
                .AddSystem(new PlungeDamageOnImpactSystem(_collidersRegistryService))

                // wall hang
                .AddSystem(new WallHangSystem())

                // grapple
                .AddSystem(new GrappleSystem(grappleConfig, _coroutinesPerformer))

                // attack
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new MeleeAttackHitSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new AttackInvulnerabilitySystem())

                // attack effects
                .AddSystem(new HitStopSystem(_container.Resolve<HitStopService>(), _container.Resolve<CameraService>()))
                .AddSystem(new DoubleAttackSystem(_container.Resolve<IGameplayRandom>()))
                .AddSystem(new DoubleAttackCooldownSystem())

                // slash attack
                .AddSystem(new SlashAttackChargeSystem())
                .AddSystem(new SlashAttackSpawnSystem(_container.Resolve<ProjectileFactory>()))

                // juggle
                .AddSystem(new AerialHitSuspensionSystem())

                 // apply damage
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new ApplyDamageCooldownSystem())

                // тело ниндзя - смертельное оружие
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new LethalContactMovementSystem())

                // inventory
                .AddSystem(new InventorySystem(
                    _container.Resolve<ConfigsProviderService>().GetConfig<PlayerInventoryConfig>().StartingConsumables,
                    _container.Resolve<ProjectileFactory>()))

                // visual
                .AddSystem(new FlipDirectionSystem()) 

                // death
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())

                // — последней всегда —
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext)) 
                ;
        }


        // SCRIPTED OBJECTS
        public Entity CreateContactTrigger(Vector3 position)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, position, "Entities/Common/FinalPointTrigger");

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


        // CREATURES
        public Entity CreateGhost(Vector3 at, GhostConfig ghostConfig)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, at, ghostConfig.PrefabPath);

            entity
                // Common
                .AddLookDirectionX(new ReactiveVariable<float>(1))
                .AddKnockbackDuration(new ReactiveVariable<float>(ghostConfig.KnockbackDuration))
                .AddKnockbackElapsedTime(new ReactiveVariable<float>(ghostConfig.KnockbackDuration))

                // Physics
                .AddLinearDrag(new ReactiveVariable<float>(ghostConfig.LinearDrag))

                // Movement
                .AddIsMoving()
                .AddMoveDirection()
                .AddRotationDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(ghostConfig.MovementSpeed))

                // Combat
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddBodyContactDamage(new ReactiveVariable<float>(ghostConfig.ContactDamage))
                .AddContactsDetectingMask(ghostConfig.ContactLayerMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(16))
                .AddContactEntitiesBuffer(new Buffer<Entity>(16))

                .AddDamageCooldown(new ReactiveVariable<float>(ghostConfig.DamageCooldown)) 
                .AddDamageCooldownTimer(new ReactiveVariable<float>(0f))
               
                // LifeCycle
                .AddMaxHealth(new ReactiveVariable<float>(ghostConfig.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(ghostConfig.MaxHealth))

                .AddIsDead()
                .AddInDeathProcess()
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(ghostConfig.DeathProcessTime))
                .AddDeathProcessCurrentTime()

                // Effects
                .AddIsGrappledTarget()
                ;


            // — Условия —
            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappledTarget.Value == false))
                .Add(new FuncCondition(() => entity.KnockbackElapsedTime.Value >= entity.KnockbackDuration.Value))
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.DamageCooldownTimer.Value <= 0)) // расскоментил недавно!
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

            ICompositeCondition canPhysicallyInteract = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            entity
                .AddCanPhysicalyInteract(canPhysicallyInteract)
                .AddCanMove(canMove)
                .AddCanFlip(canFlip)
                .AddCanApplyDamage(canApplyDamage)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                ;

            entity

                .AddSystem(new DamageKnockbackTimerSystem())
                .AddSystem(new DamageKnockbackSystem())

                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new ApplyDamageCooldownSystem())

                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())

                .AddSystem(new PhysicsStabilizationSystem())
                .AddSystem(new SimpleRigidbodyMovementSystem())
                .AddSystem(new FlipDirectionSystem())

                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                ;

            return entity;
        }


        // HELPERS 
        private Entity CreateEmpty() => new Entity();

        // LOOT
        public Entity CreatePullable(LootConfig config, Vector3 position)
        {
            Entity entity = CreateEmpty();
            _monoEntitiesFactory.Create(entity, position, config.PrefabPath);


            return entity;
        }
    }
}