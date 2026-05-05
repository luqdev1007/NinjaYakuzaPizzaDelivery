using Assets._Project.Develop.Runtime.Configs.Gameplay.Inventory;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventorySystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly ConsumableConfig[] _items;
        private readonly IThrowableBehaviourFactory _factory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private Entity _entity;
        private ReactiveVariable<int> _currentIndex;
        private ReactiveVariable<bool> _isUsing;
        private InputState _throwInput;
        private ReactiveVariable<float> _scrollDelta;

        private Dictionary<int, ReactiveVariable<int>> _chargesMap;

        public readonly ReactiveEvent OnItemSwitched = new();
        public readonly ReactiveEvent OnItemUsed = new();
        public readonly ReactiveEvent OnEmptyTry = new();

        public InventorySystem(
            ConsumableConfig[] items,
            IThrowableBehaviourFactory factory,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _items = items;
            _factory = factory;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _currentIndex = entity.CurrentThrowableIndex;
            _isUsing = entity.IsThrowing;
            _throwInput = entity.ThrowInput;
            _scrollDelta = entity.InventoryScrollDelta;

            _chargesMap = new Dictionary<int, ReactiveVariable<int>>
            {
                { 0, entity.ShurikenCharges },
                { 1, entity.SleepDartCharges }
            };
        }

        public void OnUpdate(float deltaTime)
        {
            HandleScroll();

            if (_throwInput.IsPressed.Value)
            {
                TryUse();
            }
        }

        private void TryUse()
        {
            int currentIdx = _currentIndex.Value;
            var charges = _chargesMap[currentIdx];

            if (charges.Value <= 0)
            {
                OnEmptyTry.Invoke();
                return;
            }

            charges.Value--;
            _isUsing.Value = true;

            _items[currentIdx].Use(_entity, _factory);
            OnItemUsed.Invoke();

            _coroutinesPerformer.StartPerform(ResetUsageFlag());
        }

        private void HandleScroll()
        {
            float scroll = _scrollDelta.Value;

            if (Mathf.Abs(scroll) < 0.01f)
                return;

            int step = scroll > 0 ? 1 : -1;
            int newIndex = (_currentIndex.Value + step + _items.Length) % _items.Length;

            if (newIndex != _currentIndex.Value)
            {
                _currentIndex.Value = newIndex;
                OnItemSwitched.Invoke();
            }
        }

        private IEnumerator ResetUsageFlag()
        {
            yield return new WaitForSeconds(0.15f);
            _isUsing.Value = false;
        }
    }
}