using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
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
        private readonly IInputService _inputService;
        private readonly ThrowableConfig[] _consumables;
        private readonly IThrowableBehaviourFactory _factory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ReactiveVariable<int> _currentIndex;
        private ReactiveVariable<bool> _isThrowing;
        private Transform _transform;
        private Rigidbody2D _rigidbody;

        private Dictionary<int, ReactiveVariable<int>> _chargesMap;

        public InventorySystem(IInputService input, ThrowableConfig[] consumables, IThrowableBehaviourFactory factory, ICoroutinesPerformer coroutinesPerformer)
        {
            _inputService = input;
            _consumables = consumables;
            _factory = factory;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _currentIndex = entity.CurrentThrowableIndex;
            _isThrowing = entity.IsThrowing;
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;

            _chargesMap = new Dictionary<int, ReactiveVariable<int>>
            {
                { 0, entity.ShurikenCharges },
                { 1, entity.SleepDartCharges }
            };
        }

        public void OnUpdate(float deltaTime)
        {
            HandleScroll();

            if (_inputService.IsThrowKeyPressed)
            {
                TryThrow();
            }
        }

        private void TryThrow()
        {
            int currentIdx = _currentIndex.Value;

            if (_chargesMap[currentIdx].Value <= 0) return;

            _chargesMap[currentIdx].Value--;
            _isThrowing.Value = true; // ВКЛ анимацию

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = ((Vector2)mousePos - (Vector2)_transform.position).normalized;

            var projectile = _factory.Create(_consumables[currentIdx], _rigidbody, _transform);
            projectile.Launch(_transform.position, direction);

            // Сброс анимации через 0.15 сек, чтобы не висела
            _coroutinesPerformer.StartPerform(ResetThrowingFlag());
        }

        private IEnumerator ResetThrowingFlag()
        {
            yield return new WaitForSeconds(0.15f);
            _isThrowing.Value = false;
        }

        private void HandleScroll()
        {
            float scroll = _inputService.MouseScrollDelta;
            if (Mathf.Abs(scroll) < 0.01f) return;

            int newIndex = _currentIndex.Value + (scroll > 0 ? 1 : -1);
            if (newIndex < 0) newIndex = _consumables.Length - 1;
            if (newIndex >= _consumables.Length) newIndex = 0;

            _currentIndex.Value = newIndex;
        }
    }
}