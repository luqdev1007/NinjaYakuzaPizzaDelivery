using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Урон сюрикена. Бонус ПЛЮСУЕТСЯ к ShurikenConfig.Damage в момент спавна
    /// снаряда (ProjectileFactory.CreateThrowableProjectile) — сам конфиг снаряда
    /// не мутируется, иначе бонус накапливался бы между забегами внутри
    /// ScriptableObject'а, который в редакторе живёт дольше сессии.
    ///
    /// Отдельный конфиг от BagUpgradeConfig, а не общий с полем-«полезной
    /// нагрузкой»: у тиров разная размерность (штуки против урона), и общий
    /// конфиг заставил бы держать оба поля с одним всегда пустым.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Shuriken Damage Upgrade Config",
        fileName = "ShurikenDamageUpgradeConfig",
        order = 56)]
    public class ShurikenDamageUpgradeConfig : ScriptableObject, IUpgradeConfig
    {
        [field: SerializeField] public string ItemId { get; private set; } = "shuriken_damage";

        [field: SerializeField] public CurrencyTypes Currency { get; private set; } = CurrencyTypes.Coins;

        [SerializeField] private List<DamageUpgradeTier> _tiers = new();

        public IReadOnlyList<DamageUpgradeTier> Tiers => _tiers;

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
        /// Абсолютный бонус к урону на данном тире (не накопительный —
        /// в Tiers[i] лежит итоговая прибавка тира i+1). tier = 0 — бонуса нет.
        /// </summary>
        public float GetDamageBonusFor(int tier)
        {
            if (tier <= 0)
                return 0f;

            if (_tiers.Count == 0)
                return 0f;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].DamageBonus;
        }
    }

    [Serializable]
    public class DamageUpgradeTier
    {
        [field: SerializeField, Min(0)] public int Cost { get; private set; }

        [field: SerializeField, Min(0f)] public float DamageBonus { get; private set; }
    }
}
