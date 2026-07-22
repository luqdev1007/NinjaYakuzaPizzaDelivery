using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Patrol;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    public class EnemiesFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public EnemiesFactory(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        // patrolRoute — НЕОБЯЗАТЕЛЬНЫЙ параметр, и это принципиально: у
        // ClearAllEnemiesStage на руках только позиция и конфиг, маркера в сцене
        // нет вовсе. Он вызывает Create в две руки, как и раньше, и молча попадает
        // в ветку «маршрут не задан» — слайм получит запасной отрезок вокруг
        // точки спавна. Маршрут со сцены снимает GameplayBootstrap.
        //
        // Врагам, которым маршрут не нужен (обе разновидности призрака), параметр
        // просто игнорируется.
        public Entity Create(Vector3 at, EntityConfig config, PatrolRoute? patrolRoute = null, LanternAimData? lanternAim = null)
        {
            Entity entity;

            switch (config)
            {
                // ПОРЯДОК ВЕТОК КРИТИЧЕН И НЕ СЛУЧАЕН.
                // AngryGhostConfig наследует GhostConfig, поэтому pattern matching
                // по GhostConfig поймает и его тоже. Опусти эту ветку ниже — злой
                // призрак молча уедет в ветку обычного: получит CreateGhost вместо
                // CreateAngryGhost и CreateGhostBrain вместо CreateAngryGhostBrain,
                // то есть станет неотличим от рядового Ghost.
                // Ошибки компиляции при этом НЕ БУДЕТ — только тихо неправильное
                // поведение. Ветка AngryGhostConfig обязана оставаться выше.
                case AngryGhostConfig angryGhostConfig:
                    entity = _entitiesFactory.CreateAngryGhost(at, angryGhostConfig);

                    entity.AddLootIsDropped(new ReactiveVariable<bool>(false));

                    ICompositeCondition canAngryGhostDropLoot = new CompositeCondition()
                        .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

                    entity.AddCanDropLoot(canAngryGhostDropLoot);

                    entity.AddSystem(new DropLootSystem(
                        _container.Resolve<DropLootService>(),
                        angryGhostConfig.LootTable));

                    _brainsFactory.CreateAngryGhostBrain(entity, angryGhostConfig);
                    break;

                case GhostConfig ghostConfig:
                    entity = _entitiesFactory.CreateGhost(at, ghostConfig);

                    entity.AddLootIsDropped(new ReactiveVariable<bool>(false));

                    ICompositeCondition canDropLoot = new CompositeCondition()
                        .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

                    entity.AddCanDropLoot(canDropLoot);

                    LootTableConfig lootTable = ghostConfig.LootTable;

                    entity.AddSystem(new DropLootSystem(
                        _container.Resolve<DropLootService>(),
                        lootTable));

                    // Тайминги фаз блуждания теперь живут в конфиге, а не в хардкоде фабрики
                    _brainsFactory.CreateGhostBrain(entity, ghostConfig);
                    break;

                // SlimeConfig наследует EntityConfig НАПРЯМУЮ, поэтому в ловушку
                // порядка веток, описанную выше, он не попадает: pattern matching
                // по GhostConfig его не ловит, и эта ветка может стоять где угодно
                // ниже. Держим её последней перед default, чтобы порядок призраков
                // между собой не поехал при будущих правках.
                case SlimeConfig slimeConfig:
                    entity = _entitiesFactory.CreateSlime(at, slimeConfig, patrolRoute);

                    entity.AddLootIsDropped(new ReactiveVariable<bool>(false));

                    ICompositeCondition canSlimeDropLoot = new CompositeCondition()
                        .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

                    entity.AddCanDropLoot(canSlimeDropLoot);

                    entity.AddSystem(new DropLootSystem(
                        _container.Resolve<DropLootService>(),
                        slimeConfig.LootTable));

                    _brainsFactory.CreateSlimeBrain(entity);
                    break;

                // LanternConfig наследует EntityConfig НАПРЯМУЮ (как SlimeConfig),
                // поэтому в ловушку порядка веток pattern matching не попадает.
                // Мозга нет — фонарь стационарен, поведением рулит LanternFireSystem
                // на самой сущности. Прицел (lanternAim) приезжает из GameplayBootstrap.
                case LanternConfig lanternConfig:
                    entity = _entitiesFactory.CreateLantern(at, lanternConfig, lanternAim);

                    entity.AddLootIsDropped(new ReactiveVariable<bool>(false));

                    ICompositeCondition canLanternDropLoot = new CompositeCondition()
                        .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

                    entity.AddCanDropLoot(canLanternDropLoot);

                    entity.AddSystem(new DropLootSystem(
                        _container.Resolve<DropLootService>(),
                        lanternConfig.LootTable));
                    break;

                default:
                    throw new ArgumentException($"Not support {config.GetType()} type config");
            }

            entity.AddTeam(new ReactiveVariable<Teams>(Teams.Enemies)); 

            _entitiesLifeContext.Add(entity);

            return entity;
        }
    }
}
