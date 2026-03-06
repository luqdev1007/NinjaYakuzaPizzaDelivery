using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Stats;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
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
        private readonly GameStatsService _statsService;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly PlayerDataProvider _playerDataProvider;

        private List<IDisposable> _disposables = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView view,
            MainMenuPopupService popupService,
            WalletService wallet,
            GameStatsService statsService,
            ConfigsProviderService configsProviderService,
            PlayerDataProvider playerDataProvider)
        {
            _view = view;
            _popupService = popupService;
            _wallet = wallet;
            _statsService = statsService;
            _configsProviderService = configsProviderService;
            _playerDataProvider = playerDataProvider;
        }

        public void Initialize()
        {
            _view.SetGoldText(_wallet.GetCurrency(CurrencyTypes.Gold).Value.ToString());
            _view.SetLosesText(_statsService.Losses.Value.ToString());
            _view.SetWinsText(_statsService.Wins.Value.ToString());

            _disposables.Add(_wallet.GetCurrency(CurrencyTypes.Gold).Subscribe(OnGoldChanged));
            _disposables.Add(_statsService.Wins.Subscribe(OnWinsChanged));
            _disposables.Add(_statsService.Losses.Subscribe(OnLossesChanged));

            _view.StartGameButtonClicked += OnStartGameButtonClicked;
            _view.ResetStatsButtonClicked += OnResetStatsButtonClicked;
        }

        public void Dispose()
        {
            _view.StartGameButtonClicked -= OnStartGameButtonClicked;
            _view.ResetStatsButtonClicked -= OnResetStatsButtonClicked;

            foreach (var disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
        }

        private void OnLossesChanged(int oldValue, int newValue)
        {
            _view.SetLosesText(newValue.ToString());
        }

        private void OnWinsChanged(int oldValue, int newValue)
        {
            _view.SetWinsText(newValue.ToString());
        }

        private void OnStartGameButtonClicked()
        {
            _popupService.OpenLevelsMenuPopup();
        }

        private void OnResetStatsButtonClicked()
        {
            int baseGold = _configsProviderService.GetConfig<StartWalletConfig>().GetValueFor(CurrencyTypes.Gold);

            _popupService.OpenConfirmPopup(ResetStats, $"Вы потеряете все золото и начнете с нуля с {baseGold} золота в кармане");
        }

        private void ResetStats()
        {
            _playerDataProvider.Reset();
        }

        private void OnGoldChanged(int arg1, int newValue)
        {
            _view.SetGoldText(newValue.ToString());
        }
    }
}
