using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.Timers;
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

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView view,
            GameplayPresentersFactory gmeplayPresentersFactory,
            GameplayPopupService gameplayPopupService,
            LevelConfig levelConfig)
        {
            _view = view;
            _gameplayPresentersFactory = gmeplayPresentersFactory;
            _popupService = gameplayPopupService;
            _levelConfig = levelConfig;
        }

        public void Initialize()
        {
            Debug.Log("GameplayScreenPresenter Initialized!");
            _view.OpenGameSettingsButton.onClick.AddListener(OnOpenGameSettingsButtonClicked);

            InGameTimerPresenter timerPresenter = _gameplayPresentersFactory.CreateTimerPresenter(_view.TimerView, _levelConfig.TargetTime);
            _childPresenters.Add(timerPresenter);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        private void OnOpenGameSettingsButtonClicked()
        {
            _popupService.OpenGameSettingsPopup();
        }

        public void Dispose()
        {
            _view.OpenGameSettingsButton.onClick.RemoveListener(OnOpenGameSettingsButtonClicked);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _childPresenters.Clear();
        }
    } 
}
