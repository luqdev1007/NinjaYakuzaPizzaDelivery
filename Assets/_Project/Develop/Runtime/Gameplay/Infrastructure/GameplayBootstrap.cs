using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Context;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Patrol;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Props;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private GameplayInputArgs _inputArgs;
        private GameplayStatesContext _gameplayStatesContext;
        private EntitiesLifeContext _entitiesLifeContext;
        private AIBrainsContext _brainsContext;
        private GameplaySceneContext _sceneContext;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
            {
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)} type");
            }

            _inputArgs = gameplayInputArgs;
        }

        public override IEnumerator Initialize()
        {
            var configsProvider = _container.Resolve<ConfigsProviderService>();
            LevelConfig levelConfig = configsProvider.GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            GameObject levelHolder = GameObject.FindWithTag("LevelHolder");

            if (levelHolder == null)
                throw new NullReferenceException("LevelHolder not found");

            GameObject levelInstance = Instantiate(levelConfig.LevelPrefab, levelHolder.transform);

            _sceneContext = levelInstance.GetComponentInChildren<GameplaySceneContext>();
            if (_sceneContext == null)
                throw new NullReferenceException("GameplaySceneContext missing in Level Prefab");

            // СИДИРОВАНИЕ ГЕЙМПЛЕЙНОГО РАНДОМА.
            // Порядок здесь важен и держит весь детерминизм забега:
            //   1. seed пишется в _inputArgs ДО регистраций — Process читает его при
            //      создании IGameplayRandom;
            //   2. _container.Initialize() ниже создаёт NonLazy-инстанс уже засеянным;
            //   3. только после этого появляется первый потребитель рандома — спавн
            //      врагов (ниже по методу) и, позже, ClearAllEnemiesStage.
            // Источник seed недетерминирован намеренно: детерминизм нужен ПОСЛЕ его
            // фиксации, а сам забег должен быть уникальным. Новый seed на каждый вход,
            // включая рестарт (Initialize зовётся на каждую загрузку сцены).
            _inputArgs.Seed = GenerateRunSeed();

            GameplayContextRegistrations.Process(_container, _inputArgs, _sceneContext);

            _container.Initialize();

            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

            PropEntityAuthoring[] sceneProps = levelInstance.GetComponentsInChildren<PropEntityAuthoring>(true);
            IAudioService audioService = _container.Resolve<IAudioService>();

            foreach (PropEntityAuthoring prop in sceneProps)
            {
                prop.Construct(
                        _entitiesLifeContext,
                        audioService,
                        _container.Resolve<DropLootService>(),
                        _container.Resolve<CollidersRegistryService>()
                    );
            }

            // === БАФФЫ: расстановка pre-placed pickup-сфер по уровню ===
            BuffPickupAuthoring[] sceneBuffPickups = levelInstance.GetComponentsInChildren<BuffPickupAuthoring>(true);

            foreach (BuffPickupAuthoring buffPickup in sceneBuffPickups)
            {
                buffPickup.Construct(_entitiesLifeContext, audioService);
            }
            // === КОНЕЦ БЛОКА БАФФОВ ===

            SpawnEnemies(levelInstance);

            if (_sceneContext.FinishPoint == null)
                throw new NullReferenceException("GameplaySceneContext.FinishPoint not assigned in Level Prefab");

            _container.Resolve<FinalPointTriggerService>().Create(_sceneContext.FinishPoint.position);

            yield break;
        }

        // Маркеры читаются напрямую с инстанса уровня, а не из сериализованной копии
        // в GameplaySceneContext. Копия жила отдельной жизнью и обновлялась только
        // ручным SYNC — забыли нажать (или нажатие не доехало до диска, как бывает
        // при правке префаба в Prefab Mode) и рантайм молча спавнил прошлую расстановку.
        // Компонент на объекте — единственный источник истины, рассинхронизировать
        // его больше не с чем.
        private void SpawnEnemies(GameObject levelInstance)
        {
            var enemiesFactory = _container.Resolve<EnemiesFactory>();

            // true — маркеры на выключенных объектах тоже учитываются: дизайнер
            // гасит ветку иерархии, чтобы она не мешала в сцене, но спавн от этого
            // отваливаться не должен.
            EnemySpawnMarker[] markers = levelInstance.GetComponentsInChildren<EnemySpawnMarker>(true);

            var spawnedByConfigType = new Dictionary<string, int>();
            int skippedCount = 0;

            foreach (EnemySpawnMarker marker in markers)
            {
                if (marker.Config == null)
                {
                    // Раньше пустой маркер доезжал до EnemiesFactory.Create и ронял
                    // ArgumentException без единого намёка, ГДЕ именно в иерархии
                    // лежит виноватый объект. Теперь — предупреждение с путём и спавн
                    // остальных врагов продолжается.
                    skippedCount++;

                    Debug.LogWarning(
                        $"[Spawn] Маркер без конфига пропущен: {BuildHierarchyPath(marker.transform)}",
                        marker.gameObject);

                    continue;
                }

                enemiesFactory.Create(marker.transform.position, marker.Config, TryGetPatrolRoute(marker), TryGetLanternAim(marker));

                string configTypeName = marker.Config.GetType().Name;

                spawnedByConfigType.TryGetValue(configTypeName, out int alreadySpawned);
                spawnedByConfigType[configTypeName] = alreadySpawned + 1;
            }

            Debug.Log(BuildSpawnSummary(markers.Length, skippedCount, spawnedByConfigType));
        }

        // Маршрут патруля снимается ЗДЕСЬ, а не в фабрике, по той же причине, по
        // которой здесь же живёт проверка маркера без конфига: только тут на руках
        // есть Transform объекта, а значит и его путь в иерархии для внятного
        // warning'а. Наружу уезжает либо готовый маршрут, либо null — фабрика
        // разбирается с null сама и строит запасной отрезок вокруг спавна.
        //
        // Возврат null во всех проблемных случаях означает, что слайм всё равно
        // заспавнится и будет ходить, просто не там, где задумано. Это осознанно:
        // уронить весь уровень из-за одной незаполненной пустышки хуже, чем
        // отработать с запасным маршрутом и громко об этом сказать.
        private PatrolRoute? TryGetPatrolRoute(EnemySpawnMarker marker)
        {
            if (marker.TryGetComponent(out SlimePatrolRouteAuthoring routeAuthoring) == false)
            {
                return null;
            }

            if (routeAuthoring.PointA == null || routeAuthoring.PointB == null)
            {
                Debug.LogWarning(
                    $"[Spawn] Маршрут патруля задан не полностью, будет использован запасной отрезок " +
                    $"вокруг точки спавна: {BuildHierarchyPath(marker.transform)}",
                    marker.gameObject);

                return null;
            }

            Vector2 pointA = routeAuthoring.PointA.position;
            Vector2 pointB = routeAuthoring.PointB.position;

            if (Vector2.Distance(pointA, pointB) < GetDegenerateRouteThreshold(marker.Config))
            {
                Debug.LogWarning(
                    $"[Spawn] Концы маршрута патруля совпали, будет использован запасной отрезок " +
                    $"вокруг точки спавна: {BuildHierarchyPath(marker.transform)}",
                    marker.gameObject);

                return null;
            }

            return new PatrolRoute(pointA, pointB);
        }

        // Прицел фонаря снимается ЗДЕСЬ, по той же причине, что и маршрут патруля:
        // только тут на руках есть Transform объекта, а значит и путь в иерархии
        // для внятного warning'а. Наружу уезжает либо готовый прицел, либо null —
        // фабрика разбирается с null сама (стреляет вниз из точки спавна).
        //
        // Origin — мировая позиция «дула», Direction — его мировая ось +X
        // (Muzzle.right). Дизайнер вращает дуло, чтобы прицелиться.
        private LanternAimData? TryGetLanternAim(EnemySpawnMarker marker)
        {
            if (marker.TryGetComponent(out LanternAimAuthoring aimAuthoring) == false)
            {
                return null;
            }

            if (aimAuthoring.Muzzle == null)
            {
                Debug.LogWarning(
                    $"[Spawn] Дуло фонаря не задано, снаряд полетит вниз из точки спавна: " +
                    $"{BuildHierarchyPath(marker.transform)}",
                    marker.gameObject);

                return null;
            }

            return new LanternAimData
            {
                Origin = aimAuthoring.Muzzle.position,
                Direction = aimAuthoring.Muzzle.right
            };
        }

        // Порог вырожденности — это и есть радиус достижения точки: маршрут, обе
        // точки которого попадают в один такой радиус, слайм считал бы пройденным,
        // не сдвинувшись с места.
        //
        // Для не-слайма порога нет: маршрут ему всё равно не пригодится, и
        // отбраковывать его незачем.
        private float GetDegenerateRouteThreshold(EntityConfig config)
        {
            if (config is SlimeConfig slimeConfig)
            {
                return slimeConfig.PatrolArriveDistance;
            }

            return 0f;
        }

        // Сводка одной строкой: расхождение задуманной расстановки с фактической
        // видно сразу в консоли, без захода в инспектор и без пересчёта объектов
        // в иерархии руками.
        private string BuildSpawnSummary(int markersCount, int skippedCount, Dictionary<string, int> spawnedByConfigType)
        {
            var summary = new StringBuilder();

            summary.Append($"[Spawn] Маркеров найдено: {markersCount}");

            if (spawnedByConfigType.Count == 0)
            {
                summary.Append(", врагов заспавнено: 0");
            }
            else
            {
                foreach (KeyValuePair<string, int> pair in spawnedByConfigType)
                {
                    summary.Append($", {pair.Key}: {pair.Value}");
                }
            }

            if (skippedCount > 0)
            {
                summary.Append($", пропущено без конфига: {skippedCount}");
            }

            return summary.ToString();
        }

        private string BuildHierarchyPath(Transform target)
        {
            var path = new StringBuilder(target.name);
            Transform current = target.parent;

            while (current != null)
            {
                path.Insert(0, $"{current.name}/");
                current = current.parent;
            }

            return path.ToString();
        }

        // Guid, а не DateTime/TickCount: у тиков грубое разрешение и соседние запуски
        // могли бы получить близкие seed. Хеш Guid распределён равномерно.
        private int GenerateRunSeed() => Guid.NewGuid().GetHashCode();

        public override void Run()
        {
            IAudioService audioService = _container.Resolve<IAudioService>();

            if (_inputArgs.IsRestart == false)
                audioService.PlayPlaylist("Gameplay_Playlist");

            _gameplayStatesContext.Run();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            _entitiesLifeContext?.Update(deltaTime);
            _gameplayStatesContext?.Update(deltaTime);
        }

        // Мозги переехали сюда с Update-канала. Причина: решения AI (запись
        // MoveDirection, проверки расстояний) читаются и исполняются системами,
        // которые живут на fixed — SimpleRigidbodyMovementSystem, PhysicsStabilization.
        // На Update-канале частота решений была привязана к FPS, а применение к
        // физ-шагу, из-за чего при просадке кадров призрак получал несколько
        // физ-шагов на одном устаревшем направлении, а при высоком FPS — наоборот,
        // менял решение чаще, чем оно могло быть применено.
        //
        // Порядок «мозги раньше сущностей» сохранён: brainsContext идёт первой
        // строкой, до entitiesLifeContext.FixedUpdate, ровно как было в Update.
        //
        // ВАЖНО: фазовые таймеры блуждания (TimerService в BrainsFactory) остались
        // на корутинах и продолжают тикать Time.deltaTime на Update-частоте. Это
        // известный технический долг, он этой правкой НЕ устраняется — переходы
        // между фазами блуждания по-прежнему оцениваются относительно кадрового
        // времени, тогда как сама стейт-машина теперь опрашивается на fixed.
        private void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;

            _brainsContext?.Update(fixedDeltaTime);
            _entitiesLifeContext?.FixedUpdate(fixedDeltaTime);
        }
    }
}
