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

                .AddBaseMoveSpeed(new ReactiveVariable<float>(config.Movement.MoveSpeed))
                .AddMoveSpeedModifiers(new StatModifiersList())
                .AddBaseLootCollectRange(new ReactiveVariable<float>(config.LootCollectRange))
                .AddLootCollectRangeModifiers(new StatModifiersList())

                // Уклонение. Итоговый EvasionChance заводится НУЛЁМ намеренно:
                // единственный его писатель — EvasionChanceStatSynchronizerSystem,
                // и он перезапишет значение своим Recalculate в OnInit. Дублировать
                // сюда config-значение (как это сделано выше у LootCollectRange)
                // не стали — двойное авторство одного числа только путает.
                //
                // EvadedEvent ОБЯЗАН стоять рядом со статом и добавляться вместе с
                // ним. Без него бросок продолжает отменять урон (за это отвечает
                // EvasionChance), но визуал молча не срабатывает: и
                // ApplyDamageSystem, и AfterimageView читают событие через TryGet.
                // Именно так этот баг и появился — стат был, события не было, ни
                // одной ошибки при этом не печаталось. Обе стороны теперь ругаются
                // в лог на такую сборку, но чинится она здесь, одной строкой.
                .AddBaseEvasionChance(new ReactiveVariable<float>(config.LifeCycle.EvasionChance))
                .AddEvasionChanceModifiers(new StatModifiersList())
                .AddEvasionChance(new ReactiveVariable<float>(0f))
                .AddEvadedEvent()

                .AddActiveBuffs(new ActiveBuffsList())
                ;

            _brainsFactory.CreateMainHeroBrain(entity);

            entity
                .AddSystem(new MainHeroStyleSystem(_container.Resolve<StyleEvaluator>()))
                .AddSystem(new LootMagnetSystem(_entitiesLifeContext))
                .AddSystem(new LootDistanceCollectSystem(_entitiesLifeContext, _container.Resolve<SessionLootService>()))
                .AddSystem(new TargetingCoreSystem(_entitiesLifeContext))

                .AddSystem(new MoveSpeedStatSynchronizerSystem())
                .AddSystem(new LootCollectRangeStatSynchronizerSystem())
                .AddSystem(new EvasionChanceStatSynchronizerSystem())
                .AddSystem(new BuffsTimerSystem())
                .AddSystem(new BuffMagnetSystem(_entitiesLifeContext))
                .AddSystem(new BuffDistanceCollectSystem(_entitiesLifeContext, _buffService))
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
