using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.Timers;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayPopupService _popupService;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private readonly GameplayScreenView _view;

        private LevelConfig _levelConfig;
        private GameplayInputArgs _inputArgs;

        private readonly List<IPresenter> _childPresenters = new();

        private InGameTimerPresenter _timerPresenter;

        public GameplayScreenPresenter(
            GameplayScreenView view,
            GameplayPresentersFactory gmeplayPresentersFactory,
            GameplayPopupService gameplayPopupService,
            LevelConfig levelConfig,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer,
            GameplayInputArgs inputArgs)
        {
            _view = view;
            _gameplayPresentersFactory = gmeplayPresentersFactory;
            _popupService = gameplayPopupService;
            _levelConfig = levelConfig;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _inputArgs = inputArgs;
        }

        public void Initialize()
        {
            _view.OpenGameSettingsButton.onClick.AddListener(OnOpenGameSettingsButtonClicked);
            _view.RestartButton.onClick.AddListener(OnRestartButtonClicked);

            _timerPresenter = _gameplayPresentersFactory.CreateTimerPresenter(_view.TimerView, _levelConfig.TargetTime);
            _childPresenters.Add(_timerPresenter);

            InGameWalletPresenter walletPresenter = _gameplayPresentersFactory.CreateInGameWalletPresenter(_view.InGameWalletView);
            _childPresenters.Add(walletPresenter);

            RankStylePresenter stylePresenter = _gameplayPresentersFactory.CreateStylePresenter(_view.StyleView);
            _childPresenters.Add(stylePresenter);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void StartGameplayHud()
        {
            _timerPresenter.ShowAndStart();
        }

        private void OnRestartButtonClicked()
        {
            _inputArgs.IsRestart = true;
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, _inputArgs));
        }

        private void OnOpenGameSettingsButtonClicked()
        {
            _popupService.OpenGameSettingsPopup();
        }

        public void Dispose()
        {
            _view.OpenGameSettingsButton.onClick.RemoveListener(OnOpenGameSettingsButtonClicked);
            _view.RestartButton.onClick.RemoveListener(OnRestartButtonClicked);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _childPresenters.Clear();
        }
    }
}