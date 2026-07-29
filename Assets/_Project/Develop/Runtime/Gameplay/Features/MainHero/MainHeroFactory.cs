using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Meta.Shop;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Upgrades;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroFactory
    {
        private const float MinChancePercent = 0f;
        private const float MaxChancePercent = 100f;

        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly BuffService _buffService;

        // Перманентные покупки из профиля. Резолвятся из project-скоупа
        // через родителя — фабрика живёт в gameplay-скоупе, контейнер
        // иерархический (тот же приём, что у ProjectileFactory для урона
        // сюрикена и у InventorySystem для капасити сумки).
        //
        // Товары берутся из ShopCatalogConfig, а не регистрацией каждого конфига
        // в ResourcesConfigsLoader: ConfigsProviderService ключуется ТИПОМ, а
        // стат-апгрейдов два одного типа StatUpgradeConfig, второй просто затёр
        // бы первый. Каталог заодно остаётся единственным списком того, что
        // вообще продаётся.
        private readonly PlayerUpgradesService _playerUpgradesService;
        private readonly ShopCatalogConfig _shopCatalogConfig;

        public MainHeroFactory(DIContainer container)
        {
            _container = container;

            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _buffService = _container.Resolve<BuffService>();

            _playerUpgradesService = _container.Resolve<PlayerUpgradesService>();
            _shopCatalogConfig = _configsProviderService.GetConfig<ShopCatalogConfig>();
        }

        public Entity Create(Vector3 at)
        {
            MainHeroConfig config = _configsProviderService.GetConfig<MainHeroConfig>();

            Entity entity = _entitiesFactory.CreateHero(at, config);

            entity
                .AddIsMainHero()
                .AddTeam(new ReactiveVariable<Teams>(Teams.MainHero))

                .AddIntentSwitchTarget()
                .AddIsTargetingActive()
                .AddCurrentTarget()

                .AddLootCollectRange(new ReactiveVariable<float>(config.LootCollectRange))
                .AddLootPickedEvent()

                .AddBaseMoveSpeed(new ReactiveVariable<float>(config.Movement.MoveSpeed))
                .AddMoveSpeedModifiers(new StatModifiersList())
                .AddBaseLootCollectRange(new ReactiveVariable<float>(config.LootCollectRange))
                .AddLootCollectRangeModifiers(new StatModifiersList())

                // Уклонение. Итоговый EvasionChance заводится НУЛЁМ намеренно:
                // единственный его писатель — EvasionChanceStatSynchronizerSystem,
                // и он перезапишет значение своим Recalculate в OnInit. Дублировать
                // сюда config-значение (как это сделано выше у LootCollectRange)
                // не стали — двойное авторство одного числа только путает.
                //
                // EvadedEvent ОБЯЗАН стоять рядом со статом и добавляться вместе с
                // ним. Без него бросок продолжает отменять урон (за это отвечает
                // EvasionChance), но визуал молча не срабатывает: и
                // ApplyDamageSystem, и AfterimageView читают событие через TryGet.
                // Именно так этот баг и появился — стат был, события не было, ни
                // одной ошибки при этом не печаталось. Обе стороны теперь ругаются
                // в лог на такую сборку, но чинится она здесь, одной строкой.
                .AddBaseEvasionChance(new ReactiveVariable<float>(config.LifeCycle.EvasionChance))
                .AddEvasionChanceModifiers(new StatModifiersList())
                .AddEvasionChance(new ReactiveVariable<float>(0f))
                .AddEvadedEvent()

                .AddActiveBuffs(new ActiveBuffsList())
                ;

            _brainsFactory.CreateMainHeroBrain(entity);

            entity
                .AddSystem(new MainHeroStyleSystem(_container.Resolve<StyleEvaluator>()))
                .AddSystem(new LootMagnetSystem(_entitiesLifeContext))
                .AddSystem(new LootDistanceCollectSystem(_entitiesLifeContext, _container.Resolve<SessionLootService>()))
                .AddSystem(new TargetingCoreSystem(_entitiesLifeContext))

                .AddSystem(new MoveSpeedStatSynchronizerSystem())
                .AddSystem(new LootCollectRangeStatSynchronizerSystem())
                .AddSystem(new EvasionChanceStatSynchronizerSystem())
                .AddSystem(new BuffsTimerSystem())
                .AddSystem(new BuffMagnetSystem(_entitiesLifeContext))
                .AddSystem(new BuffDistanceCollectSystem(_entitiesLifeContext, _buffService))
                ;

            ApplyPurchasedUpgrades(entity);

            _entitiesLifeContext.Add(entity);

            return entity;
        }

        /// <summary>
        /// Накатывает всё купленное в магазине на уже собранного героя.
        ///
        /// Зовётся ДО EntitiesLifeContext.Add: именно там entity.Initialize()
        /// прогоняет OnInit систем, и синхронизатор уклонения делает свой первый
        /// Recalculate. Добавив модификатор раньше, мы получаем правильное
        /// значение с первого же кадра и не полагаемся на событие Changed.
        ///
        /// Один проход по каталогу с разбором по ТИПУ конфига, а не по ItemId:
        /// строковые id принадлежат сейву и меняются балансом, типы —
        /// компилятору. Исключение одно — анлок, у которого цель выражена
        /// собственным enum'ом AbilityUnlockTarget (см. AbilityUnlockConfig).
        /// </summary>
        private void ApplyPurchasedUpgrades(Entity entity)
        {
            for (int i = 0; i < _shopCatalogConfig.Items.Count; i++)
            {
                ShopItemConfigBase itemConfig = _shopCatalogConfig.Items[i];

                if (itemConfig == null)
                {
                    continue;
                }

                int tier = _playerUpgradesService.GetTier(itemConfig.ItemId);

                if (tier == 0)
                {
                    continue;
                }

                ApplyPurchase(entity, itemConfig, tier);
            }

            ApplyChargedSlashUnlockGate(entity);
        }

        private void ApplyPurchase(Entity entity, ShopItemConfigBase itemConfig, int tier)
        {
            if (itemConfig is StatUpgradeConfig statUpgradeConfig)
            {
                ApplyStatBonus(entity, statUpgradeConfig.TargetStat, statUpgradeConfig.GetStatBonusFor(tier));

                return;
            }

            if (itemConfig is ChargedSlashChargesUpgradeConfig chargesUpgradeConfig)
            {
                // База (BaseChargedSlashCharges) уже лежит в компоненте из
                // EntitiesFactory — здесь только прибавка, как и у остальных
                // покупок. Второго компонента не заводим.
                entity.ChargedSlashCharges.Value += chargesUpgradeConfig.GetChargesBonusFor(tier);

                return;
            }

            // Power и Reach на героя не влияют — они читаются на спавне снаряда
            // в ProjectileFactory, где живут все его числа.
        }

        /// <summary>
        /// Заряженный слэш недоступен, пока не куплен анлок.
        ///
        /// Дописываем условие в СУЩЕСТВУЮЩИЙ composite CanChargeSlashAttack
        /// вместо нового компонента-маркера: гейт способности уже есть и уже
        /// проверяется в SlashAttackChargeSystem, а новый IEntityComponent
        /// потребовал бы регенерации EntityAPI и завёл бы второй источник
        /// правды рядом с первым. Дописывание в собранный composite — приём из
        /// этого же кода (ProjectileFactory: mustSelfRelease.Add).
        ///
        /// unlocked считается ОДИН раз и захватывается замыканием: покупка
        /// в главном меню применяется со следующего забега, а не посреди
        /// текущего — герой всё равно пересобирается на каждый старт уровня.
        /// </summary>
        private void ApplyChargedSlashUnlockGate(Entity entity)
        {
            bool unlocked = IsAbilityUnlocked(AbilityUnlockTarget.ChargedSlash);

            entity.CanChargeSlashAttack.Add(new FuncCondition(() => unlocked));
        }

        private bool IsAbilityUnlocked(AbilityUnlockTarget target)
        {
            for (int i = 0; i < _shopCatalogConfig.Items.Count; i++)
            {
                AbilityUnlockConfig unlockConfig = _shopCatalogConfig.Items[i] as AbilityUnlockConfig;

                if (unlockConfig == null)
                {
                    continue;
                }

                if (unlockConfig.TargetAbility != target)
                {
                    continue;
                }

                if (_playerUpgradesService.GetTier(unlockConfig.ItemId) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyStatBonus(Entity entity, StatUpgradeTarget targetStat, float bonus)
        {
            switch (targetStat)
            {
                case StatUpgradeTarget.EvasionChance:
                    // Модификатор ложится В ДОПОЛНЕНИЕ к базе из конфига, второго
                    // компонента не заводит: база — BaseEvasionChance, покупка —
                    // строчка в EvasionChanceModifiers, итог считает синхронизатор.
                    // Он же клампит 0..100, поэтому перебор бонусом безопасен.
                    //
                    // Перманентный: Remove не зовётся никогда, апгрейд живёт весь
                    // забег. Поэтому и не IBuffEffect — снимать нечего.
                    entity.EvasionChanceModifiers.Add(new AdditiveStatModifier(bonus));
                    break;

                case StatUpgradeTarget.DoubleAttackChance:
                    // ВНИМАНИЕ, ВТОРОЙ ПИСАТЕЛЬ. DoubleAttackChance пишется здесь
                    // (апгрейд) и в EntitiesFactory:278 (база из конфига). У крита
                    // НЕТ триплета Base/Modifiers/Synchronizer, как у уклонения, —
                    // значит нет и места, где значение собиралось бы из слагаемых,
                    // и порядок записей определяет результат. Сейчас это держится
                    // ровно потому, что писателей два и они упорядочены: фабрика
                    // сущности залила базу, мы прибавляем поверх.
                    //
                    // ПОЯВИТСЯ ТРЕТИЙ ПИСАТЕЛЬ (бафф, дебафф, ещё один товар) —
                    // строить триплет, а не дописывать сюда: иначе разъедется
                    // молча, как уже разъезжались CompletedLevels у двух сервисов
                    // и LootCollectRange у двух Add'ов в этой самой фабрике.
                    //
                    // Кламп здесь ручной по той же причине — синхронизатора,
                    // который сделал бы это за нас, не существует.
                    entity.DoubleAttackChance.Value = Mathf.Clamp(
                        entity.DoubleAttackChance.Value + bonus,
                        MinChancePercent,
                        MaxChancePercent);
                    break;
            }
        }
    }
}
