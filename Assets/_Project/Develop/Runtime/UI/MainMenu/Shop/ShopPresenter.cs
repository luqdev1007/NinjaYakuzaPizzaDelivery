using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;

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
    /// </summary>
    public class ShopPresenter : ISubscribePresenter
    {
        private readonly ShopView _view;
        private readonly ShopCatalogConfig _catalogConfig;
        private readonly CurrencyIconsConfig _currencyIconsConfig;
        private readonly WalletService _walletService;
        private readonly ProjectPresentersFactory _presentersFactory;
        private readonly ViewsFactory _viewsFactory;

        private readonly List<ShopItemPresenter> _itemPresenters = new();

        private CurrencyTypes _activeCurrency;

        public ShopPresenter(
            ShopView view,
            ShopCatalogConfig catalogConfig,
            CurrencyIconsConfig currencyIconsConfig,
            WalletService walletService,
            ProjectPresentersFactory presentersFactory,
            ViewsFactory viewsFactory)
        {
            _view = view;
            _catalogConfig = catalogConfig;
            _currencyIconsConfig = currencyIconsConfig;
            _walletService = walletService;
            _presentersFactory = presentersFactory;
            _viewsFactory = viewsFactory;
        }

        public void Initialize()
        {
            for (int i = 0; i < _catalogConfig.Items.Count; i++)
            {
                ShopItemConfigBase itemConfig = _catalogConfig.Items[i];

                if (itemConfig == null)
                {
                    continue;
                }

                ShopItemView itemView = _viewsFactory.Create<ShopItemView>(ViewIDs.ShopItemView);

                _view.ItemsListView.Add(itemView);

                ShopItemPresenter itemPresenter = _presentersFactory
                    .CreateShopItemPresenter(itemView, itemConfig);

                itemPresenter.Initialize();

                _itemPresenters.Add(itemPresenter);
            }

            SetActiveCurrency(GetFirstAvailableCurrency());
        }

        public void Dispose()
        {
            _view.SwitchCurrencyButton.onClick.RemoveListener(OnSwitchCurrencyClicked);
            _view.BackButton.onClick.RemoveListener(OnBackButtonClicked);

            _walletService.OnCurrencyChanged -= OnBalanceChanged;

            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                _view.ItemsListView.Remove(itemPresenter.View);
                _viewsFactory.Release(itemPresenter.View);
                itemPresenter.Dispose();
            }

            _itemPresenters.Clear();
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
        }

        /// <summary>
        /// Показать вкладку валюты: карточки чужой валюты гаснут, свои
        /// пересобирают состояние под текущий баланс.
        /// </summary>
        public void SetActiveCurrency(CurrencyTypes currency)
        {
            _activeCurrency = currency;

            _view.SetCurrencyIcon(_currencyIconsConfig.GetSpriteFor(currency));
            _view.SetCurrencyLabel(currency.ToString());

            foreach (ShopItemPresenter itemPresenter in _itemPresenters)
            {
                itemPresenter.View.gameObject.SetActive(itemPresenter.Config.Currency == currency);
            }

            RefreshAllStates();
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

        private CurrencyTypes GetFirstAvailableCurrency()
        {
            List<CurrencyTypes> availableCurrencies = _walletService.AvailableCurrencies;

            if (availableCurrencies.Count == 0)
            {
                return CurrencyTypes.Coins;
            }

            return availableCurrencies[0];
        }
    }
}
