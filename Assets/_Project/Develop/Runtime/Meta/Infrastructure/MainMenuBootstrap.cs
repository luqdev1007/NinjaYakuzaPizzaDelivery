using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Meta.Features.Shop;
using Assets._Project.Develop.Runtime.Meta.Features.Upgrades;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
#endif

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;

#if UNITY_EDITOR
        // Дебаг-инструменты сцены целиком под UNITY_EDITOR — в билд не попадает
        // ни одна строка. Сюда же уехали _coroutinesPerformer и
        // _playerDataProvider: их единственный потребитель — ручной сейв по F2,
        // и без гарда они остались бы в билде присвоенными, но никем не читаемыми.
        private const int DebugGoldGrant = 1000;

        private ICoroutinesPerformer _coroutinesPerformer;
        private PlayerDataProvider _playerDataProvider;

        private ShopService _shopService;
        private PlayerUpgradesService _playerUpgradesService;
        private WalletService _walletService;

        private BagUpgradeConfig _bagUpgradeConfig;
        private ShurikenDamageUpgradeConfig _shurikenDamageUpgradeConfig;
        private PlayerInventoryConfig _playerInventoryConfig;
#endif

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
            MainMenuContextRegistrations.Process(_container);
        }

        public override IEnumerator Initialize()
        {
#if UNITY_EDITOR
            _playerDataProvider = _container.Resolve<PlayerDataProvider>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            InitializeShopDebug();
#endif

            yield break;
        }

        public override void Run()
        {
            IAudioService audioService = _container.Resolve<IAudioService>();
            audioService.PlayPlaylist("MainMenu_Playlist");
        }

#if UNITY_EDITOR
        // Гардим саму сигнатуру Update, а не только тело: пустой Update Unity всё
        // равно вызывает каждый кадр через native->managed мост, и в билде этот
        // вызов был бы чистой платой ни за что.
        private void Update()
        {
            // Ручной сейв по F2 — дебажный инструмент. В билде игроку он не нужен,
            // а лишний способ дёрнуть запись сейва с клавиатуры только вредит.
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());
            }

            HandleShopDebugInput();
        }

        private void InitializeShopDebug()
        {
            _shopService = _container.Resolve<ShopService>();
            _playerUpgradesService = _container.Resolve<PlayerUpgradesService>();
            _walletService = _container.Resolve<WalletService>();

            ConfigsProviderService configsProvider = _container.Resolve<ConfigsProviderService>();

            _bagUpgradeConfig = configsProvider.GetConfig<BagUpgradeConfig>();
            _shurikenDamageUpgradeConfig = configsProvider.GetConfig<ShurikenDamageUpgradeConfig>();
            _playerInventoryConfig = configsProvider.GetConfig<PlayerInventoryConfig>();

            Debug.Log(
                "<color=#7FDBFF>[ShopDebug]</color> F5 — купить сумку, F6 — купить урон сюрикена, " +
                $"F7 — состояние, F8 — выдать {DebugGoldGrant} золота. " +
                "Сброс покупок — существующая кнопка Reset Stats в меню.");

            LogShopState("старт сцены");
        }

        private void HandleShopDebugInput()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                DebugPurchase(_bagUpgradeConfig, "СУМКА");
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                DebugPurchase(_shurikenDamageUpgradeConfig, "УРОН");
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                LogShopState("ручной запрос");
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                _walletService.Add(CurrencyTypes.Coins, DebugGoldGrant);

                Debug.Log($"<color=#7FDBFF>[ShopDebug]</color> Выдано {DebugGoldGrant} золота. " +
                    $"Баланс: {_walletService.GetCurrency(CurrencyTypes.Coins).Value}");
            }
        }

        private void DebugPurchase(IUpgradeConfig config, string label)
        {
            if (config == null)
            {
                Debug.LogError($"<color=#7FDBFF>[ShopDebug]</color> {label}: конфиг не загружен");
                return;
            }

            int goldBefore = _walletService.GetCurrency(config.Currency).Value;
            int tierBefore = _playerUpgradesService.GetTier(config.ItemId);

            bool hasNextTier = config.TryGetCostForNextTier(tierBefore, out int cost);

            bool purchased = _shopService.TryPurchase(config);

            int goldAfter = _walletService.GetCurrency(config.Currency).Value;
            int tierAfter = _playerUpgradesService.GetTier(config.ItemId);

            string costText = hasNextTier ? cost.ToString() : "—";

            string reason = string.Empty;

            if (purchased == false)
            {
                if (hasNextTier == false)
                {
                    reason = " | причина: МАКС-ТИР достигнут";
                }
                else
                {
                    reason = " | причина: не хватает валюты";
                }
            }

            Debug.Log(
                $"<color=#7FDBFF>[ShopDebug]</color> {label} TryPurchase={purchased}{reason}\n" +
                $"  тир: {tierBefore} -> {tierAfter} (макс {config.MaxTier}), цена следующего: {costText}\n" +
                $"  золото: {goldBefore} -> {goldAfter}");

            LogShopState($"после покупки {label}");
        }

        private void LogShopState(string reason)
        {
            int bagTier = _playerUpgradesService.GetTier(_bagUpgradeConfig.ItemId);
            int damageTier = _playerUpgradesService.GetTier(_shurikenDamageUpgradeConfig.ItemId);

            float damageBonus = _shurikenDamageUpgradeConfig.GetDamageBonusFor(damageTier);

            Debug.Log(
                $"<color=#7FDBFF>[ShopDebug]</color> Состояние ({reason}):\n" +
                $"  золото: {_walletService.GetCurrency(CurrencyTypes.Coins).Value}\n" +
                $"  сумка тир {bagTier}/{_bagUpgradeConfig.MaxTier} -> капасити сюрикена: {GetShurikenCapacityText()}\n" +
                $"  урон тир {damageTier}/{_shurikenDamageUpgradeConfig.MaxTier} -> бонус урона: +{damageBonus}");
        }

        /// <summary>
        /// Повторяет расчёт InventorySystem.GetCapacityFor, чтобы стенд показывал
        /// ровно то число, с которым слот зарефилится на старте уровня. Заодно
        /// проверяет, что связка BagUpgradeConfig.TargetConsumableId -> Id
        /// расходника вообще резолвится: не резолвится — увидим это здесь, а не
        /// молча получим базовый капасити в бою.
        /// </summary>
        private string GetShurikenCapacityText()
        {
            InventoryItemConfig target = FindTargetConsumable();

            if (target == null)
            {
                return $"РАСХОДНИК НЕ НАЙДЕН (ищем Id='{_bagUpgradeConfig.TargetConsumableId}')";
            }

            int tier = _playerUpgradesService.GetTier(_bagUpgradeConfig.ItemId);
            int capacity = _bagUpgradeConfig.GetCapacityFor(tier, target.MaxCharges);

            return $"{capacity} (база {target.MaxCharges})";
        }

        private InventoryItemConfig FindTargetConsumable()
        {
            if (_playerInventoryConfig == null)
            {
                return null;
            }

            foreach (InventoryItemConfig consumable in _playerInventoryConfig.StartingConsumables)
            {
                if (consumable == null)
                {
                    continue;
                }

                if (consumable.Id == _bagUpgradeConfig.TargetConsumableId)
                {
                    return consumable;
                }
            }

            return null;
        }
#endif
    }
}
