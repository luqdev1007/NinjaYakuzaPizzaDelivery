using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Ветка «использований за уровень»: сколько раз за забег можно выпустить
    /// заряженный слэш.
    ///
    /// Отдельный тип от Power/Reach, а не общий с безымянными BonusA/BonusB:
    /// у этой ветки одна нагрузка и она ЦЕЛАЯ (штуки), у соседних — две и
    /// дробные. Ровно этим в проекте обосновано разделение BagUpgradeConfig и
    /// ShurikenDamageUpgradeConfig: «общий конфиг заставил бы держать оба поля
    /// с одним всегда пустым».
    ///
    /// Семантика тира АБСОЛЮТНАЯ, копия GetDamageBonusFor сюрикена:
    /// в Tiers[i] лежит итоговая прибавка тира i+1, а не приращение.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Charged Slash Charges Upgrade Config",
        fileName = "ChargedSlashChargesUpgradeConfig",
        order = 60)]
    public class ChargedSlashChargesUpgradeConfig : ShopItemConfigBase
    {
        [SerializeField] private List<ChargedSlashChargesTier> _tiers = new();

        public IReadOnlyList<ChargedSlashChargesTier> Tiers => _tiers;

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
        /// Эффект СЛЕДУЮЩЕГО тира. Индексация та же, что у
        /// TryGetCostForNextTier: _tiers[currentTier] — и есть следующий тир.
        /// </summary>
        public override string GetTierEffectText(int currentTier)
        {
            if (currentTier < 0)
                return string.Empty;

            if (currentTier >= _tiers.Count)
                return string.Empty;

            return $"+{_tiers[currentTier].ChargesBonus} к использованиям";
        }

        /// <summary>
        /// Абсолютная прибавка к числу зарядов на данном тире. tier = 0 —
        /// прибавки нет.
        /// </summary>
        public int GetChargesBonusFor(int tier)
        {
            if (tier <= 0)
                return 0;

            if (_tiers.Count == 0)
                return 0;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].ChargesBonus;
        }
    }

    [Serializable]
    public class ChargedSlashChargesTier
    {
        [field: SerializeField, Min(0)] public int Cost { get; private set; }

        [field: SerializeField, Min(0)] public int ChargesBonus { get; private set; }
    }
}
