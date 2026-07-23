using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities;
using UnityEngine;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles
{
    public class ProjectileFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly MonoEntitiesFactory _monoEntitiesFactory;
        private readonly CollidersRegistryService _collidersRegistryService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public ProjectileFactory(DIContainer container)
        {
            _container = container;

            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
            _monoEntitiesFactory = container.Resolve<MonoEntitiesFactory>();
            _collidersRegistryService = container.Resolve<CollidersRegistryService>();
            _coroutinesPerformer = container.Resolve<ICoroutinesPerformer>();
        }

        public Entity CreateChargedSlashProjectile(Transform parent, Entity owner)
        {
            // settings (config)
            string prefabPath = "Entities/Projectiles/ChargedSlashProjectile";
            float damage = 10;
            float speed = 15;
            float lifeTime = 2;
            LayerMask hitMask = LayersAPI.LayerMaskEnemies;

            Entity entity = new Entity();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, prefabPath);
            mono.transform.SetParent(null);

            entity
                .AddMoveDirection(new ReactiveVariable<Vector2>(new Vector2(owner.LookDirectionX.Value, 0)))
                .AddLookDirectionX()
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(speed))

                .AddLifeTime(new ReactiveVariable<float>(lifeTime))

                .AddBodyContactDamage(new ReactiveVariable<float>(damage))

                .AddContactsDetectingMask(hitMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))

                .AddTeam(owner.Team)
                ;

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => true))
                ;

            ICompositeCondition mustSelfRelease = new CompositeCondition()
                .Add(new FuncCondition(() => entity.LifeTime.Value <= 0))
                ;

            entity
                .AddCanMove(canMove)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new DealDamageOnContactSystem())

                .AddSystem(new TransformMovementSystem())

                .AddSystem(new TimedLifeTimeSystem())
                .AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        /// <summary>
        /// Язык слайма. Отдельная сущность, а НЕ дочерний узел слайма: катана
        /// мапит цель через GetComponentInParent&lt;MonoEntity&gt;, поэтому удар по
        /// дочернему языку был бы неотличим от удара по слайму. Плюс
        /// CollidersRegistryService.Register использует Dictionary.Add — повторная
        /// регистрация того же коллайдера бросила бы ArgumentException.
        ///
        /// Уставки СПЕЦИАЛЬНО не хардкодятся в теле метода (в соседних методах
        /// этой фабрики они захардкожены под комментарием "// settings (config)" —
        /// эту практику не наследуем): всё, что нужно знать языку, приезжает
        /// параметрами, остальным рулит TongueSystem на стороне слайма.
        ///
        /// Состав сущности намеренно минимальный. Здоровья, смерти, лута,
        /// самоосвобождения и контактной цепочки у языка нет: разрубаемость
        /// катаной обеспечивает один TakeDamageRequest, а MeleeAttackHitSystem
        /// на такой сущности отрабатывает штатно — Invoke по пустому списку
        /// подписчиков проходит, CurrentHealth никто не читает.
        /// Жизненным циклом языка управляет TongueSystem через EntitiesLifeContext.
        /// </summary>
        public Entity CreateSlimeTongue(Vector2 startPosition, string prefabPath)
        {
            Entity entity = new Entity();

            // Перегрузка с позицией уже инстанцирует без родителя, поэтому
            // SetParent(null) соседних методов здесь не нужен.
            _monoEntitiesFactory.Create(entity, (Vector3)startPosition, prefabPath);

            // ЗАЧЕМ ЯЗЫКУ ЗАВЕДОМО ЛОЖНОЕ CanApplyDamage.
            //
            // Компонент нужен НЕ для урона, а чтобы выключить язык из
            // автонаведения. NearestDamagableTargetSelector.IsValidTarget
            // проверяет цель так: TryGetCanApplyDamage(...) && Evaluate() == false.
            // Отсутствие компонента роняет левую часть && — и проверка
            // ПРОПУСКАЕТСЯ ЦЕЛИКОМ. Язык при этом проходит все остальные
            // критерии селектора (TakeDamageRequest есть, дистанция мала по
            // определению), поэтому без этой строки лок-он героя прыгает
            // на язык вместо слайма.
            //
            // ОБЩИЙ ПАТТЕРН ЭТОЙ МАШИНЕРИИ, который здесь и кусается:
            // ОТСУТСТВУЮЩИЙ КОМПОНЕНТ РАЗРЕШАЕТ, А НЕ ЗАПРЕЩАЕТ. То же самое
            // у EntitiesHelper.TryTakeDamageFrom с Team. Сущности минимального
            // состава (а язык именно такая) от этого страдают систематически.
            //
            // РАЗРУБ КАТАНОЙ ЭТИМ НЕ ЗАДЕВАЕТСЯ, и вот почему это держится:
            // CanApplyDamage читает ApplyDamageSystem, а её на языке НЕТ —
            // у языка вообще нет систем, TongueSystem подписана на его
            // TakeDamageRequest напрямую. MeleeAttackHitSystem проверяет только
            // HasComponent<TakeDamageRequest>() и бьёт в него в обход
            // EntitiesHelper.
            // МИНА НА БУДУЩЕЕ: добавишь языку ApplyDamageSystem — эта константа
            // молча выключит ВЕСЬ урон по языку, и разруб перестанет работать
            // без единой ошибки компиляции.
            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => false))
                ;

            // TEAM ЯЗЫКУ НЕ ВЫДАЁТСЯ — ОСОЗНАННО, НЕ ЗАБЫТО.
            //
            // Единственный путь контактного урона от героя — это
            // LethalContactMovementSystem ("скорость = урон"), и она ходит через
            // EntitiesHelper.TryTakeDamageFrom, то есть ТИМ-ФИЛЬТРУЕТСЯ. Выдай
            // языку Team = Enemies — герой и язык окажутся в одной команде
            // относительно этого фильтра, и дэш/пике сквозь язык перестанут его
            // рубить.
            // Это прямо противоречит design pillar "скорость = урон": пролёт
            // сквозь язык на скорости ОБЯЗАН его срезать. Поэтому язык остаётся
            // нейтралом без Team и уязвим для тарана.
            entity
                .AddTakeDamageRequest()
                .AddTongueOriginPoint(new ReactiveVariable<Vector2>(startPosition))
                .AddCanApplyDamage(canApplyDamage)
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        /// <summary>
        /// Снаряд фонаря (Lantern). Автономный fire-and-forget: летит прямо в
        /// заданном направлении, гаснет об геометрию, по времени жизни или от разруба.
        ///
        /// TEAM НЕ ВЫДАЁТСЯ — ОСОЗНАННО, ровно как языку слайма: рубка дэшем/пике
        /// идёт через тим-фильтр EntitiesHelper.TryTakeDamageFrom, а нейтрал без
        /// Team проходит фильтр и потому уязвим для тарана (design pillar
        /// «скорость = урон»). Контактный урон герою при этом работает: source
        /// (снаряд) без Team роняет левый операнд && в тим-фильтре, проверка
        /// пропускается, урон уходит.
        ///
        /// CanApplyDamage = false — так же, как языку: выключает снаряд из
        /// автонаведения героя (NearestDamagableTargetSelector). Разруб этим не
        /// задет: у снаряда нет ApplyDamageSystem, LanternProjectileSystem подписан
        /// на TakeDamageRequest напрямую. МИНА: добавишь снаряду ApplyDamageSystem —
        /// эта константа молча выключит ВЕСЬ урон по нему, и разруб отвалится без
        /// ошибки компиляции.
        ///
        /// ДЕСПАВН ОБ ГЕОМЕТРИЮ ЖИВЁТ НЕ ЗДЕСЬ. Он целиком внутри
        /// LanternProjectileSystem — Linecast по BlockMask из предыдущей позиции в
        /// новую, с ignore-set'ом коллайдеров, снятым в точке вылета. Поэтому
        /// ContactsDetectingMask здесь — ТОЛЬКО TargetMask (герой).
        ///
        /// ГЕОМЕТРИЮ В ContactsDetectingMask НЕ ДОБАВЛЯТЬ. Ровно на этом снаряд
        /// ломался в прошлой итерации: маска включала Default(0), на нём висит
        /// LevelBounds (охватывающий уровень триггер), overlap тела ловил его на
        /// первом же Update, DeathMaskTouchDetectorSystem поднимала IsTouchDeathMask
        /// — и снаряд гас в точке вылета, каждый выстрел. Сюрикеновый паттерн
        /// (DeathMask + IsTouchDeathMask + DeathMaskTouchDetectorSystem) здесь не
        /// заводим ещё и потому, что он туннелирует на скорости: overlap проверяет
        /// точку, а не пройденный за тик отрезок.
        ///
        /// Движение, каст и время жизни — на fixed (LanternProjectileSystem),
        /// контактная цепочка — на Update (общие системы проекта). См. шапку
        /// LanternProjectileSystem.
        /// </summary>
        public Entity CreateLanternProjectile(Vector2 position, Vector2 direction, LanternProjectileData data)
        {
            Entity entity = new Entity();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, (Vector3)position, data.PrefabPath);

            // Ориентация спрайта по направлению полёта — как в CreateThrowableProjectile.
            Vector2 flyDirection = direction.normalized;
            float angle = Mathf.Atan2(flyDirection.y, flyDirection.x) * Mathf.Rad2Deg;
            mono.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => false))
                ;

            entity
                .AddMoveDirection(new ReactiveVariable<Vector2>(flyDirection))
                .AddMoveSpeed(new ReactiveVariable<float>(data.Speed))
                .AddLifeTime(new ReactiveVariable<float>(data.LifeTime))

                .AddTakeDamageRequest()
                .AddCanApplyDamage(canApplyDamage)

                // Маска детекта — только герой. Геометрия сюда не идёт: деспавн об
                // стену ведётся кастом в LanternProjectileSystem (см. шапку метода).
                .AddBodyContactDamage(new ReactiveVariable<float>(data.ContactDamage))
                .AddContactsDetectingMask(data.TargetMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(16))
                .AddContactEntitiesBuffer(new Buffer<Entity>(16))
                ;

            entity
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
                .AddSystem(new DealDamageOnContactSystem())

                .AddSystem(new LanternProjectileSystem(_entitiesLifeContext, data.BlockMask))
                ;

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        public void CreateThrowableProjectile(ThrowableItemConfig throwableConfig, Transform parent, Vector2 aimDirection, Entity playerEntity)
        {
            Entity entity = new Entity();

            MonoEntity mono = _monoEntitiesFactory.Create(entity, parent, throwableConfig.PrefabPath);
            Vector3 startPosition = mono.transform.position;
            mono.transform.SetParent(null);

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            mono.transform.rotation = Quaternion.Euler(0, 0, angle);

            entity
                .AddMoveDirection(new ReactiveVariable<Vector2>(aimDirection))
                .AddLookDirectionX()
                .AddIsMoving()
                .AddMoveSpeed(new ReactiveVariable<float>(throwableConfig.ProjectileSpeed))

                .AddContactsDetectingMask(throwableConfig.HitMask)
                .AddContactCollidersBuffer(new Buffer<Collider2D>(64))
                .AddContactEntitiesBuffer(new Buffer<Entity>(64))

                .AddTeam(playerEntity.Team);

            ICompositeCondition canMove = new CompositeCondition()
                .Add(new FuncCondition(() => true));

            ICompositeCondition mustSelfRelease = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => Vector3.Distance(startPosition, entity.Transform.position) >= throwableConfig.MaxFlyDistance));

            entity
                .AddCanMove(canMove)
                .AddMustSelfRelease(mustSelfRelease);

            entity
                .AddSystem(new TransformMovementSystem())
                .AddSystem(new BodyContactDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService));

            if (throwableConfig is ShurikenConfig shurikenConfig)
            {
                entity
                    .AddBodyContactDamage(new ReactiveVariable<float>(shurikenConfig.Damage))
                    .AddIsTouchDeathMask()
                    .AddDeathMask(shurikenConfig.HitMask);

                mustSelfRelease.Add(new FuncCondition(() => entity.IsTouchDeathMask.Value == true));

                entity
                    .AddSystem(new DealDamageOnContactSystem())
                    .AddSystem(new DeathMaskTouchDetectorSystem());
            }

            entity.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

            _entitiesLifeContext.Add(entity);
        }
    }
}
