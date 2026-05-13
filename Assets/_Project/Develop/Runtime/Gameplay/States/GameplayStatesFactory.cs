using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class GameplayStatesFactory
    {
        private readonly DIContainer _container;
        private readonly GameplayInputArgs _inputArgs;

        public GameplayStatesFactory(DIContainer container, GameplayInputArgs inputArgs)
        {
            _container = container;
            _inputArgs = inputArgs;
        }

        public LevelIntroState CreateIntroState()
        {
            return new LevelIntroState(
                _container.Resolve<CameraService>(),
                _container.Resolve<DialogPresenter>(),
                _container.Resolve<GameplayUIRoot>()
            );
        }

        public LevelScoutingState CreateScoutingState()
        {
            return new LevelScoutingState(
                _container.Resolve<CameraService>(),
                _container.Resolve<IInputService>()
            );
        }

        public LevelProcessState CreateProcessState()
        {
            var configsProvider = _container.Resolve<ConfigsProviderService>();
            var levelConfig = configsProvider.GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            return new LevelProcessState(
                _container.Resolve<CameraService>(),
                _container.Resolve<MainHeroFactory>(),
                _container.Resolve<GameplayUIRoot>(),
                _container.Resolve<InGameTimerFeatureService>(),
                levelConfig.StartPlayerPosition
            );
        }

        public WinState CreateWinState()
        {
            return new WinState(
                _container.Resolve<IInputService>(),
                _container.Resolve<LevelsProgressionService>(),
                _inputArgs,
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<GameplayPopupService>(),
                _container.Resolve<WalletService>()
            );
        }

        public DefeatState CreateDefeatState()
        {
            return new DefeatState(
                _container.Resolve<IInputService>(),
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

            stateMachine.AddTransition(intro, scouting, new FuncCondition(() =>
            {
                return intro.IsFinished;
            }));

            stateMachine.AddTransition(scouting, process, new FuncCondition(() =>
            {
                return scouting.IsConfirmed;
            }));

            return stateMachine;
        }

        public GameplayStateMachine CreateGameplayStateMachine()
        {
            FinalPointTriggerService finalPointTrigger = _container.Resolve<FinalPointTriggerService>();
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
                return finalPointTrigger.HasMainHeroContact.Value == true;
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