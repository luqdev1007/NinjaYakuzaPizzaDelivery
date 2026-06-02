using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;

namespace Assets._Project.Develop.Runtime.UI.Wallet
{
    public class InGameWalletPresenter : IPresenter
    {
        private readonly SessionLootService _sessionLootService;
        private readonly WalletService _walletService;
        private readonly InGameWalletView _view;

        private readonly Dictionary<CurrencyTypes, int> _startBalances = new();

        public InGameWalletPresenter(
            SessionLootService sessionLootService,
            WalletService walletService,
            InGameWalletView view)
        {
            _sessionLootService = sessionLootService;
            _walletService = walletService;
            _view = view;
        }

        public void Initialize()
        {
            foreach (LootTypes type in Enum.GetValues(typeof(LootTypes)))
            {
                if (TryMapToCurrency(type, out CurrencyTypes currencyType))
                {
                    _startBalances[currencyType] = _walletService.GetCurrency(currencyType).Value;
                }
            }

            foreach (LootTypes type in Enum.GetValues(typeof(LootTypes)))
            {
                UpdateView(type, 0, _sessionLootService.GetCollectedAmount(type));
            }

            _sessionLootService.OnLootUpdated += UpdateView;
        }

        public void Dispose()
        {
            _sessionLootService.OnLootUpdated -= UpdateView;
        }

        private void UpdateView(LootTypes type, int addedAmount, int totalSessionValue)
        {
            if (TryMapToCurrency(type, out CurrencyTypes currencyType))
            {
                int startBalance = _startBalances.TryGetValue(currencyType, out int baseVal) ? baseVal : 0;
                int finalDisplayValue = startBalance + totalSessionValue;

                _view.UpdateCurrency(currencyType, finalDisplayValue);

                if (addedAmount > 0)
                {
                    _view.PlayCollectEffect(currencyType);
                }
            }
        }

        private bool TryMapToCurrency(LootTypes lootType, out CurrencyTypes currencyType)
        {
            switch (lootType)
            {
                case LootTypes.Coin:
                    currencyType = CurrencyTypes.Coins;
                    return true;

                case LootTypes.SoulShard:
                    currencyType = CurrencyTypes.SoulShard;
                    return true;

                default:
                    currencyType = default;
                    return false;
            }
        }
    }
}