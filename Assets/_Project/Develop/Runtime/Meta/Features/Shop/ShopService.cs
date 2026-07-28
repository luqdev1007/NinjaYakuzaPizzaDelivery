using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Meta.Features.Upgrades;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;

namespace Assets._Project.Develop.Runtime.Meta.Features.Shop
{
    /// <summary>
    /// Покупка тир-апгрейда: проверка максимума -> списание -> инкремент тира ->
    /// сохранение.
    ///
    /// Сервис работает через IUpgradeConfig и НЕ знает, что именно продаёт:
    /// капасити сумки и бонус урона читают свои потребители (InventorySystem и
    /// ProjectileFactory) напрямую у своих конфигов. Поэтому добавление третьего
    /// товара сюда не приезжает.
    ///
    /// Сохранение — существующий явный SaveAsync, автосейва в проекте нет.
    /// Второго save-пути не заводим и не дублируем вызов, как это сделано в
    /// MainMenuScreenPresenter.ResetStats (там Reset() уже пишет сейв сам,
    /// и следом стартует ещё одна корутина записи).
    /// </summary>
    public class ShopService
    {
        private readonly WalletService _walletService;
        private readonly PlayerUpgradesService _playerUpgradesService;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public ShopService(
            WalletService walletService,
            PlayerUpgradesService playerUpgradesService,
            PlayerDataProvider playerDataProvider,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _walletService = walletService;
            _playerUpgradesService = playerUpgradesService;
            _playerDataProvider = playerDataProvider;
            _coroutinesPerformer = coroutinesPerformer;
        }

        /// <summary>
        /// Купить следующий тир. false = максимум уже куплен ИЛИ не хватает валюты.
        /// Обе ветки отказа не имеют побочных эффектов: валюта не списывается,
        /// тир не растёт, сейв не пишется.
        /// </summary>
        public bool TryPurchase(IUpgradeConfig config)
        {
            if (config == null)
                return false;

            int currentTier = _playerUpgradesService.GetTier(config.ItemId);

            if (config.TryGetCostForNextTier(currentTier, out int cost) == false)
                return false;

            if (_walletService.TrySpend(config.Currency, cost) == false)
                return false;

            _playerUpgradesService.IncrementTier(config.ItemId);

            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            return true;
        }

        /// <summary>
        /// Можно ли купить следующий тир прямо сейчас. Отдельно от TryPurchase,
        /// чтобы UI мог гасить кнопку, не дёргая покупку.
        /// </summary>
        public bool CanPurchase(IUpgradeConfig config)
        {
            if (config == null)
                return false;

            int currentTier = _playerUpgradesService.GetTier(config.ItemId);

            if (config.TryGetCostForNextTier(currentTier, out int cost) == false)
                return false;

            return _walletService.IsEnough(config.Currency, cost);
        }
    }
}
