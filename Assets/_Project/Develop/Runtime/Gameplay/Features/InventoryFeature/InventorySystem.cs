using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Configs.Inventory.Potions;
using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Meta.Features.Upgrades;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventorySystem : IInitializableSystem, IUpdatableSystem
    {
        private const float UseResetTime = 0.15f;

        private readonly InventoryItemConfig[] _consumables;
        private readonly ProjectileFactory _projectileFactory;
        private readonly PlayerUpgradesService _playerUpgradesService;
        private readonly BagUpgradeConfig _bagUpgradeConfig;

        private Entity _playerEntity;

        private ReactiveVariable<bool> _intentUseItem;
        private ReactiveVariable<float> _intentSwitchItemDelta;
        private ReactiveVariable<Vector2> _intentAimDirection;

        private ReactiveVariable<int> _currentItemIndex;
        private ReactiveVariable<bool> _isUsingItem;
        private ReactiveEvent<InventoryItemConfig> _itemUsedEvent;

        private List<ReactiveVariable<int>> _chargesList;

        private float _useResetTimer;

        public InventorySystem(
            InventoryItemConfig[] consumables,
            ProjectileFactory projectileFactory,
            PlayerUpgradesService playerUpgradesService,
            BagUpgradeConfig bagUpgradeConfig)
        {
            _consumables = consumables;
            _projectileFactory = projectileFactory;
            _playerUpgradesService = playerUpgradesService;
            _bagUpgradeConfig = bagUpgradeConfig;
        }

        public void OnInit(Entity entity)
        {
            _playerEntity = entity;

            _intentUseItem = entity.IntentUseItem;
            _intentSwitchItemDelta = entity.IntentSwitchItemDelta;
            _intentAimDirection = entity.IntentAimDirection;

            _currentItemIndex = entity.CurrentItemIndex;
            _isUsingItem = entity.IsUsingItem;
            _itemUsedEvent = entity.ItemUsedEvent;

            _chargesList = entity.InventoryCharges;
            _chargesList.Clear();

            // Рефил до капасити на каждом старте уровня. Схема расходников A:
            // количество не покупается и между забегами не хранится — покупается
            // вместимость, и забег всегда начинается полным.
            for (int i = 0; i < _consumables.Length; i++)
            {
                _chargesList.Add(new ReactiveVariable<int>(GetCapacityFor(_consumables[i])));
            }
        }

        /// <summary>
        /// Вместимость слота. Апгрейд сумки прокачивает РОВНО ОДИН расходник —
        /// тот, чей Id совпал с BagUpgradeConfig.TargetConsumableId. Всем
        /// остальным капасити = MaxCharges из их собственного конфига, как было.
        /// </summary>
        private int GetCapacityFor(InventoryItemConfig consumable)
        {
            if (consumable == null)
                return 0;

            if (_bagUpgradeConfig == null)
                return consumable.MaxCharges;

            if (consumable.Id != _bagUpgradeConfig.TargetConsumableId)
                return consumable.MaxCharges;

            int tier = _playerUpgradesService.GetTier(_bagUpgradeConfig.ItemId);

            return _bagUpgradeConfig.GetCapacityFor(tier, consumable.MaxCharges);
        }

        public void OnUpdate(float deltaTime)
        {
            HandleSwitchItem();

            if (_isUsingItem.Value)
            {
                _useResetTimer -= deltaTime;

                if (_useResetTimer <= 0f)
                {
                    _isUsingItem.Value = false;
                }
            }

            if (_intentUseItem.Value)
            {
                TryUseActiveItem();
            }
        }

        private void TryUseActiveItem()
        {
            if (_isUsingItem.Value)
                return;

            int currentIndex = _currentItemIndex.Value;

            if (_chargesList[currentIndex].Value <= 0)
                return;

            InventoryItemConfig activeConfig = _consumables[currentIndex];

            _chargesList[currentIndex].Value--;
            _isUsingItem.Value = true;
            _useResetTimer = UseResetTime;

            _itemUsedEvent?.Invoke(activeConfig);

            Vector2 aimDirection = _intentAimDirection.Value;

            if (activeConfig is ThrowableItemConfig throwableConfig)
            {
                _projectileFactory.CreateThrowableProjectile(throwableConfig, _playerEntity.ShootPoint, aimDirection, _playerEntity);
            }
            else if (activeConfig is PotionItemConfig potionConfig)
            {
                ApplyInternalEffect(potionConfig);
            }
        }

        private void ApplyInternalEffect(PotionItemConfig config)
        {
            // _playerEntity.AddSpeedBuffModifier(config.SpeedMultiplier);
            // _playerEntity.AddBuffDuration(config.Duration);
            Debug.Log($"used {config.name} item");
        }

        private void HandleSwitchItem()
        {
            if (_consumables == null || _consumables.Length == 0)
                return;

            float scrollDelta = _intentSwitchItemDelta.Value;

            if (Mathf.Abs(scrollDelta) < 0.01f)
                return;

            int newIndex = _currentItemIndex.Value + (scrollDelta > 0 ? 1 : -1);

            if (newIndex < 0)
                newIndex = _consumables.Length - 1;

            if (newIndex >= _consumables.Length)
                newIndex = 0;

            _currentItemIndex.Value = newIndex;

            InventoryItemConfig activeItem = _consumables[newIndex];
            string itemName = activeItem != null ? activeItem.name : "Null Config";

            Debug.Log($"<color=#FFD700>[Inventory]</color> Сменили слот на: <b>[{newIndex}]</b> — {itemName} (Тип: {activeItem?.GetType().Name})");
        }
    }
}
