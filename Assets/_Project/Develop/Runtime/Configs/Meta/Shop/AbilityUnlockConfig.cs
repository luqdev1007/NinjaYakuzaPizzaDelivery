using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Какую способность открывает покупка.
    ///
    /// Enum, а НЕ сверка ItemId со строковой константой в коде фабрики:
    /// ItemId — ключ сейва, его правит тот, кто балансит магазин, и он не
    /// обязан знать, что где-то в MainHeroFactory лежит строка, которая должна
    /// с ним совпадать. Разъехавшись, они не дали бы ни ошибки, ни лога —
    /// способность просто перестала бы открываться. Ровно тот же аргумент, по
    /// которому StatUpgradeConfig несёт StatUpgradeTarget, а не имя стата в ItemId.
    /// </summary>
    public enum AbilityUnlockTarget
    {
        ChargedSlash = 0,
    }

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
        [SerializeField, Tooltip("Какую способность открывает покупка")]
        private AbilityUnlockTarget _targetAbility;

        [SerializeField, Min(0)] private int _cost;

        [SerializeField, Tooltip("Что даёт покупка. Показывается, пока не куплено")]
        private string _effectText;

        public AbilityUnlockTarget TargetAbility => _targetAbility;

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
