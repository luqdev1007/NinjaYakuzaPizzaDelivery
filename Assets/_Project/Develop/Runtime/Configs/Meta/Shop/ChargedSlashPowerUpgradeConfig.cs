using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Ветка «мощь»: урон и скорость полёта заряженного слэша.
    ///
    /// Две нагрузки на тир — это нормально, потому что обе всегда заполнены:
    /// ветка по определению качает оба числа сразу. Упрёк «поле всегда пустое»,
    /// которым в проекте обоснованы раздельные конфиги, здесь не применим.
    ///
    /// ПОБОЧНЫЙ ЭФФЕКТ, ПРИНЯТ СОЗНАТЕЛЬНО: пробег слэша = Speed * LifeTime,
    /// поэтому рост скорости удлиняет и пробег, пересекаясь с веткой Reach.
    /// Развязывать (переводя слэш на ограничение по дистанции, как MaxFlyDistance
    /// у метательных) не стали — это поменяло бы ощущение способности.
    ///
    /// Семантика тира АБСОЛЮТНАЯ, копия GetDamageBonusFor сюрикена.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Charged Slash Power Upgrade Config",
        fileName = "ChargedSlashPowerUpgradeConfig",
        order = 61)]
    public class ChargedSlashPowerUpgradeConfig : ShopItemConfigBase
    {
        [SerializeField] private List<ChargedSlashPowerTier> _tiers = new();

        public IReadOnlyList<ChargedSlashPowerTier> Tiers => _tiers;

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

        public override string GetTierEffectText(int currentTier)
        {
            if (currentTier < 0)
                return string.Empty;

            if (currentTier >= _tiers.Count)
                return string.Empty;

            ChargedSlashPowerTier nextTier = _tiers[currentTier];

            return $"+{nextTier.DamageBonus} урона, +{nextTier.SpeedBonus} скорости";
        }

        public float GetDamageBonusFor(int tier)
        {
            if (tier <= 0)
                return 0f;

            if (_tiers.Count == 0)
                return 0f;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].DamageBonus;
        }

        public float GetSpeedBonusFor(int tier)
        {
            if (tier <= 0)
                return 0f;

            if (_tiers.Count == 0)
                return 0f;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].SpeedBonus;
        }
    }

    [Serializable]
    public class ChargedSlashPowerTier
    {
        [field: SerializeField, Min(0)] public int Cost { get; private set; }

        [field: SerializeField, Min(0f)] public float DamageBonus { get; private set; }

        [field: SerializeField, Min(0f)] public float SpeedBonus { get; private set; }
    }
}
