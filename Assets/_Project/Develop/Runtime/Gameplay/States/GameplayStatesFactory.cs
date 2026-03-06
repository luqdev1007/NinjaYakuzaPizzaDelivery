using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class GameplayStatesFactory
    {
        private readonly DIContainer _container;
        private GameplayInputArgs _inputArgs;

        public GameplayStatesFactory(DIContainer container, GameplayInputArgs inputArgs)
        {
            _container = container;
            _inputArgs = inputArgs;
        }

        public PreperationState CreatePreperationState()
        {
            return new PreperationState(
                _inputArgs,
                _container.Resolve<StartGameTriggerService>());
        }

        public LaunchState CreateLaunchState(float timer)
        {
            return new LaunchState(timer);
        }

        public StageProcessState CreateStageProcessState()
        {
            return new StageProcessState(_container.Resolve<StageProviderService>(), _container.Resolve<LevelProgressService>());
        }

        public WinState CreateWinState(GameplayInputArgs inputArgs)
        {
            return new WinState(
                _container.Resolve<IInputService>(),
                _container.Resolve<LevelsProgressionService>(),
                inputArgs,
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<GameplayPopupService>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<ConfigsProviderService>()
                );     
        }

        public DefeatState CreateDefeatState()
        {
            return new DefeatState(
                _container.Resolve<IInputService>(),
                _container.Resolve<GameplayPopupService>());
        }

        public GameplayStateMachine CreateGameplayStateMachine(GameplayInputArgs inputArgs)
        {
            FinalPointTriggerService finalPointTrigger =
                _container.Resolve<FinalPointTriggerService>();
            MainHeroHolderService mainHeroHolderService =
                _container.Resolve<MainHeroHolderService>();

            GameplayStateMachine coreLoopState = CreateCoreLoopState();
            DefeatState defeatState = CreateDefeatState();
            WinState winState = CreateWinState(inputArgs);

            ICondition toWin = new FuncCondition(() =>
                finalPointTrigger.HasMainHeroContact.Value == true);

            ICondition toDefeat = new FuncCondition(() =>
            {
                if (mainHeroHolderService.MainHero != null)
                    return mainHeroHolderService.MainHero.IsDead.Value;
                return false;
            });

            GameplayStateMachine gameplayCycle = new GameplayStateMachine();

            gameplayCycle.AddState(coreLoopState);
            gameplayCycle.AddState(winState);
            gameplayCycle.AddState(defeatState);

            gameplayCycle.AddTransition(coreLoopState, winState, toWin);
            gameplayCycle.AddTransition(coreLoopState, defeatState, toDefeat);

            return gameplayCycle;
        }

        public GameplayStateMachine CreateCoreLoopState()
        {
            StartGameTriggerService startTrigger =
                _container.Resolve<StartGameTriggerService>();

            PreperationState preperationState = CreatePreperationState();
            LaunchState launchState = CreateLaunchState(timer: 2);
            StageProcessState stageProcessState = CreateStageProcessState();

            FuncCondition prepToLaunch =
                new FuncCondition(() => startTrigger.IsStartRequested);

            FuncCondition launchToProcess =
                new FuncCondition(() => launchState.IsFinished);

            GameplayStateMachine coreLoopState = new GameplayStateMachine();

            coreLoopState.AddState(preperationState);
            coreLoopState.AddState(launchState);
            coreLoopState.AddState(stageProcessState);

            coreLoopState.AddTransition(preperationState, launchState, prepToLaunch);
            coreLoopState.AddTransition(launchState, stageProcessState, launchToProcess);

            return coreLoopState;
        }

    }
}
