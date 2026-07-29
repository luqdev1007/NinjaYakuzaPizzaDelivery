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
    ///
    /// ItemId/Currency и витринные поля живут в ShopItemConfigBase — здесь
    /// остаётся только то, что специфично для сумки.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Bag Upgrade Config",
        fileName = "BagUpgradeConfig",
        order = 55)]
    public class BagUpgradeConfig : ShopItemConfigBase
    {
        [field: SerializeField, Tooltip("Id расходника (InventoryItemConfig.Id), чью вместимость качаем")]
        public string TargetConsumableId { get; private set; }

        [SerializeField] private List<BagUpgradeTier> _tiers = new();

        public IReadOnlyList<BagUpgradeTier> Tiers => _tiers;

        public override int MaxTier => _tiers.Count;

        public override bool TryGetCostForNextTier(int currentTier, out int cost)
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
        /// Что даст следующий тир. Индексация та же, что у TryGetCostForNextTier:
        /// _tiers[currentTier] — это и есть следующий тир (тир 1 лежит в [0]),
        /// то есть тот, чью капасити вернёт GetCapacityFor(currentTier + 1).
        /// </summary>
        public override string GetTierEffectText(int currentTier)
        {
            if (currentTier < 0)
                return string.Empty;

            if (currentTier >= _tiers.Count)
                return string.Empty;

            return $"Вместимость → {_tiers[currentTier].Capacity}";
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
