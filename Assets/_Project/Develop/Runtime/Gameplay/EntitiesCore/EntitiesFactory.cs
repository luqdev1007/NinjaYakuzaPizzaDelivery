using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.Meta.Features.ShipUpgrades;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
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


        // main heroes
        public Entity CreateWizard(Transform parent, WizardConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddRotationSpeed(new ReactiveVariable<float>(config.RotationSpeed))
                .AddRotationDirection()

                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddIsDead()
                .AddInDeathProcess()

                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessCurrentTime()

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddAttackProcessInitialTime(new ReactiveVariable<float>(config.AttackProcessTime))
                .AddAttackProcessCurrentTime()
                .AddInAttackProcess()

                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddAttackCanceledEvent()

                .AddAttackDelayTime(new ReactiveVariable<float>(config.AttackDelayTime))
                .AddAttackDelayEndEvent()

                .AddInstantAttackDamage(new ReactiveVariable<float>(config.InstantAttackDamage))

                .AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.AttackCooldown))
                .AddAttackCooldownCurrentTime()
                .AddInAttackCooldown();

            ICompositeCondition canRotate = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false))
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false));

            ICompositeCondition mustCancelAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true));

            entity
                .AddCanRotate(canRotate)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                .AddCanStartAttack(canStartAttack)
                .AddMustCancelAttack(mustCancelAttack);

            entity
                  .AddSystem(new RigidbodyRotationSystem())

                  .AddSystem(new InstantShootSystem(_container.Resolve<ProjectilesFactory>(), _container.Resolve<ConfigsProviderService>()))
                  .AddSystem(new StartAttackSystem())
                  .AddSystem(new AttackProcessTimerSystem())
                  .AddSystem(new AttackDelayEndTriggerSystem())
                  .AddSystem(new AttackCooldownTimerSystem())
                  .AddSystem(new EndAttackSystem())
                  .AddSystem(new AttackCancelSystem())

                  .AddSystem(new ApplyDamageSystem())

                  .AddSystem(new DeathSystem())
                  .AddSystem(new DeathProcessTimerSystem())
                  .AddSystem(new DisableCollidersOnDeathSystem())
                  .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            return entity;
        }

        public Entity CreateEngineer(Transform parent, EngineerConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent();

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            return entity;
        }

        public Entity CreateCaptain(Transform parent, CaptainConfig config)
        {
            Entity entity = CreateEmpty();

            _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddMoveDirection()
                .AddRotationDirection()
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed));

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddCanMove(canMove)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            return entity;
        }
        // main heroes


        // enemies
        public Entity CreateSoldier(Transform parent, SoldierConfig config)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));


            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity

                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            return entity;
        }

        public Entity CreateDriver(Transform parent, DriverConfig config)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));


            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            return entity;
        }

        public Entity CreateArcher(Transform parent, ArcherConfig config)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddDeathMask(LayersAPI.LayerMaskWater)
                .AddIsTouchDeathMask()

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
            ;

            return entity;
        }
        // enemies


        // vehicles
        public Entity CreateMainShip(Transform parent, MainShipConfig config)
        {
            Entity entity = CreateEmpty();

            float baseHeight = 12;
            Vector3 basePosition = Vector3.up * baseHeight;

            _monoEntitiesFactory.Create(entity, parent == null ? basePosition : parent.position, config.PrefabPath);

            // saved data
            PlayerMainShipDataProvider shipData = _container.Resolve<PlayerMainShipDataProvider>();

            entity
                .AddMaxHealth(new ReactiveVariable<float>(shipData.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(shipData.MaxHealth))

                .AddIsDead(new ReactiveVariable<bool>())
                .AddInDeathProcess(new ReactiveVariable<bool>(false))
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddMainShipTag();

                /*
                .AddMoveSinkSpeed(new ReactiveVariable<float>(config.MoveSinkSpeed))
                .AddRotationSinkSpeed(new ReactiveVariable<float>(config.RotationSinkSpeed))
                ;
                */

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                ;

            entity
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                .AddSystem(new ReleaseChildEntitiesSystem(_entitiesLifeContext))

                .AddSystem(new ApplyDamageSystem())

                //.AddSystem(new ShipSinkingSystem())
                ;

            return entity;
        }
        
        public Entity CreateSmallShip(Transform at, SmallShipConfig config)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, at, config.PrefabPath);

            Vector3 randomOfsset = new Vector3(Random.Range(-30, 10), 0, Random.Range(-30, 10));
            mono.transform.position += randomOfsset;
            mono.transform.SetParent(null);

            entity
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
                .AddRotationSpeed(new ReactiveVariable<float>(config.RotationSpeed))
                .AddMoveDirection(new ReactiveVariable<Vector3>())
                .AddRotationDirection(new ReactiveVariable<Vector3>())

                .AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))

                .AddBodyContactDamage(new ReactiveVariable<float>(config.BodyContactDamage))
                .AddContactsDetectingMask(LayersAPI.LayerMaskHittable)
                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))

                .AddIsTouchAnotherTeam()

                .AddIsDead(new ReactiveVariable<bool>(false))
                .AddInDeathProcess()
                .AddDeathProcessCurrentTime(new ReactiveVariable<float>(config.DeathProcessTime))
                .AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))

                .AddTakeDamageRequest()
                .AddTakeDamageEvent()

                .AddCurrentTarget()

                .AddMoveSinkSpeed(new ReactiveVariable<float>(config.MoveSinkSpeed))
                .AddRotationSinkSpeed(new ReactiveVariable<float>(config.RotationSinkSpeed))
                ;

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canRotate = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false));

            ICompositeCondition mustDie = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

            ICompositeCondition mustExplode = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsTouchAnotherTeam.Value == true));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true))
                .Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

            entity
                .AddCanMove(canMove)
                .AddCanRotate(canRotate)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease)
                .AddCanApplyDamage(canApplyDamage)
                .AddMustExplode(mustExplode)
                ;

            entity
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem())

                .AddSystem(new ApplyDamageSystem())

                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))

                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())
                .AddSystem(new AnotherTeamTouchDetectorSystem())

                .AddSystem(new SelfExplodeSystem(_container.Resolve<ExplosionsFactory>()))

                .AddSystem(new ShipSinkingSystem())
            ;

            return entity;
        }
        // vehicles


        // projectiles
        public Entity CreateSimpleProjectile(
            Transform parent, 
            ProjectileCreationContext ctx, 
            SimpleProjectileConfig config)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            // Vector3 shootDirection = parent.forward;
            Vector3 shootDirection = ctx.ShootDirection;

            entity
                .AddPushDirection(new ReactiveVariable<Vector3>(shootDirection))
                .AddPushForce(new ReactiveVariable<float>(ctx.LaunchPower))
                .AddGravityScale(new ReactiveVariable<float>(config.GravityScale))

                .AddIsDead()
                .AddContactsDetectingMask(LayersAPI.LayerMaskWater)
                .AddContactCollidersBuffer(new Buffer<Collider>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))
                .AddBodyContactDamage(new ReactiveVariable<float>(ctx.FinalDamage))
                .AddDeathMask(LayersAPI.LayerMaskWater) // ? | LayersAPI.LayerMaskDefault)
                .AddIsTouchDeathMask()
                .AddIsTouchAnotherTeam()
                .AddTeam(new ReactiveVariable<Teams>(ctx.Owner.Team.Value))
                ;

            ICompositeCondition mustDie = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true))
                .Add(new FuncCondition(() => entity.IsTouchAnotherTeam.Value == true));

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == true));

            entity
                .AddMustDie(mustDie)
                .AddMustExplode(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                  .AddSystem(new RigidbodyGravityApplySystem())
                  .AddSystem(new AddDelayedForceSystem(ctx.LaunchDelay, _container.Resolve<ICoroutinesPerformer>()))
                  .AddSystem(new SlowApearEntityViewSystem(ctx.LaunchDelay * 1.25f, _container.Resolve<ICoroutinesPerformer>()))
                  .AddSystem(new TransformRotateWithLinearVelocitySystem())

                  .AddSystem(new DeathSystem())
                  .AddSystem(new DisableCollidersOnDeathSystem())
                  .AddSystem(new SelfReleaseSystem(_entitiesLifeContext))
                  .AddSystem(new BodyContactDetectingSystem())
                  .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                  .AddSystem(new DealDamageOnContactSystem())
                  .AddSystem(new DeathMaskTouchDetectorSystem())
                  .AddSystem(new AnotherTeamTouchDetectorSystem())

                  .AddSystem(new SelfExplodeSystem(_container.Resolve<ExplosionsFactory>()))
                  ;

            return entity;
        }
        // projectiles


        public Entity CreateBallista(Transform parent, BallistaConfig config, Teams team)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono =_monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddTeam(new ReactiveVariable<Teams>(team));

            return entity;
        }

        private Entity CreateEmpty() => new Entity();
    }
}
