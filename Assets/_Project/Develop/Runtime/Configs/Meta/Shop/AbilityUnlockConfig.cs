using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Бинарный товар: куплен или нет, тиров нет. Открывает способность.
    ///
    /// Отдельный тип, а не StatUpgradeConfig с одним тиром: у того есть
    /// TargetStat и StatBonus, к анлоку неприменимые. Анлок с TargetStat =
    /// каким-нибудь EvasionChance и StatBonus = 0 читался бы как «апгрейд
    /// уклонения, который ничего не даёт» — вранье в данных, которое рано или
    /// поздно кто-то применит буквально.
    ///
    /// Тиры базы при этом переиспользуются как есть: MaxTier = 1, куплен =
    /// GetTier(ItemId) == 1. Заводить рядом второй механизм «владения» ради
    /// бинарного случая не нужно — сейв уже умеет хранить единицу.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Ability Unlock Config",
        fileName = "AbilityUnlockConfig",
        order = 59)]
    public class AbilityUnlockConfig : ShopItemConfigBase
    {
        [SerializeField, Min(0)] private int _cost;

        [SerializeField, Tooltip("Что даёт покупка. Показывается, пока не куплено")]
        private string _effectText;

        public override int MaxTier => 1;

        public override bool TryGetCostForNextTier(int currentTier, out int cost)
        {
            cost = 0;

            if (currentTier < 0)
                return false;

            if (currentTier >= MaxTier)
                return false;

            cost = _cost;

            return true;
        }

        public override string GetTierEffectText(int currentTier)
        {
            if (currentTier < 0)
                return string.Empty;

            if (currentTier >= MaxTier)
                return string.Empty;

            return _effectText;
        }
    }
}
