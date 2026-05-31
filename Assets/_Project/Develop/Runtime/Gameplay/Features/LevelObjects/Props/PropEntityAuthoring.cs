using Assets._Project.Develop.Runtime.Configs.Gameplay.Props;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature; // Для DropLootSystem и DropLootService
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
        private DropLootService _dropLootService; // [ДОБАВЛЕНО] Поле под сервис лута

        // [ОБНОВЛЕНО] Внедряем DropLootService через параметры конструктора
        public void Construct(EntitiesLifeContext entitiesLifeContext, IAudioService audioService, DropLootService dropLootService)
        {
            _entitiesLifeContext = entitiesLifeContext;
            _audioService = audioService;
            _dropLootService = dropLootService;
        }

        private void Start()
        {
            if (_entitiesLifeContext == null)
            {
                Debug.LogError($"[Prop] Контекст не внедрен в {gameObject.name}! Забыли вызвать Construct в Bootstrap?");
                return;
            }

            BuildPropEntity();

            _entitiesLifeContext.Released += OnEntityReleased;
        }

        private void BuildPropEntity()
        {
            Entity entity = new Entity();

            // Базовые компоненты здоровья и смерти
            entity
                .AddTakeDamageRequest()
                .AddTakeDamageEvent()
                .AddMaxHealth(new ReactiveVariable<float>(_config.MaxHealth))
                .AddCurrentHealth(new ReactiveVariable<float>(_config.MaxHealth))
                .AddIsDead()
                .AddInDeathProcess(new ReactiveVariable<bool>(false))
                .AddTransform(transform)
                ;

            // [ДОБАВЛЕНО] Инициализируем компоненты лута
            entity.AddLootIsDropped(new ReactiveVariable<bool>(false));

            ICompositeCondition canDropLoot = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0)); // Дроп при ХП <= 0
            entity.AddCanDropLoot(canDropLoot);

            // Условия жизнедеятельности сущности
            ICompositeCondition canApplyDamage = new CompositeCondition().Add(new FuncCondition(() => entity.IsDead.Value == false));
            ICompositeCondition mustDie = new CompositeCondition().Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));
            ICompositeCondition mustSelfRelease = new CompositeCondition().Add(new FuncCondition(() => entity.IsDead.Value == true));

            entity
                .AddCanApplyDamage(canApplyDamage)
                .AddMustDie(mustDie)
                .AddMustSelfRelease(mustSelfRelease);

            // Базовые системы
            entity
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            // [ДОБАВЛЕНО] Вешаем систему лута, если у пропса в конфиге задана таблица дропа
            if (_config.LootTable != null)
            {
                entity.AddSystem(new DropLootSystem(_dropLootService, _config.LootTable));
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
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_entitiesLifeContext != null)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;
            }
        }
    }
}