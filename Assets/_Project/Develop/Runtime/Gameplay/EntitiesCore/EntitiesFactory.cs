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
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Contact;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Explosion;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Gadgets.Glider;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Explosion;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.HangWall;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Patrol;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Plunge;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slope;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Tether;
using Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.HitStop;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.Features.InventoryFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Tongue;
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

                // tether (язык слайма шлёт сюда просьбу на захват — та же схема,
                // что у bounce выше: запрос-компонент на герое, применяет
                // система героя. Контракт писателей — в TetherComponents.cs)
                .AddIsTethered()
                .AddTetherAnchorPoint()
                .AddTetherRequest(new ReactiveEvent<TetherData>())
                .AddTetherReleaseRequest(new ReactiveEvent<TetherReleaseReason>())

                // разруб языка — сигнал для очков стиля, писатель TongueSystem
                .AddTongueCutEvent(new ReactiveEvent())

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
                // Две механики тяги одновременно недопустимы: обе пишут в
                // linearVelocity героя, и арбитра между ними нет. Плюс
                // GrappleSystem подменяет gravityScale и восстанавливает его
                // своим Release — на пересечении это восстановление прилетело
                // бы посреди активного захвата.
                .Add(new FuncCondition(() => entity.IsTethered.Value == false))
                .Add(new FuncCondition(() => entity.InSpawnProcess.Value == false));

            // Можно ли зацепить героя языком. Проверяется НА СТОРОНЕ СЛАЙМА
            // в момент попадания: условие ложно — язык просто втягивается,
            // как на этапе 3a.
            //
            // IsPlunging В СПИСКЕ НЕТ — ЭТО ДИЗАЙН-РЕШЕНИЕ, НЕ ЗАБЫТАЯ СТРОКА.
            // Выдернуть героя из пике языком — желаемое поведение: пике это
            // мощный самонаводящийся коммит вниз, и возможность сорвать его
            // языком единственное, что делает слайма угрозой для игрока,
            // спамящего пике. НЕ "чинить".
            //
            // IsDashing/IsSliding, наоборот, дают иммунитет: это короткие
            // окна, которыми игрок платит за проход сквозь язык, и в них он
            // язык РУБИТ (LethalContactMovementSystem) — захват поверх этого
            // был бы наказанием за успешное исполнение.
            ICompositeCondition canBeTethered = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.IsDashing.Value == false))
                .Add(new FuncCondition(() => entity.IsSliding.Value == false))
                .Add(new FuncCondition(() => entity.IsGrappling.Value == false))
                .Add(new FuncCondition(() => entity.IsTethered.Value == false))
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
                .AddCanBeTethered(canBeTethered)
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

                // tether (язык слайма). Стоит РЯДОМ С ГРЭПЛОМ и ПОСЛЕ
                // RigidbodyMovementSystem осознанно: обе механики тяги
                // накладываются поверх уже посчитанной базовой скорости,
                // на том же fixed-канале. Уставок из конфига здесь нет —
                // они приезжают в TetherRequest от конкретного слайма.
                .AddSystem(new TetherPullSystem())

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


        // Призрак-камикадзе. Базовая сборка целиком переиспользуется из CreateGhost:
        // AngryGhostConfig наследует GhostConfig, а PrefabPath в ассете указывает на
        // AngryGhost, поэтому MonoEntitiesFactory поднимет нужный префаб.
        public Entity CreateAngryGhost(Vector3 at, AngryGhostConfig config)
        {
            Entity entity = CreateGhost(at, config);

            entity
                // Агро
                .AddDetectionRadius(new ReactiveVariable<float>(config.DetectionRadius))
                .AddChaseSpeed(new ReactiveVariable<float>(config.ChaseSpeed))
                .AddIsAgro(new ReactiveVariable<bool>(false))

                // Взведение
                .AddArmingRadius(new ReactiveVariable<float>(config.ArmingRadius))
                .AddDisarmRadius(new ReactiveVariable<float>(config.DisarmRadius))
                .AddArmingDuration(new ReactiveVariable<float>(config.ArmingDuration))
                .AddArmingTimer(new ReactiveVariable<float>(0f))
                .AddIsArming(new ReactiveVariable<bool>(false))

                // Взрыв
                .AddExplosionRadius(new ReactiveVariable<float>(config.ExplosionRadius))
                .AddExplosionDamage(new ReactiveVariable<float>(config.ExplosionDamage))
                .AddExplosionKnockbackForce(new ReactiveVariable<float>(config.ExplosionKnockbackForce))
                .AddForcedExplosionKnockbackMultiplier(
                    new ReactiveVariable<float>(config.ForcedExplosionKnockbackMultiplier))
                .AddExplosionLayerMask(config.ExplosionLayerMask)
                .AddDetonationRequest(new ReactiveEvent<DetonationKind>())
                .AddDetonationEvent(new ReactiveEvent<DetonationKind>())
                .AddHasDetonated(new ReactiveVariable<bool>(false))
                ;

            // Ужесточаем условие движения ПОСТ-ФАКТУМ: CanMove собран внутри
            // CreateGhost, здесь к нему доклеивается ещё одна проверка. Add на
            // ICompositeCondition кладёт условие в конец и склеивает стандартной
            // операцией (And), поэтому семантика — «всё, что было, И не взводимся».
            //
            // Во время взведения SimpleRigidbodyMovementSystem вообще не пишет
            // скорость, а PhysicsStabilizationSystem гасит остаточную — призрак
            // замирает физически, а не только обнулением MoveDirection.
            entity.CanMove.Add(new FuncCondition(() => entity.IsArming.Value == false));

            entity
                .AddSystem(new AgroDetectionSystem(_container.Resolve<MainHeroHolderService>()))
                .AddSystem(new ArmingTimerSystem())
                .AddSystem(new ExplosionSystem(_collidersRegistryService))

                // ОТСТУПЛЕНИЕ ОТ ЗАДАННОГО ПОРЯДКА, осознанное.
                // Требовалось подписать ForcedDetonationSystem на IsDead РАНЬШЕ
                // DisableCollidersOnDeathSystem. Недостижимо без правки CreateGhost:
                // та регистрирует DisableCollidersOnDeathSystem внутри себя, а
                // Entity.AddSystem умеет только дописывать в конец — вставки нет.
                // Рефакторить сигнатуру CreateGhost ради этого не стали.
                //
                // Функционально порядок не влияет: DisableCollidersOnDeathSystem
                // гасит СОБСТВЕННЫЕ коллайдеры призрака, а ExplosionSystem читает
                // чужие через CollidersRegistryService и свою сущность пропускает
                // явной проверкой target == _selfEntity.
                .AddSystem(new ForcedDetonationSystem())
                ;

            return entity;
        }


        // Наземный патрульный. Собран ПО ОБРАЗЦУ CreateGhost, но три системы
        // призрака здесь отсутствуют намеренно:
        //
        //   SimpleRigidbodyMovementSystem — рулит по НАПРАВЛЕНИЮ и никогда не
        //     останавливается сама, поэтому маршрут с концами ею не выражается.
        //     Заменена на TargetPointMovementSystem, которая едет К ТОЧКЕ.
        //
        //   PhysicsStabilizationSystem — была бы вторым тиковым писателем
        //     linearVelocity и вдобавок гасила бы движение к нулю. Её роль
        //     (осаждение арки knockback) здесь исполняет сама рулёжка: по
        //     окончании окна тело возвращается к точке маршрута.
        //
        //   SurfaceCheckSystem — не добавляется и не нужна. Слой Enemies(9) не
        //     сталкивается с Ground(8)/Wall(10)/Slope(11), тело живёт с
        //     gravityScale = 0, никакой «земли» под ним физически нет. Высоту
        //     задают точки маршрута, которые дизайнер ставит руками.
        //
        // ОТЛИЧИЕ ОТ ПРИЗРАКА, НА КОТОРОЕ СТОИТ ОБРАТИТЬ ВНИМАНИЕ:
        // здесь есть DeathProcessTimerSystem, а у призрака её нет. Без неё
        // InDeathProcess остаётся false навсегда, mustSelfRelease срабатывает в
        // тот же тик, что и смерть, и DeathProcessTime из конфига оказывается
        // мёртвым полем — труп исчезает мгновенно. Слайму окно смерти нужно:
        // на префабе есть анимация смерти и VFX.
        public Entity CreateSlime(Vector3 at, SlimeConfig slimeConfig, PatrolRoute? patrolRoute)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, at, slimeConfig.PrefabPath);

            PatrolRoute route = ResolvePatrolRoute(at, slimeConfig, patrolRoute);

            entity
                // Common
                .AddLookDirectionX(new ReactiveVariable<float>(1))
                .AddKnockbackDuration(new ReactiveVariable<float>(slimeConfig.KnockbackDuration))
                .AddKnockbackElapsedTime(new ReactiveVariable<float>(slimeConfig.KnockbackDuration))

                // Movement
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(slimeConfig.MovementSpeed))

                // Patrol
                .AddPatrolPointA(route.PointA)
                .AddPatrolPointB(route.PointB)
                .AddMoveTargetPoint(new ReactiveVariable<Vector2>(route.PointA))
                .AddPatrolPauseMin(slimeConfig.PatrolPauseMin)
                .AddPatrolPauseMax(slimeConfig.PatrolPauseMax)
                .AddPatrolArriveDistance(slimeConfig.PatrolArriveDistance)

                // Combat
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddBodyContactDamage(new ReactiveVariable<float>(slimeConfig.ContactDamage))
                .AddContactsDetectingMask(slimeConfig.ContactLayerMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(16))
                .AddContactEntitiesBuffer(new Buffer<Entity>(16))

                .AddDamageCooldown(new ReactiveVariable<float>(slimeConfig.DamageCooldown))
                .AddDamageCooldownTimer(new ReactiveVariable<float>(0f))

                // LifeCycle
                .AddMaxHealth(new ReactiveVariable<float>(slimeConfig.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(slimeConfig.MaxHealth))

                .AddIsDead()
                .AddInDeathProcess()
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(slimeConfig.DeathProcessTime))
                .AddDeathProcessCurrentTime()

                // Tongue.
                // Реактивны только те два флага, у которых есть читатель: CanMove
                // читает IsTongueActive, TongueTelegraphView — IsTongueTelegraphing.
                // Остальное — уставки из конфига, они не меняются после сборки
                // сущности, поэтому нереактивны (то же решение, что по
                // PatrolPointA/PatrolPointB).
                .AddIsTongueActive(new ReactiveVariable<bool>(false))
                .AddIsTongueTelegraphing(new ReactiveVariable<bool>(false))

                .AddTongueRange(slimeConfig.MaxRange)
                .AddTongueSpeed(slimeConfig.GrabAttackSpeed)
                .AddTongueRetractSpeed(slimeConfig.GrabBackSpeed)
                .AddTongueTelegraphDuration(slimeConfig.TongueTelegraphDuration)
                .AddTongueCooldown(slimeConfig.TongueCooldown)
                .AddTongueAimArcDot(slimeConfig.TongueAimArcDot)
                .AddSightBlockMask(slimeConfig.SightBlockMask)
                .AddTargetMask(slimeConfig.TargetMask)
                .AddTonguePrefabPath(slimeConfig.TonguePrefabPath)

                // Уставки притяга. Живут на слайме, а НЕ на герое: это
                // параметры конкретного врага, они уезжают герою в TetherData
                // в момент захвата.
                .AddTongueGrabTime(slimeConfig.GrabTime)
                .AddTetherPullSpeed(slimeConfig.TetherPullSpeed)
                .AddTetherPullAcceleration(slimeConfig.TetherPullAcceleration)
                ;


            // — Условия —
            // Состав CanMove сверен с призраком, за вычетом IsGrappledTarget:
            // тот компонент слайму не выдаётся и переиспользовать его нельзя —
            // у него другая семантика и он читается в CanMove призрака
            // (правило single-writer).
            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.KnockbackElapsedTime.Value >= entity.KnockbackDuration.Value))
                ;

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
                .AddCanFlip(canFlip)
                .AddCanApplyDamage(canApplyDamage)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                ;

            // Ужесточаем условие движения ПОСТ-ФАКТУМ, ровно как в
            // CreateAngryGhost с IsArming: Add на ICompositeCondition кладёт
            // условие в конец и склеивает стандартной операцией (And).
            //
            // Мозг и PatrolState при этом НЕ трогаются: слайм продолжает «хотеть»
            // идти к точке маршрута, а исполняющая система движения загашена
            // условием. Мозг решает, системы исполняют, условия гасят.
            entity.CanMove.Add(new FuncCondition(() => entity.IsTongueActive.Value == false));

            entity
                .AddSystem(new DamageKnockbackTimerSystem())
                .AddSystem(new DamageKnockbackSystem())

                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new ApplyDamageCooldownSystem())

                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())

                .AddSystem(new TargetPointMovementSystem())
                .AddSystem(new FlipDirectionSystem())

                // ProjectileFactory резолвится отложенно, в теле метода (образец —
                // SlashAttackSpawnSystem в AddHeroSystems): в конструкторе
                // EntitiesFactory её ещё нет. EntitiesLifeContext брать из
                // контейнера не нужно — он уже лежит в поле фабрики.
                .AddSystem(new TongueSystem(
                    _container.Resolve<ProjectileFactory>(),
                    _entitiesLifeContext,
                    _container.Resolve<MainHeroHolderService>()))

                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                ;

            return entity;
        }

        // Маршрут со сцены либо запасной отрезок вокруг точки спавна.
        //
        // Проверка вырожденности (совпавшие концы) живёт НЕ здесь, а в
        // GameplayBootstrap: только там на руках есть Transform маркера, а значит
        // и путь объекта в иерархии для внятного warning'а. Сюда вырожденный
        // маршрут уже не доезжает — приезжает null.
        private PatrolRoute ResolvePatrolRoute(Vector3 at, SlimeConfig slimeConfig, PatrolRoute? patrolRoute)
        {
            if (patrolRoute.HasValue)
            {
                return patrolRoute.Value;
            }

            float halfRange = slimeConfig.PatrolFallbackHalfRange;

            Vector2 pointA = new Vector2(at.x - halfRange, at.y);
            Vector2 pointB = new Vector2(at.x + halfRange, at.y);

            return new PatrolRoute(pointA, pointB);
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