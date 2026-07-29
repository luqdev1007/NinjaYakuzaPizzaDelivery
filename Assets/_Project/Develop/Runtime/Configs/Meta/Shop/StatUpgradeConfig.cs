using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Какой стат героя качает товар. ОТДЕЛЬНОЕ поле, а не переиспользованный
    /// ItemId: ItemId — это ключ в PlayerData.PurchasedTiers, то есть формат
    /// сейва. Повесив на него вторую роль «имя стата», мы бы связали сейв с
    /// раскладкой статов — переименование стата стоило бы игрокам покупок.
    ///
    /// Enum, а не строка (как TargetConsumableId у сумки): множество статов
    /// закрытое и известно на компиляции, а опечатка в строке дала бы молчаливый
    /// промах в бою вместо ошибки в редакторе.
    /// </summary>
    public enum StatUpgradeTarget
    {
        EvasionChance = 0,
        DoubleAttackChance = 1,
    }

    /// <summary>
    /// Процентный стат-апгрейд героя (уклонение, шанс двойного удара).
    ///
    /// Один тип на оба товара, в отличие от пары BagUpgradeConfig /
    /// ShurikenDamageUpgradeConfig. Там конфиги разведены потому, что у тиров
    /// РАЗНАЯ размерность (штуки против урона) и общий конфиг тащил бы два поля
    /// с одним всегда пустым. Здесь размерность одна — проценты 0..100, — так
    /// что разводить нечего, различается только цель.
    ///
    /// Семантика тира — АБСОЛЮТНАЯ, как у ShurikenDamageUpgradeConfig: в
    /// Tiers[i] лежит итоговая прибавка тира i+1, а не приращение к предыдущему.
    /// Копируем осознанно: два разных правила чтения тиров в одном магазине —
    /// гарантированный источник ошибок в балансе.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Stat Upgrade Config",
        fileName = "StatUpgradeConfig",
        order = 58)]
    public class StatUpgradeConfig : ShopItemConfigBase
    {
        [SerializeField, Tooltip("Какой стат героя качает этот товар")]
        private StatUpgradeTarget _targetStat;

        [SerializeField] private List<StatUpgradeTier> _tiers = new();

        public StatUpgradeTarget TargetStat => _targetStat;

        public IReadOnlyList<StatUpgradeTier> Tiers => _tiers;

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
        /// то есть тот, чей бонус вернёт GetStatBonusFor(currentTier + 1).
        /// </summary>
        public override string GetTierEffectText(int currentTier)
        {
            if (currentTier < 0)
                return string.Empty;

            if (currentTier >= _tiers.Count)
                return string.Empty;

            return $"+{_tiers[currentTier].StatBonus}% {GetStatDisplayName()}";
        }

        /// <summary>
        /// Абсолютная прибавка к стату на данном тире (не накопительная —
        /// в Tiers[i] лежит итоговый бонус тира i+1). tier = 0 — бонуса нет.
        /// Один в один GetDamageBonusFor у урона сюрикена, включая кламп индекса.
        /// </summary>
        public float GetStatBonusFor(int tier)
        {
            if (tier <= 0)
                return 0f;

            if (_tiers.Count == 0)
                return 0f;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].StatBonus;
        }

        private string GetStatDisplayName()
        {
            switch (_targetStat)
            {
                case StatUpgradeTarget.EvasionChance:
                    return "уклонения";

                case StatUpgradeTarget.DoubleAttackChance:
                    return "шанса двойного удара";

                default:
                    return string.Empty;
            }
        }
    }

    [Serializable]
    public class StatUpgradeTier
    {
        [field: SerializeField, Min(0)] public int Cost { get; private set; }

        [field: SerializeField, Range(0f, 100f)] public float StatBonus { get; private set; }
    }
}
