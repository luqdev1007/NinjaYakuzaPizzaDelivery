using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using System;

public class StagesFactory
{
    private readonly DIContainer _container;
    private readonly LevelConfig _levelConfig; // добавь поле

    public StagesFactory(DIContainer container, LevelConfig levelConfig)
    {
        _container = container;
        _levelConfig = levelConfig;
    }

    public IStage Create(StageConfig stageConfig)
    {
        switch (stageConfig)
        {
            case ClearAllEnemiesStageConfig clearAllEnemiesStageConfig:
                return new ClearAllEnemiesStage(
                    clearAllEnemiesStageConfig,
                    _container.Resolve<EnemiesFactory>(),
                    _container.Resolve<EntitiesLifeContext>());

            case FinalPointReachedStageConfig:
                return new FinalPointReachedStage(
                    _container.Resolve<FinalPointTriggerService>(),
                    _container.Resolve<LevelProgressService>(),
                    _container.Resolve<MainHeroHolderService>(),
                    _levelConfig.FinalPointPosition); // берём отсюда

            default:
                throw new ArgumentException(
                    $"Not supported {stageConfig.GetType()} type config");
        }
    }
}