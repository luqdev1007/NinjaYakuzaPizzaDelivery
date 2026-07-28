using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.DataProviders
{
    public class PlayerDataProvider : DataProvider<PlayerData>
    {
        private readonly ConfigsProviderService _configsProviderService;

        public PlayerDataProvider(
            ISaveLoadService saveLoadService,
            ConfigsProviderService configsProviderService,
            ICoroutinesPerformer coroutinesPerformer) : base(saveLoadService, coroutinesPerformer)
        {
            _configsProviderService = configsProviderService;
        }

        // Единственный писатель флага — IntroBootstrap на завершении/скипе интро.
        // Через IDataReader/IDataWriter не гоняем: за одним bool отдельный сервис
        // избыточен, а два писателя дали бы тихий рассинхрон.
        public bool IntroSeen
        {
            get => Data.IntroSeen;
            set => Data.IntroSeen = value;
        }

        protected override PlayerData GetOriginData()
        {
            return new PlayerData()
            {
                WalletData = InitWalletData(),
                Wins = 0,
                Losses = 0,
                CompletedLevels = new(),
                IntroSeen = false
            };
        }

        private Dictionary<CurrencyTypes, int> InitWalletData()
        {
            Dictionary<CurrencyTypes, int> walletData = new();

            StartWalletConfig walletConfig = _configsProviderService.GetConfig<StartWalletConfig>();

            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
                walletData[currencyType] = walletConfig.GetValueFor(currencyType);

            return walletData;
        }
    }
}


