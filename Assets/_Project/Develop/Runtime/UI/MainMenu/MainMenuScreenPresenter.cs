using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _view;
        private readonly MainMenuPopupService _mainMenupopupService;

        private readonly ConfigsProviderService _configsProviderService;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ProjectPresentersFactory _presentersFactory;

        private WalletPresenter _walletPresenter;
        private ICoroutinesPerformer _coroutinesPerformer;

        private List<IDisposable> _disposables = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView view,
            MainMenuPopupService mainMenuPopupService,
            ConfigsProviderService configsProviderService,
            PlayerDataProvider playerDataProvider,
            ProjectPresentersFactory presentersFactory,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _view = view;
            _mainMenupopupService = mainMenuPopupService;
            _configsProviderService = configsProviderService;
            _playerDataProvider = playerDataProvider;
            _presentersFactory = presentersFactory;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void Initialize()
        {
            Debug.Log("Init main menu screen presenter");
            _view.OpenOrdersButton.onClick.AddListener(OpenChooseOrderTiles);

            _view.ResetStatsButton.onClick.AddListener(OnResetStatsButtonClicked);
            _view.OpenGameSettingsButton.onClick.AddListener(OnOpenGameSettingsButtonClicked);

            _view.OpenDojoButton.onClick.AddListener(OnOpenDojoButtonClicked);

            _view.DojoView.ExitDojoButton.onClick.AddListener(OnExitDojoButtonClicked);

            _walletPresenter = _presentersFactory.CreateWalletPresenter(_view.WalletView);
            _disposables.Add(_walletPresenter);

            _walletPresenter.Initialize();
        }

        private void OnOpenDojoButtonClicked()
        {
            _view.DojoView.gameObject.SetActive(true);
        }

        private void OnExitDojoButtonClicked()
        {
            _view.DojoView.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _view.OpenOrdersButton.onClick.RemoveListener(OpenChooseOrderTiles);

            _view.ResetStatsButton.onClick.RemoveListener(OnResetStatsButtonClicked);
            _view.OpenGameSettingsButton.onClick.RemoveListener(OnOpenGameSettingsButtonClicked);

            _view.OpenDojoButton.onClick.RemoveListener(OnOpenDojoButtonClicked);

            _view.DojoView.ExitDojoButton.onClick.RemoveListener(OnExitDojoButtonClicked);

            foreach (var disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
        }

        private void OpenChooseOrderTiles()
        {
            Debug.Log("Choose order");
            _mainMenupopupService.OpenLevelsMenuPopup();
        }

        private void OnOpenGameSettingsButtonClicked()
        {
            _mainMenupopupService.OpenGameSettingsPopup();
        }

        private void OnResetStatsButtonClicked()
        {
            int baseGold = _configsProviderService.GetConfig<StartWalletConfig>().GetValueFor(CurrencyTypes.Coins);

            _mainMenupopupService.OpenConfirmPopup(ResetStats,
                $"Reset stats?\nYou will start a new game with {baseGold} gold");
        }

        private void ResetStats()
        {
            _playerDataProvider.Reset();
            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());
        }
    }
}