using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs; 
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature; 
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly BuffService _buffService; 

        public MainHeroFactory(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _buffService = _container.Resolve<BuffService>(); 
        }

        public Entity Create(Vector3 at)
        {
            MainHeroConfig config = _configsProviderService.GetConfig<MainHeroConfig>();

            Entity entity = _entitiesFactory.CreateHero(at, config);

            entity
                .AddIsMainHero()
                .AddTeam(new ReactiveVariable<Teams>(Teams.MainHero))

                .AddIntentSwitchTarget()
                .AddIsTargetingActive()
                .AddCurrentTarget()

                .AddLootCollectRange(new ReactiveVariable<float>(config.LootCollectRange))
                .AddLootPickedEvent()

                // === БАФФЫ: модификатор-слой для MoveSpeed/LootCollectRange ===
                .AddBaseMoveSpeed(new ReactiveVariable<float>(config.Movement.MoveSpeed))
                .AddMoveSpeedModifiers(new StatModifiersList())
                .AddBaseLootCollectRange(new ReactiveVariable<float>(config.LootCollectRange))
                .AddLootCollectRangeModifiers(new StatModifiersList())
                .AddActiveBuffs(new ActiveBuffsList())
                // === КОНЕЦ БЛОКА БАФФОВ ===
                ;

            _brainsFactory.CreateMainHeroBrain(entity);

            entity
                .AddSystem(new MainHeroStyleSystem(_container.Resolve<StyleEvaluator>()))
                .AddSystem(new LootMagnetSystem(_entitiesLifeContext))
                .AddSystem(new LootDistanceCollectSystem(_entitiesLifeContext, _container.Resolve<SessionLootService>()))
                .AddSystem(new TargetingCoreSystem(_entitiesLifeContext))

                // === БАФФЫ: синхронизаторы статов + таймер + pickup-системы баффов ===
                .AddSystem(new MoveSpeedStatSynchronizerSystem())
                .AddSystem(new LootCollectRangeStatSynchronizerSystem())
                .AddSystem(new BuffsTimerSystem())
                .AddSystem(new BuffMagnetSystem(_entitiesLifeContext))
                .AddSystem(new BuffDistanceCollectSystem(_entitiesLifeContext, _buffService))
                // === КОНЕЦ БЛОКА БАФФОВ ===
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}