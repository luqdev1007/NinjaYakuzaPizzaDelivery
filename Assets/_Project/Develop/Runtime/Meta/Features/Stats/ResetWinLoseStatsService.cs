using Assets._Project.Develop.Runtime.Configs.Meta.Stats;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Meta.Features.Stats
{
    public class ResetWinLoseStatsService
    {
        private readonly WalletService _walletService;
        private readonly GameStatsService _gameStatsService;

        public ResetWinLoseStatsService(
            WalletService walletService,
            GameStatsService gameStatsService,
            ConfigsProviderService configsProviderService)
        {
            _walletService = walletService;
            _gameStatsService = gameStatsService;

            ResetCost = configsProviderService.GetConfig<GameRewardsConfig>().ResetCost;
            ResetCurrency = CurrencyTypes.Gold;
        }

        public int ResetCost { get; private set; }
        public CurrencyTypes ResetCurrency { get; private set; }
        public bool CanReset => _walletService.IsEnough(ResetCurrency, ResetCost);

        public bool TryResetData()
        {
            if (CanReset)
            {
                Debug.Log("stats reseted");
                _gameStatsService.ResetStats();
                
                _walletService.Spend(ResetCurrency, ResetCost);

                return true;
            }

            Debug.Log($"not enough {ResetCurrency} for reset data");
            return false;
        }
    }
}