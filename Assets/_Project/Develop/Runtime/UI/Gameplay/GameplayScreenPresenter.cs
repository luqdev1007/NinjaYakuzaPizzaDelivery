using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.Timers;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;
        private readonly GameplayPopupService _popupService;

        private LevelConfig _levelConfig;
        private GameplayInputArgs _inputArgs;

        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private readonly List<IPresenter> _childPresenters = new();

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
            Debug.Log("GameplayScreenPresenter Initialized!");

            _view.OpenGameSettingsButton.onClick.AddListener(OnOpenGameSettingsButtonClicked);
            _view.RestartButton.onClick.AddListener(OnRestartButtonClicked);

            InGameTimerPresenter timerPresenter = _gameplayPresentersFactory.CreateTimerPresenter(_view.TimerView, _levelConfig.TargetTime);
            _childPresenters.Add(timerPresenter);

            GameplayWalletPresenter walletPresenter = _gameplayPresentersFactory.CteateGameplayWalletPresenter(_view.WalletView);
            _childPresenters.Add(walletPresenter);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        private void OnRestartButtonClicked()
        {
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
