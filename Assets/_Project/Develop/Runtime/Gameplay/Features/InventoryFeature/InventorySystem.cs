using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Configs.Inventory.Potions;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventorySystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly InventoryItemConfig[] _consumables;
        private readonly ProjectileFactory _projectileFactory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private Entity _playerEntity;

        private ReactiveVariable<bool> _intentUseItem;
        private ReactiveVariable<float> _intentSwitchItemDelta;
        private ReactiveVariable<Vector2> _intentAimDirection;

        private ReactiveVariable<int> _currentItemIndex;
        private ReactiveVariable<bool> _isUsingItem;
        private ReactiveEvent<InventoryItemConfig> _itemUsedEvent;

        private List<ReactiveVariable<int>> _chargesList;

        public InventorySystem(
            InventoryItemConfig[] consumables,
            ProjectileFactory projectileFactory,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _consumables = consumables;
            _projectileFactory = projectileFactory;
            _coroutinesPerformer = coroutinesPerformer;
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

            for (int i = 0; i < _consumables.Length; i++)
            {
                _chargesList.Add(new ReactiveVariable<int>(_consumables[i].MaxCharges));
            }
        }

        public void OnUpdate(float deltaTime)
        {
            HandleSwitchItem();

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

            _coroutinesPerformer.StartPerform(ResetUsingFlag());
        }

        private void ApplyInternalEffect(PotionItemConfig config)
        {
            // _playerEntity.AddSpeedBuffModifier(config.SpeedMultiplier);
            // _playerEntity.AddBuffDuration(config.Duration);
            Debug.Log($"used {config.name} item");
        }

        private IEnumerator ResetUsingFlag()
        {
            yield return new WaitForSeconds(0.15f);

            _isUsingItem.Value = false;
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