using Assets._Project.Develop.Runtime.Configs.Gameplay.Props;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Props
{
    public class PropEntityAuthoring : MonoEntity
    {
        [SerializeField] private PropConfig _config;

        private EntitiesLifeContext _entitiesLifeContext;
        private IAudioService _audioService;
        private DropLootService _dropLootService;
        private CollidersRegistryService _collidersRegistryService;

        private Collider2D[] _propColliders; 

        public void Construct(
            EntitiesLifeContext entitiesLifeContext,
            IAudioService audioService,
            DropLootService dropLootService,
            CollidersRegistryService collidersRegistryService)
        {
            _entitiesLifeContext = entitiesLifeContext;
            _audioService = audioService;
            _dropLootService = dropLootService;
            _collidersRegistryService = collidersRegistryService;
        }

        private void Start()
        {
            if (_entitiesLifeContext == null)
            {
                Debug.LogError($"[Prop] Контекст не внедрен в {gameObject.name}!");
                return;
            }

            BuildPropEntity();

            _entitiesLifeContext.Released += OnEntityReleased;
        }

        private void BuildPropEntity()
        {
            Entity entity = new Entity();

            entity
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddMaxHealth(new ReactiveVariable<float>(_config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(_config.MaxHealth))
                .AddIsDead()
                .AddInDeathProcess(new ReactiveVariable<bool>(false))
                .AddTransform(transform);

            entity.AddLootIsDropped(new ReactiveVariable<bool>(false));

            ICompositeCondition canDropLoot = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));
            entity.AddCanDropLoot(canDropLoot);

            ICompositeCondition canApplyDamage = new CompositeCondition().Add(new FuncCondition(() => entity.IsDead.Value == false));
            ICompositeCondition mustDie = new CompositeCondition().Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));
            ICompositeCondition mustSelfRelease = new CompositeCondition().Add(new FuncCondition(() => entity.IsDead.Value == true));

            entity
                .AddCanApplyDamage(canApplyDamage)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            if (_config.LootTable != null)
            {
                entity.AddSystem(new DropLootSystem(_dropLootService, _config.LootTable));
            }

            _propColliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in _propColliders)
            {
                _collidersRegistryService.Register(col, entity);
            }

            // Привязка вьюшек
            EntityView[] views = GetComponentsInChildren<EntityView>();

            foreach (EntityView view in views)
            {
                if (view is PropVisualsView propVisual)
                {
                    propVisual.Construct(_audioService);
                }

                view.Link(entity);
            }

            LinkedEntity = entity;
            _entitiesLifeContext.Add(entity);
        }

        private void OnEntityReleased(Entity entity)
        {
            if (entity == LinkedEntity)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;
                UnregisterColliders();
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_entitiesLifeContext != null)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;
            }
            UnregisterColliders();
        }

        private void UnregisterColliders()
        {
            if (_propColliders == null || _collidersRegistryService == null) return;

            foreach (var col in _propColliders)
            {
                if (col != null)
                {
                    _collidersRegistryService.Unregister(col);
                }
            }
        }
    }
}