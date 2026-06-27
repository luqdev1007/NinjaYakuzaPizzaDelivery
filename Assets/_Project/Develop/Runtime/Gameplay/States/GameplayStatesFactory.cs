using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Context;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.States;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class GameplayStatesFactory
    {
        private readonly DIContainer _container;
        private readonly GameplayInputArgs _inputArgs;
        private readonly GameplaySceneContext _sceneContext;

        public GameplayStatesFactory(DIContainer container, GameplayInputArgs inputArgs, GameplaySceneContext sceneContext)
        {
            _container = container;
            _inputArgs = inputArgs;
            _sceneContext = sceneContext;
        }

        public LevelIntroState CreateIntroState()
        {
            var configsProvider = _container.Resolve<ConfigsProviderService>();
            var levelConfig = configsProvider.GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            return new LevelIntroState(
                _container.Resolve<CameraService>(),
                _container.Resolve<GameplayPopupService>(),
                _container.Resolve<GameplayUIRoot>(),
                levelConfig.StartLevelDialogConfig,
                _inputArgs.IsRestart
            );
        }

        public LevelScoutingState CreateScoutingState()
        {
            return new LevelScoutingState(
                _container.Resolve<CameraService>(),
                _container.Resolve<IInputService>(),
                _container.Resolve<GameplayPopupService>(),
                _inputArgs.IsRestart
            );
        }

        public LevelProcessState CreateProcessState()
        {
            if (_sceneContext.StartPoint == null)
                throw new NullReferenceException("GameplaySceneContext.StartPoint not assigned in Level Prefab");

            return new LevelProcessState(
                _container.Resolve<CameraService>(),
                _container.Resolve<MainHeroFactory>(),
                _container.Resolve<GameplayUIRoot>(),
                _container.Resolve<GameplayScreenPresenter>(),
                _container.Resolve<InGameTimerFeatureService>(),
                _container.Resolve<StageProviderService>(),
                _container.Resolve<RankStyleService>(),
                _container.Resolve<StyleEvaluator>(),
                _container.Resolve<IInputService>(),
                _sceneContext.StartPoint.position
            );
        }

        public WinState CreateWinState()
        {
            return new WinState(
                _container.Resolve<IInputService>(),
                _container.Resolve<GameplayScreenPresenter>(),
                _container.Resolve<LevelsProgressionService>(),
                _inputArgs,
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<GameplayPopupService>(),
                _container.Resolve<SessionLootService>(),
                _container.Resolve<WalletService>()
            );
        }

        public DefeatState CreateDefeatState()
        {
            return new DefeatState(
                _container.Resolve<IInputService>(),
                _container.Resolve<GameplayScreenPresenter>(),
                _container.Resolve<GameplayPopupService>()
            );
        }

        public GameplayStateMachine CreateCoreLoop()
        {
            LevelIntroState intro = CreateIntroState();
            LevelScoutingState scouting = CreateScoutingState();
            LevelProcessState process = CreateProcessState();

            GameplayStateMachine stateMachine = new GameplayStateMachine();

            stateMachine.AddState(intro);
            stateMachine.AddState(scouting);
            stateMachine.AddState(process);

            stateMachine.AddTransition(intro, scouting, new FuncCondition(() => intro.IsFinished));

            stateMachine.AddTransition(scouting, process, new FuncCondition(() => scouting.IsConfirmed));

            return stateMachine;
        }

        public GameplayStateMachine CreateGameplayStateMachine()
        {
            StageProviderService stageProviderService = _container.Resolve<StageProviderService>();
            MainHeroHolderService mainHeroHolderService = _container.Resolve<MainHeroHolderService>();

            GameplayStateMachine coreLoop = CreateCoreLoop();
            WinState winState = CreateWinState();
            DefeatState defeatState = CreateDefeatState();

            GameplayStateMachine gameplayCycle = new GameplayStateMachine();

            gameplayCycle.AddState(coreLoop);
            gameplayCycle.AddState(winState);
            gameplayCycle.AddState(defeatState);

            gameplayCycle.AddTransition(coreLoop, winState, new FuncCondition(() =>
            {
                return stageProviderService.CurrentStageResult.Value == StageResults.Completed;
            }));

            gameplayCycle.AddTransition(coreLoop, defeatState, new FuncCondition(() =>
            {
                if (mainHeroHolderService.MainHero != null)
                {
                    return mainHeroHolderService.MainHero.IsDead.Value == true &&
                           mainHeroHolderService.MainHero.InDeathProcess.Value == false;
                }

                return false;
            }));

            return gameplayCycle;
        }
    }
}