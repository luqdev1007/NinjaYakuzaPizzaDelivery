using Assets._Project.Develop.Runtime.Meta.Features.Wallet;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Общий контракт тир-апгрейда для ShopService: чем платим, сколько стоит
    /// следующий тир и есть ли он вообще. Полезная нагрузка тира (капасити,
    /// бонус урона) контракту не видна — её читает конкретный потребитель у
    /// конкретного конфига, поэтому ShopService не знает, что именно продаёт.
    /// </summary>
    public interface IUpgradeConfig
    {
        string ItemId { get; }

        CurrencyTypes Currency { get; }

        int MaxTier { get; }

        /// <summary>
        /// Стоимость перехода с currentTier на currentTier + 1.
        /// false = следующего тира нет (куплен максимум).
        /// </summary>
        bool TryGetCostForNextTier(int currentTier, out int cost);
    }
}
