using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.Hints;
using Assets._Project.Develop.Runtime.UI.Gameplay.ResultPopups;
using Assets._Project.Develop.Runtime.UI.Gameplay.Stages;
using Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.Timers;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private readonly DIContainer _container;
        private readonly GameplayInputArgs _inputArgs;

        public GameplayPresentersFactory(DIContainer container, GameplayInputArgs inputArgs)
        {
            _container = container;
            _inputArgs = inputArgs;
        }

        public RankStylePresenter CreateStylePresenter(RankStyleView view)
        {
            return new RankStylePresenter(view, _container.Resolve<RankStyleService>());
        }

        public DialogPresenter CreateDialogPresenter(DialogDisplayView view, DialogConfig config)
        {
            return new DialogPresenter(
                view, 
                _container.Resolve<ICoroutinesPerformer>(), 
                config, 
                _container.Resolve<ConfigsProviderService>().GetConfig<CharactersConfig>(),
                _container.Resolve<GameplayUIRoot>()
                );
        }

        public HintPresenter CreateHintPresenter(HintView view, string message)
        {
            return new HintPresenter(view, _container.Resolve<ICoroutinesPerformer>(), message);
        }

        public LevelProgressPresenter CreateLevelProgressPresenter(BarWithText view)
        {
            return new LevelProgressPresenter(
                view,
                _container.Resolve<LevelProgressService>());
        }

        public EntitiesHealthDisplayPresenter CreateEntitiesHealthDisplayPresenter(EntitiesHealthDisplay view)
        {
            return new EntitiesHealthDisplayPresenter(
                _container.Resolve<EntitiesLifeContext>(),
                view,
                _container.Resolve<ViewsFactory>(),
                this
                );
        }

        public EntityHealthPresenter CreateEntityHealthPresenter(Entity entity, BarWithText view)
        {
            return new EntityHealthPresenter(view, entity);
        }

        public WinPopupPresenter CreateWinPopupPresenter(WinPopupView view)
        {
            return new WinPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                view,
                _container.Resolve<SceneSwitcherService>()
                );
        }

        public DefeatPopupPresenter CreateDefeatPopupPresenter(DefeatPopupView view)
        {
            return new DefeatPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                view,
                _container.Resolve<SceneSwitcherService>(),
                _inputArgs,
                _container.Resolve<WalletService>()
                );
        }

        public InGameTimerPresenter CreateTimerPresenter(InGameTimerView view, float targetTime)
        {
            TimerService timerService = _container.Resolve<TimerServiceFactory>().Create(targetTime);

            return new InGameTimerPresenter(view, timerService, _container.Resolve<InGameTimerFeatureService>(), targetTime);
        }

        public GameplayScreenPresenter CreateGameplayScreen(GameplayScreenView view)
        {
            LevelConfig levelConfig = _container.Resolve<ConfigsProviderService>().GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            return new GameplayScreenPresenter(
                view, 
                _container.Resolve<GameplayPresentersFactory>(),
                _container.Resolve<GameplayPopupService>(),
                levelConfig,
                _container.Resolve<SceneSwitcherService>(),
                _container.Resolve<ICoroutinesPerformer>(),
                _inputArgs,
                _container.Resolve<WalletService>()
                );
        }

        public StagePresenter CreateStagePresenter(IconTextView view)
        {
            return new StagePresenter(
                view,
                _container.Resolve<StageProviderService>()
                );
        }

        public GameplayWalletPresenter CteateGameplayWalletPresenter(WalletHUDView walletView)
        {
            return new GameplayWalletPresenter(_container.Resolve<WalletService>(), walletView);
        }
    }
}