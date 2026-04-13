using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class HeroStyleSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private readonly StyleEvaluator _evaluator;
        private readonly StyleService _service;

        private Entity _entity;
        private readonly List<IDisposable> _disposables = new();

        public HeroStyleSystem(StyleEvaluator evaluator, StyleService service)
        {
            _evaluator = evaluator;
            _service = service;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            // 1. Успешное попадание (событие без аргументов)
            _disposables.Add(_entity.SuccessfulHitEvent.Subscribe(() =>
            {
                _evaluator.ProcessDamage(_entity.AttackDamage.Value, "Melee_Attack");
            }));

            // 2. Получение урона героем (принимает DamageData, игнорируем его для триггера штрафа)
            _disposables.Add(_entity.TakeDamageEvent.Subscribe(_ => _evaluator.ProcessPlayerHit()));

            // 3. Рывок (ReactiveVariable передает oldValue и newValue)
            _disposables.Add(_entity.IsDashing.Subscribe((prev, current) =>
            {
                if (current) // Если значение стало true
                {
                    _evaluator.ProcessDash();
                }
            }));

            // 4. Пике (аналогично рывку)
            _disposables.Add(_entity.IsPlunging.Subscribe((prev, current) =>
            {
                if (current)
                {
                    _evaluator.ProcessDamage(_entity.PlungeAOEDamage.Value, "Plunge_Attack");
                }
            }));

                _disposables.Add(_entity.LootPickedEvent.Subscribe(lootType =>
                {
                    _evaluator.ProcessLootPick();
                    UnityEngine.Debug.Log($"Loot Picked: {lootType}. Style points added!");
                }));
            }

        public void OnUpdate(float deltaTime)
        {
            _service.UpdateDecay(deltaTime); // УБЕДИСЬ, ЧТО ЭТА СТРОКА ЕСТЬ

            if (_entity.Rigidbody.linearVelocity.y > 5f)
            {
                // Очки должны начисляться с учетом deltaTime!
                _evaluator.ProcessMovementAcceleration(deltaTime);
            }
        }

        public void OnDispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();

            _evaluator.Dispose();
        }
    }
}