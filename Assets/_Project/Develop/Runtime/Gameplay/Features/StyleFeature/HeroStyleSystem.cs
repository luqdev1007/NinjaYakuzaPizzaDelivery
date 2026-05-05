using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature
{
    public class HeroStyleSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private readonly StyleEvaluator _evaluator;
        private readonly RankStyleService _service;
        private readonly List<IDisposable> _disposables = new();
        private Entity _entity;

        public HeroStyleSystem(StyleEvaluator evaluator, RankStyleService service)
        {
            _evaluator = evaluator;
            _service = service;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _disposables.Add(_entity.SuccessfulHitEvent.Subscribe(() =>
                _evaluator.ProcessDamage(_entity, _entity.AttackDamage.Value, "Melee_Attack")));

            _disposables.Add(_entity.TakeDamageEvent.Subscribe(_ =>
                _evaluator.ProcessPlayerHit(_entity)));

            _disposables.Add(_entity.IsDashing.Subscribe((_, current) => {
                if (current) _evaluator.ProcessDash(_entity);
            }));

            _disposables.Add(_entity.LootPickedEvent.Subscribe(_ =>
                _evaluator.ProcessLoot(_entity)));
        }

        public void OnUpdate(float deltaTime)
        {
            _service.UpdateDecay(_entity, deltaTime);
        }

        public void OnDispose()
        {
            foreach (var d in _disposables) d.Dispose();
            _disposables.Clear();
        }
    }
}