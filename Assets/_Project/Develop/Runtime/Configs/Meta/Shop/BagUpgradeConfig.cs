using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Сумка расходника: покупается ВМЕСТИМОСТЬ, а не количество. Старт уровня
    /// рефилит расходник до капасити (см. InventorySystem.OnInit), поэтому
    /// «сколько осталось» между забегами не хранится нигде и хранить не нужно.
    ///
    /// TargetConsumableId связывает апгрейд с конкретным InventoryItemConfig по
    /// его Id. Матчинг по Id, а не по типу конфига — чтобы не заводить четвёртый
    /// список метательных конфигов: их уже дублируют PlayerInventoryConfig,
    /// InventoryUIPresenter._consumables и MainHeroConfig.ThrowableSettings.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Bag Upgrade Config",
        fileName = "BagUpgradeConfig",
        order = 55)]
    public class BagUpgradeConfig : ScriptableObject, IUpgradeConfig
    {
        [field: SerializeField] public string ItemId { get; private set; } = "bag_capacity";

        [field: SerializeField] public CurrencyTypes Currency { get; private set; } = CurrencyTypes.Coins;

        [field: SerializeField, Tooltip("Id расходника (InventoryItemConfig.Id), чью вместимость качаем")]
        public string TargetConsumableId { get; private set; }

        [SerializeField] private List<BagUpgradeTier> _tiers = new();

        public IReadOnlyList<BagUpgradeTier> Tiers => _tiers;

        public int MaxTier => _tiers.Count;

        public bool TryGetCostForNextTier(int currentTier, out int cost)
        {
            cost = 0;

            if (currentTier < 0)
                return false;

            if (currentTier >= _tiers.Count)
                return false;

            cost = _tiers[currentTier].Cost;

            return true;
        }

        /// <summary>
        /// Вместимость на данном тире. tier = 0 (не куплен) — возвращается
        /// baseCapacity, то есть MaxCharges самого расходника: апгрейд не должен
        /// становиться обязательным условием того, что предмет вообще работает.
        /// </summary>
        public int GetCapacityFor(int tier, int baseCapacity)
        {
            if (tier <= 0)
                return baseCapacity;

            if (_tiers.Count == 0)
                return baseCapacity;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].Capacity;
        }
    }

    [Serializable]
    public class BagUpgradeTier
    {
        [field: SerializeField, Min(0)] public int Cost { get; private set; }

        [field: SerializeField, Min(0)] public int Capacity { get; private set; }
    }
}
