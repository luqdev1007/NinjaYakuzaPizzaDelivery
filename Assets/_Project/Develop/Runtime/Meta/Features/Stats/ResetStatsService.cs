using Assets._Project.Develop.Runtime.Configs.Meta.Stats;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Stats;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;

public class ResetStatsService
{
    private readonly WalletService _walletService;
    private readonly GameStatsService _gameStatsService;
    private readonly LevelsProgressionService _levelsProgressionService;
    private readonly PlayerDataProvider _playerDataProvider;

    public ResetStatsService(
        WalletService walletService,
        GameStatsService gameStatsService,
        ConfigsProviderService configsProviderService,
        LevelsProgressionService levelsProgressionService,
        PlayerDataProvider playerDataProvider)
    {
        _walletService = walletService;
        _gameStatsService = gameStatsService;
        _levelsProgressionService = levelsProgressionService;
        _playerDataProvider = playerDataProvider;
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
            _gameStatsService.ResetStats();
            _levelsProgressionService.ResetProgress(); // добавь метод
            _walletService.Spend(ResetCurrency, ResetCost);
            _playerDataProvider.SaveAsync();
            return true;
        }
        return false;
    }
}