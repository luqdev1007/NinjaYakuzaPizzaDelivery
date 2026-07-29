using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Ветка «охват»: время жизни снаряда (то есть пробег) и размер хитбокса.
    ///
    /// Дальность выражена ВРЕМЕНЕМ, а не дистанцией, потому что так устроен
    /// сам слэш: SelfReleaseSystem снимает его по LifeTime &lt;= 0, в отличие от
    /// метательных, которые считают Vector3.Distance до MaxFlyDistance.
    /// Прибавка в секундах, а не в юнитах — иначе цифра в конфиге не совпадала
    /// бы с тем, чем её меряет игра.
    ///
    /// HitboxScaleBonus — ДОЛЯ, прибавляемая к множителю: итоговый масштаб =
    /// ChargedSlashConfig.HitboxScale * (1 + bonus). Доля, а не абсолютный
    /// размер, потому что базовый размер живёт в префабе вместе с визуалом, и
    /// абсолют в конфиге стал бы вторым хозяином одной величины.
    ///
    /// Семантика тира АБСОЛЮТНАЯ, копия GetDamageBonusFor сюрикена.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Charged Slash Reach Upgrade Config",
        fileName = "ChargedSlashReachUpgradeConfig",
        order = 62)]
    public class ChargedSlashReachUpgradeConfig : ShopItemConfigBase
    {
        [SerializeField] private List<ChargedSlashReachTier> _tiers = new();

        public IReadOnlyList<ChargedSlashReachTier> Tiers => _tiers;

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

            ChargedSlashReachTier nextTier = _tiers[currentTier];

            int hitboxPercent = Mathf.RoundToInt(nextTier.HitboxScaleBonus * 100f);

            return $"+{nextTier.LifeTimeBonus}с полёта, +{hitboxPercent}% к размеру";
        }

        public float GetLifeTimeBonusFor(int tier)
        {
            if (tier <= 0)
                return 0f;

            if (_tiers.Count == 0)
                return 0f;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].LifeTimeBonus;
        }

        public float GetHitboxBonusFor(int tier)
        {
            if (tier <= 0)
                return 0f;

            if (_tiers.Count == 0)
                return 0f;

            int index = Mathf.Min(tier, _tiers.Count) - 1;

            return _tiers[index].HitboxScaleBonus;
        }
    }

    [Serializable]
    public class ChargedSlashReachTier
    {
        [field: SerializeField, Min(0)] public int Cost { get; private set; }

        [field: SerializeField, Min(0f), Tooltip("Прибавка к времени жизни снаряда, секунды")]
        public float LifeTimeBonus { get; private set; }

        [field: SerializeField, Min(0f), Tooltip("Доля к множителю размера: 0.5 = +50%")]
        public float HitboxScaleBonus { get; private set; }
    }
}
