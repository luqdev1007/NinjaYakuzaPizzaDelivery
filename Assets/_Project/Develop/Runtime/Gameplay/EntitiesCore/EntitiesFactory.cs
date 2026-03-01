using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

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

        // examples
        /*
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
        */

        /*
        public Entity CreateBallista(Transform parent, BallistaConfig config, Teams team)
        {
            Entity entity = CreateEmpty();

            MonoEntity mono =_monoEntitiesFactory.Create(entity, parent, config.PrefabPath);

            entity
                .AddTeam(new ReactiveVariable<Teams>(team));

            return entity;
        }
        */

        private Entity CreateEmpty() => new Entity();
    }
}
