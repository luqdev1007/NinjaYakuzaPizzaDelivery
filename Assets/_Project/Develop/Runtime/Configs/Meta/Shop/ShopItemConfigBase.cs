using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Общая часть товара магазина: идентификация (ItemId), валюта и витринные
    /// данные (имя, описание, иконка). Всё, что различается от товара к товару —
    /// таблица тиров и её полезная нагрузка — остаётся на наследниках.
    ///
    /// Базовый КЛАСС, а не только интерфейс IUpgradeConfig: витрине нужен
    /// сериализуемый ScriptableObject, который можно положить в один список
    /// ShopCatalogConfig._items. Интерфейс в поле [SerializeField] Unity не
    /// сериализует, а держать параллельно список объектов и список конфигов
    /// значило бы завести второй источник правды о составе магазина.
    ///
    /// ItemId/Currency переехали сюда из наследников БЕЗ переименования полей:
    /// в .asset они лежат как &lt;ItemId&gt;k__BackingField и
    /// &lt;Currency&gt;k__BackingField, а Unity сериализует базовые поля тем же
    /// плоским списком — значения уже заполненных ассетов сохраняются.
    /// </summary>
    public abstract class ShopItemConfigBase : ScriptableObject, IUpgradeConfig
    {
        [field: SerializeField] public string ItemId { get; private set; }

        [field: SerializeField] public CurrencyTypes Currency { get; private set; }

        [field: SerializeField, Tooltip("Название товара в карточке")]
        public string ItemName { get; private set; }

        [field: SerializeField, TextArea, Tooltip("Описание товара в карточке")]
        public string Description { get; private set; }

        [field: SerializeField, Tooltip("Иконка товара. Может быть null — карточка это переживёт")]
        public Sprite Icon { get; private set; }

        public abstract int MaxTier { get; }

        public abstract bool TryGetCostForNextTier(int currentTier, out int cost);

        public abstract string GetTierEffectText(int currentTier);
    }
}
