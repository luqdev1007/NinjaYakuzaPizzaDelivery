using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;
        private readonly GameplayPopupService _popupService;

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView view,
            GameplayPresentersFactory gmeplayPresentersFactory,
            GameplayPopupService gameplayPopupService)
        {
            _view = view;
            _gameplayPresentersFactory = gmeplayPresentersFactory;
            _popupService = gameplayPopupService;
        }

        public void Initialize()
        {
            Debug.Log("GameplayScreenPresenter Initialized!");
            _view.OpenAudioSettingsButton.onClick.AddListener(OnOpenAudioSettingsButtonClicked);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        private void OnOpenAudioSettingsButtonClicked()
        {
            _popupService.OpenAudioSettingsPopup();
        }

        public void Dispose()
        {
            _view.OpenAudioSettingsButton.onClick.RemoveListener(OnOpenAudioSettingsButtonClicked);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _childPresenters.Clear();
        }
    } 
}
