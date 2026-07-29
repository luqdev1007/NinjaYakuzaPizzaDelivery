using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Meta.Features.Shop;
using Assets._Project.Develop.Runtime.Meta.Features.Upgrades;
using Assets._Project.Develop.Runtime.UI.Core;
using System;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    /// <summary>
    /// Одна карточка: рисует товар и дёргает ShopService по клику.
    ///
    /// Читает тир напрямую у PlayerUpgradesService, а не хранит копию:
    /// единственный писатель тиров — сам сервис, и локальный кэш здесь стал бы
    /// вторым источником правды, который разъедется после Reset Stats.
    /// </summary>
    public class ShopItemPresenter : ISubscribePresenter
    {
        private const string LockedTierInfo = "Закрыто";

        private readonly ShopItemView _view;
        private readonly ShopItemConfigBase _config;
        private readonly ShopService _shopService;
        private readonly PlayerUpgradesService _playerUpgradesService;
        private readonly CurrencyIconsConfig _currencyIconsConfig;

        /// <summary>
        /// Покупка СОСТОЯЛАСЬ. Аргумент — ItemId купленного товара.
        ///
        /// Отдельный канал от WalletService.OnCurrencyChanged, и это не
        /// дублирование. OnCurrencyChanged поднимается ВНУТРИ TrySpend, то есть
        /// ДО ShopService.IncrementTier: подписчик такого события видит ещё
        /// старый тир. Для перекраски цены это терпимо, а для дерева покупок —
        /// нет: ветки проверяют «куплен ли родитель» и остались бы закрытыми до
        /// следующего случайного обновления.
        ///
        /// Это событие инвокается ПОСЛЕ TryPurchase, когда тир уже поднят.
        /// </summary>
        public event Action<string> Purchased;

        public ShopItemPresenter(
            ShopItemView view,
            ShopItemConfigBase config,
            ShopService shopService,
            PlayerUpgradesService playerUpgradesService,
            CurrencyIconsConfig currencyIconsConfig)
        {
            _view = view;
            _config = config;
            _shopService = shopService;
            _playerUpgradesService = playerUpgradesService;
            _currencyIconsConfig = currencyIconsConfig;
        }

        public ShopItemView View => _view;

        public ShopItemConfigBase Config => _config;

        public void Initialize()
        {
            _view.Init(_config.ItemName, _config.Description, _config.Icon);
            _view.SetCurrencyIcon(_currencyIconsConfig.GetSpriteFor(_config.Currency));

            RefreshState();
        }

        public void Dispose()
        {
            _view.Clicked -= OnViewClicked;
        }

        public void Subscribe()
        {
            _view.Clicked += OnViewClicked;
        }

        public void Unsubscribe()
        {
            _view.Clicked -= OnViewClicked;
        }

        /// <summary>
        /// Пересобрать цену, тир и состояние кнопки по текущему профилю и
        /// кошельку. Зовётся и своим кликом, и общей перекраской из ShopPresenter.
        /// </summary>
        public void RefreshState()
        {
            if (IsLockedByParent())
            {
                _view.SetTierInfo(LockedTierInfo);
                _view.SetLocked();

                return;
            }

            int currentTier = _playerUpgradesService.GetTier(_config.ItemId);

            if (_config.TryGetCostForNextTier(currentTier, out int cost) == false)
            {
                _view.SetTierInfo($"Тир {_config.MaxTier}/{_config.MaxTier} · МАКС");
                _view.SetMaxed();

                return;
            }

            _view.SetTierInfo($"Тир {currentTier}/{_config.MaxTier} · {_config.GetTierEffectText(currentTier)}");
            _view.SetCost(cost);

            if (_shopService.CanPurchase(_config))
            {
                _view.SetAvailable();
            }
            else
            {
                _view.SetBlock();
            }
        }

        /// <summary>
        /// Родитель объявлен, но не куплен. При динамическом дереве карточка в
        /// таком состоянии обычно не создаётся вовсе — проверка страхует от
        /// рассинхрона и делает карточку самодостаточной, а не зависящей от
        /// того, что кто-то снаружи не забыл её не создать.
        /// </summary>
        private bool IsLockedByParent()
        {
            if (string.IsNullOrEmpty(_config.RequiredItemId))
            {
                return false;
            }

            return _playerUpgradesService.GetTier(_config.RequiredItemId) == 0;
        }

        /// <summary>
        /// RefreshState после покупки обязателен, хотя ShopPresenter и так
        /// перекрашивает всё по OnCurrencyChanged: списание валюты происходит
        /// ВНУТРИ TryPurchase, до IncrementTier, поэтому общая перекраска
        /// успевает прочитать ещё старый тир. Этот вызов приходит последним и
        /// показывает уже новый.
        ///
        /// Purchased инвокается ПОСЛЕ RefreshState — к моменту, когда
        /// ShopPresenter начнёт раскрывать ветки, карточка-родитель уже
        /// перерисована.
        /// </summary>
        private void OnViewClicked()
        {
            if (_shopService.TryPurchase(_config) == false)
            {
                return;
            }

            RefreshState();

            Purchased?.Invoke(_config.ItemId);
        }
    }
}
