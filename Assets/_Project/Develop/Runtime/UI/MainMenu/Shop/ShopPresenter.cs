using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Upgrades;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    /// <summary>
    /// Витрина магазина: конфиг-driven создание карточек по образцу
    /// LevelsMenuPopupPresenter, вкладки валют и общая перекраска состояний.
    ///
    /// Вкладка переключает ВИДИМОСТЬ карточек (gameObject.SetActive), а не
    /// состав ShopItemsListView: список — общий ElementsListView, и Remove из
    /// него сорвал бы карточку с _parent, после чего вернуть её на место было
    /// бы нечем. Погашенная карточка вдобавок не ловит клик (GameObject
    /// неактивен), поэтому подписка сразу на все карточки безопасна.
    ///
    /// ДЕРЕВО ПОКУПОК. Товар с непустым RequiredItemId не получает карточку,
    /// пока родитель не куплен: такие конфиги лежат в _pendingConfigs и
    /// превращаются в карточки в тот момент, когда родителя купили. Механизм
    /// НЕ ЗНАЕТ ни про заряженный слэш, ни про какой-либо конкретный товар —
    /// он оперирует только RequiredItemId, поэтому следующее дерево прокачки
    /// стоит ровно новых ассетов и нуля строк здесь.
    /// </summary>
    public class ShopPresenter : ISubscribePresenter
    {
        private readonly ShopView _view;
        private readonly ShopCatalogConfig _catalogConfig;
        private readonly CurrencyIconsConfig _currencyIconsConfig;
        private readonly WalletService _walletService;
        private readonly PlayerUpgradesService _playerUpgradesService;
        private readonly ProjectPresentersFactory _presentersFactory;
        private readonly ViewsFactory _viewsFactory;

        private readonly List<ShopItemPresenter> _itemPresenters = new();

        /// <summary>
        /// Товары, ждущие покупки родителя. Порядок сохраняет порядок каталога.
        /// </summary>
        private readonly List<ShopItemConfigBase> _pendingConfigs = new();

        private CurrencyTypes _activeCurrency;

        /// <summary>
        /// Активны ли сейчас подписки. Нужен, чтобы карточка, созданная В
        /// СЕРЕДИНЕ жизни презентера, подписалась сразу: общий Subscribe() к
        /// этому моменту уже прошёл и второй раз её не подхватит — без этого
        /// свежая ветка просто не кликалась бы.
        /// </summary>
        private bool _isSubscribed;

        public ShopPresenter(
            ShopView view,
            ShopCatalogConfig catalogConfig,
            CurrencyIconsConfig currencyIconsConfig,
            WalletService walletService,
            PlayerUpgradesService playerUpgradesService,
            ProjectPresentersFactory presentersFactory,
            ViewsFactory viewsFactory)
        {
            _view = view;
            _catalogConfig = catalogConfig;
            _currencyIconsConfig = currencyIconsConfig;
            _walletService = walletService;
            _playerUpgradesService = playerUpgradesService;
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
        }

        public void Initialize()
        {
            ValidateRequiredItemIds();

            for (int i = 0; i < _catalogConfig.Items.Count; i++)
            {
                ShopItemConfigBase itemConfig = _catalogConfig.Items[i];

                if (itemConfig == null)
                {
                    continue;
                }

                if (IsParentPurchased(itemConfig) == false)
                {
                    _pendingConfigs.Add(itemConfig);

                    continue;
                }

                CreateItemCard(itemConfig);
            }

            // Иконки слотов расставляются ОДИН раз: набор валют за время жизни
            // экрана не меняется, а переключение вкладки двигает только кнопку.
            _view.CurrencySwitchView.SetSlotIcons(BuildSlotIcons());

            SetActiveCurrency(GetFirstAvailableCurrency(), animated: false);
        }

        public void Dispose()
        {
            _view.SwitchCurrencyButton.onClick.RemoveListener(OnSwitchCurrencyClicked);
            _view.BackButton.onClick.RemoveListener(OnBackButtonClicked);

            _walletService.OnCurrencyChanged -= OnBalanceChanged;

            // Виджет — узел префаба, он переживёт презентера, поэтому недобитый
            // твин остался бы крутить уже ничей RectTransform.
            _view.CurrencySwitchView.Cleanup();

            // Цикл идёт по _itemPresenters, а туда попадают и карточки, созданные
            // динамически после покупки родителя, — разбираются все одинаково.
            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                itemPresenter.Purchased -= OnItemPurchased;

                _view.ItemsListView.Remove(itemPresenter.View);
                _viewsFactory.Release(itemPresenter.View);
                itemPresenter.Dispose();
            }

            _itemPresenters.Clear();
            _pendingConfigs.Clear();

            _isSubscribed = false;
        }

        public void Subscribe()
        {
            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                itemPresenter.Subscribe();
            }

            _view.SwitchCurrencyButton.onClick.AddListener(OnSwitchCurrencyClicked);
            _view.BackButton.onClick.AddListener(OnBackButtonClicked);

            _walletService.OnCurrencyChanged += OnBalanceChanged;

            _isSubscribed = true;
        }

        public void Unsubscribe()
        {
            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                itemPresenter.Unsubscribe();
            }

            _view.SwitchCurrencyButton.onClick.RemoveListener(OnSwitchCurrencyClicked);
            _view.BackButton.onClick.RemoveListener(OnBackButtonClicked);

            _walletService.OnCurrencyChanged -= OnBalanceChanged;

            _isSubscribed = false;
        }

        /// <summary>
        /// Создать карточку одного товара. Публичный путь, а не только шаг
        /// Initialize: ветки дерева появляются в середине жизни презентера, и
        /// им нужен весь тот же набор действий — инстанс, лист, презентер,
        /// подписка, фильтр вкладки.
        /// </summary>
        public void CreateItemCard(ShopItemConfigBase itemConfig)
        {
            if (itemConfig == null)
            {
                return;
            }

            ShopItemView itemView = _viewsFactory.Create<ShopItemView>(ViewIDs.ShopItemView);

            _view.ItemsListView.Add(itemView);

            ShopItemPresenter itemPresenter = _presentersFactory
                .CreateShopItemPresenter(itemView, itemConfig);

            itemPresenter.Initialize();
            itemPresenter.Purchased += OnItemPurchased;

            _itemPresenters.Add(itemPresenter);

            if (_isSubscribed)
            {
                itemPresenter.Subscribe();
            }

            // Свежая карточка обязана сразу подчиниться активной вкладке, иначе
            // ветка за осколки выскочит поверх вкладки за золото.
            itemView.gameObject.SetActive(itemConfig.Currency == _activeCurrency);
        }

        /// <summary>
        /// Показать вкладку валюты: кнопка виджета едет к слоту активной валюты,
        /// карточки чужой валюты гаснут, свои пересобирают состояние под текущий
        /// баланс.
        /// </summary>
        public void SetActiveCurrency(CurrencyTypes currency)
        {
            SetActiveCurrency(currency, animated: true);
        }

        /// <summary>
        /// Перекрасить все карточки. Нужно не только своей карточке после
        /// покупки, но и чужим: упавший баланс мог сделать соседний товар
        /// недоступным.
        /// </summary>
        public void RefreshAllStates()
        {
            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                itemPresenter.RefreshState();
            }
        }

        private void SetActiveCurrency(CurrencyTypes currency, bool animated)
        {
            _activeCurrency = currency;

            _view.CurrencySwitchView.SetLabel(currency.ToString());
            _view.CurrencySwitchView.SlideTo(GetSlotIndexFor(currency), animated);

            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                itemPresenter.View.gameObject.SetActive(itemPresenter.Config.Currency == currency);
            }

            RefreshAllStates();
        }

        /// <summary>
        /// Пришло ПОСЛЕ IncrementTier, поэтому тир актуален и ветки можно
        /// раскрывать прямо здесь. Через OnCurrencyChanged этого сделать было
        /// нельзя — оно поднимается из середины TrySpend, до инкремента.
        /// </summary>
        private void OnItemPurchased(string purchasedItemId)
        {
            RevealPendingItemsFor(purchasedItemId);

            RefreshAllStates();
        }

        /// <summary>
        /// Достать из отложенных всех детей купленного товара. Обход с конца —
        /// список правится по ходу.
        /// </summary>
        private void RevealPendingItemsFor(string purchasedItemId)
        {
            for (int i = _pendingConfigs.Count - 1; i >= 0; i--)
            {
                ShopItemConfigBase pendingConfig = _pendingConfigs[i];

                if (pendingConfig.RequiredItemId != purchasedItemId)
                {
                    continue;
                }

                if (IsParentPurchased(pendingConfig) == false)
                {
                    continue;
                }

                _pendingConfigs.RemoveAt(i);

                CreateItemCard(pendingConfig);
            }
        }

        private bool IsParentPurchased(ShopItemConfigBase itemConfig)
        {
            if (string.IsNullOrEmpty(itemConfig.RequiredItemId))
            {
                return true;
            }

            return _playerUpgradesService.GetTier(itemConfig.RequiredItemId) > 0;
        }

        private void OnSwitchCurrencyClicked()
        {
            List<CurrencyTypes> availableCurrencies = _walletService.AvailableCurrencies;

            if (availableCurrencies.Count == 0)
            {
                return;
            }

            int currentIndex = availableCurrencies.IndexOf(_activeCurrency);
            int nextIndex = (currentIndex + 1) % availableCurrencies.Count;

            SetActiveCurrency(availableCurrencies[nextIndex]);
        }

        private void OnBackButtonClicked()
        {
            _view.gameObject.SetActive(false);
        }

        private void OnBalanceChanged(CurrencyTypes currency, int delta, int total)
        {
            RefreshAllStates();
        }

        private List<Sprite> BuildSlotIcons()
        {
            List<Sprite> icons = new();

            foreach (CurrencyTypes currency in _walletService.AvailableCurrencies)
            {
                icons.Add(_currencyIconsConfig.GetSpriteFor(currency));
            }

            return icons;
        }

        /// <summary>
        /// Слот валюты = её позиция в AvailableCurrencies. Виджет двухслотовый,
        /// поэтому валюта, не влезшая в разметку, честно садится на последний
        /// слот вместо тихого промаха мимо несуществующего.
        /// </summary>
        private int GetSlotIndexFor(CurrencyTypes currency)
        {
            int index = _walletService.AvailableCurrencies.IndexOf(currency);

            if (index < 0)
            {
                return 0;
            }

            int lastSlotIndex = _view.CurrencySwitchView.SlotsCount - 1;

            if (index > lastSlotIndex)
            {
                return lastSlotIndex;
            }

            return index;
        }

        private CurrencyTypes GetFirstAvailableCurrency()
        {
            List<CurrencyTypes> availableCurrencies = _walletService.AvailableCurrencies;

            if (availableCurrencies.Count == 0)
            {
                return CurrencyTypes.Coins;
            }

            return availableCurrencies[0];
        }

        /// <summary>
        /// RequiredItemId — строковый указатель, и промахивается он МОЛЧА:
        /// опечатка означала бы товар, который никогда не появится в магазине,
        /// без единого сообщения. Ровно та же мина, что у
        /// BagUpgradeConfig.TargetConsumableId, и лечится она так же — проверкой
        /// на старте.
        ///
        /// Только под UNITY_EDITOR: это диагностика для того, кто правит
        /// конфиги, а не рантайм-логика.
        /// </summary>
        private void ValidateRequiredItemIds()
        {
#if UNITY_EDITOR
            HashSet<string> knownItemIds = new();

            foreach (ShopItemConfigBase itemConfig in _catalogConfig.Items)
            {
                if (itemConfig == null)
                {
                    continue;
                }

                knownItemIds.Add(itemConfig.ItemId);
            }

            foreach (ShopItemConfigBase itemConfig in _catalogConfig.Items)
            {
                if (itemConfig == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(itemConfig.RequiredItemId))
                {
                    continue;
                }

                if (itemConfig.RequiredItemId == itemConfig.ItemId)
                {
                    Debug.LogError(
                        $"[Shop] Товар '{itemConfig.ItemId}' требует сам себя — карточка не появится никогда.");

                    continue;
                }

                if (knownItemIds.Contains(itemConfig.RequiredItemId) == false)
                {
                    Debug.LogError(
                        $"[Shop] RequiredItemId='{itemConfig.RequiredItemId}' не найден в каталоге " +
                        $"у товара '{itemConfig.ItemId}' — карточка не появится никогда.");
                }
            }
#endif
        }
    }
}
