using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _view;
        private readonly MainMenuPopupService _popupService;
        private readonly WalletService _wallet;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ProjectPresentersFactory _presentersFactory;

        private WalletPresenter _walletPresenter;
        private List<IDisposable> _disposables = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView view,
            MainMenuPopupService popupService,
            WalletService wallet,
            ConfigsProviderService configsProviderService,
            PlayerDataProvider playerDataProvider,
            ProjectPresentersFactory presentersFactory)
        {
            _view = view;
            _popupService = popupService;
            _wallet = wallet;
            _configsProviderService = configsProviderService;
            _playerDataProvider = playerDataProvider;
            _presentersFactory = presentersFactory;
        }

        public void Initialize()
        {
            _view.StartGameButtonClicked += OnStartGameButtonClicked;
            _view.ResetStatsButtonClicked += OnResetStatsButtonClicked;

            _walletPresenter = _presentersFactory.CreateWalletPresenter(_view.WalletView);
            _walletPresenter.Initialize();
        }

        public void Dispose()
        {
            _view.StartGameButtonClicked -= OnStartGameButtonClicked;
            _view.ResetStatsButtonClicked -= OnResetStatsButtonClicked;

            _walletPresenter?.Dispose();

            foreach (var disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
        }

        private void OnStartGameButtonClicked()
        {
            _popupService.OpenLevelsMenuPopup();
        }

        private void OnResetStatsButtonClicked()
        {
            int baseGold = _configsProviderService.GetConfig<StartWalletConfig>().GetValueFor(CurrencyTypes.Gold);

            _popupService.OpenConfirmPopup(ResetStats,
                $"Вы потеряете все золото и начнете с нуля с {baseGold} золота в кармане");
        }

        private void ResetStats()
        {
            _playerDataProvider.Reset();
        }
    }
}