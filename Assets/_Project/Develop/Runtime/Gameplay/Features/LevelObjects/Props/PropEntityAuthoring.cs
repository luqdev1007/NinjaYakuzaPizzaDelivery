using Assets._Project.Develop.Runtime.Configs.Gameplay.Props;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
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

        public void Construct(EntitiesLifeContext entitiesLifeContext, IAudioService audioService)
        {
            _entitiesLifeContext = entitiesLifeContext;
            _audioService = audioService;
        }

        private void Start()
        {
            if (_entitiesLifeContext == null)
            {
                Debug.LogError($"[Prop] Контекст не внедрен в {gameObject.name}! Забыли вызвать Construct в Bootstrap?");
                return;
            }

            BuildPropEntity();

            // [ДОБАВЛЕНО]: Подписываемся на глобальное событие освобождения сущностей
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
                .AddInDeathProcess(new ReactiveVariable<bool>(false));

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

        // [ДОБАВЛЕНО]: Ловим момент, когда SelfReleaseSystem удалит сущность из контекста
        private void OnEntityReleased(Entity entity)
        {
            if (entity == LinkedEntity)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;

                // Убираем Cleanup(entity); — он здесь лишний и ломает код из-за неинициализированных базовых полей

                // Физически удаляем объект со сцены
                Destroy(gameObject);
            }
        }

        // [ДОБАВЛЕНО]: Обязательно отписываемся при уничтожении (например, при смене сцены), чтобы не плодить утечки памяти
        private void OnDestroy()
        {
            if (_entitiesLifeContext != null)
            {
                _entitiesLifeContext.Released -= OnEntityReleased;
            }
        }
    }
}