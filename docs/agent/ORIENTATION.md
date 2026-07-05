# NYPD — ORIENTATION (снимок кода)

> Составлено в режиме read-only разведки. Источник истины — код; доки сверены и
> расхождения выписаны явно. Unity **6000.4.11f1**, платформа StandaloneWindows64.
> Активная сцена на момент разбора — `Assets/_Project/Scenes/Gameplay.unity`.
> Unity MCP отвечает (1 инстанс, консоль без ошибок — только warning про слишком
> длинное имя файла в `Art/SFX/Weapons/Catana`).

---

## 0. Проверка связи
- MCP доступен, `editor_state.ready_for_tools = true`, `is_compiling = false`.
- Консоль чистая: единственная запись — AssetDatabase warning про имя файла
  катаны длиннее 250 байт (файл игнорируется импортёром). На код не влияет.

---

## 1. Карта кодовой базы

Весь игровой код — под `Assets/_Project/Develop/` (Runtime + Editor). Ассеты —
`Assets/_Project/Art/`, конфиги (ScriptableObject) — `Develop/Runtime/Configs/`.
Корневой неймспейс — `Assets._Project.Develop.*` (да, с `Assets.` в префиксе).

Сцены (`Assets/_Project/Scenes/`): `Init`, `MainMenu`, `Gameplay`, `Tutorial`, `Empty`.

| Папка (Develop/Runtime) | Зона ответственности |
|---|---|
| `Infrastructure/DI` | Самописный DI: `DIContainer`, `Registration`, `IInitializable` |
| `Infrastructure/EntryPoint` | `GameEntryPoint` (Awake-бутстрап) + `ProjectContextRegistrations` (project-scope) |
| `Gameplay/Infrastructure` | `GameplayBootstrap` (главный Update-луп) + `GameplayContextRegistrations` (scene-scope) |
| `Gameplay/States` | Геймплейная стейт-машина уровня (Intro→Scouting→Process, Win/Defeat) |
| `Gameplay/EntitiesCore` | ECS-lite: `Entity`, `EntitiesLifeContext`, `EntitiesFactory`, `MonoEntity*`, `CollidersRegistryService`, `Generated/EntityAPI.cs` |
| `Gameplay/Features/Entities/MovementFeature` | Move/Jump/AirJump/Dash/Slide/Slope/Plunge/HangWall — системы передвижения героя |
| `Gameplay/Features/Entities/Combat` | Attack (melee, charged slash, cooldowns, i-frames, double-attack), ApplyDamage, Contact, HitImpact |
| `Gameplay/Features/Entities/Gadgets` | Glider (глайд), Grapple (крюк-кошка) |
| `Gameplay/Features/MainHero` | `MainHeroFactory`, holder, стиль-система героя, HUD-view (pizza HP, lives) |
| `Gameplay/Features/Enemies` | `EnemiesFactory` (сейчас только Ghost) |
| `Gameplay/Features/AI` | `StateMachineBrain`, `AIStateMachine`, `AIParallelState`, `BrainsFactory`, `AIBrainsContext`, таргетинг + states |
| `Gameplay/Features/StyleFeature` | DMC-ранк-система: `RankStyleService`, `StyleEvaluator`, конфиги рангов/действий |
| `Gameplay/Features/BuffsFeature` + `LevelObjects/Buffs` | Система баффов + pre-placed pickup-сферы (arc/magnet/distance-collect) |
| `Gameplay/Features/LootFeature` | `LootFactory`, `DropLootService`, `SessionLootService`, drop-системы |
| `Gameplay/Features/StageFeature` | Стадии уровня, `StageProviderService`, финиш-триггер |
| `Gameplay/Features/LevelResultsFeature` | `LevelResultService` (сводит ранк + лут в результат уровня) |
| `Gameplay/Features/LifeCycle` | Spawn/Death процессы, self-release, отключение коллайдеров на смерти |
| `Gameplay/Features/InGameTimers` / `Features/HitStop` / `Features/CameraFeature` | Таймер уровня, hit-stop (time-scale), камеры (Cinemachine) |
| `Gameplay/Features/InteractiveObjects` | `Trampoline` (баунс — vanilla MonoBehaviour) |
| `Gameplay/Features/Projectiles` / `ThrowableFeature` | Сюрикены/дротики, фабрика проджектайлов |
| `Meta/Features` | `Wallet` (2 валюты), `Stats`, `LevelsProgression` |
| `UI/Core`, `UI/Gameplay`, `UI/MainMenu` | MVP: попапы (настройки/confirm), HUD (HP/стиль/таймер/баффы/инвентарь), меню |
| `Utilities/*` | Reactive, Conditions, StateMachineCore, DataManagment (save/load JSON), Configs, Coroutines, Audio, Assets, Timer |
| `Develop/Editor` | `EntityAPIGenerator`, `LayersAPIGenerator`, `EntryPointSceneAutoLoader`, `GameplaySceneContextEditor` |

---

## 2. Ключевые системы — фактическое устройство

### 2.1 Точки входа и флоу
- **`GameEntryPoint.Awake()`** (сцена `Init`): создаёт project-`DIContainer`,
  прогоняет `ProjectContextRegistrations.Process` → `container.Initialize()`,
  затем корутиной грузит конфиги, player-data (load или `Reset`) и через
  `SceneSwitcherService` уходит в `MainMenu`. Ставит `vSyncCount=0`,
  `targetFrameRate=60`.
- **`GameplayBootstrap : SceneBootstrap`** (сцена `Gameplay`): в `Initialize()`
  берёт `LevelConfig` по `LevelNumber`, инстанцирует `LevelPrefab` в объект с
  тегом `LevelHolder`, достаёт `GameplaySceneContext` из префаба, регистрирует
  scene-scope (`GameplayContextRegistrations`), затем конструирует props/buff-
  pickups/врагов/финиш-триггер. Иерархия сцены: `SceneBootstrap` (bootstrap),
  `LevelHolder` (пустой, наполняется в рантайме), `Main Camera` (CinemachineBrain).
- **Главный луп — `GameplayBootstrap.Update()`** тикает по порядку:
  `_brainsContext.Update(dt)` → `_entitiesLifeContext.Update(dt)` →
  `_gameplayStatesContext.Update(dt)`. **`Time.deltaTime`, т.е. переменный
  таймстеп** (не FixedUpdate).

### 2.2 DI-контейнер (`Infrastructure/DI`)
- Иерархический: `DIContainer(parent)` с fallback в `_parent` для `Resolve`/
  `IsAlreadyRegister`. Project-scope → gameplay scene-scope.
- Только singleton (`Registration` кэширует `_cachedInstance`). Lazy по умолчанию,
  `.NonLazy()` помечает eager.
- Cycle-detection в `Resolve<T>()` через список `_requests` (кидает
  `InvalidOperationException`, а не StackOverflow).
- **`Initialize()`**: по всем регистрациям — если `IsNonLazy`, создаёт инстанс;
  затем зовёт `OnInitialize()`, который вызывает `IInitializable.Initialize()`
  **только если инстанс уже создан** (`_cachedInstance != null`). Значит
  lazy-сервис, ещё не резолвнутый, `Initialize()` не получает. Гэп из
  tech_architecture.md **подтверждён фактически**.
- Массовое создание сущностей — через `*Factory` (Entities/Enemies/MainHero/
  Projectile/Loot/Stages/Brains), не через DI lifetime.

### 2.3 ECS-lite (`EntitiesCore`)
- **`Entity`** (`partial`, вторая часть — сгенерированный `Generated/EntityAPI.cs`):
  `Dictionary<Type, IEntityComponent>` + собственные списки
  `_systems/_initializables/_updatables/_disposables`. `AddComponent/TryGet/Get/
  Has`, `AddSystem` (роутит систему по интерфейсам `IInitializable/IUpdatable/
  IDisposableSystem`; если entity уже инициализирована — сразу зовёт `OnInit`).
- **`EntitiesLifeContext`**: список entity, `Update` — обратный проход с
  try/catch (сломанная entity логируется и вычищается, не вешает уровень),
  затем обрабатывает `_releaseRequests`. `Add` сразу зовёт `entity.Initialize()`.
- **`EntityAPIGenerator`** (Editor): reflection-генератор fluent-API
  (`AddX/GetX/TryGetX`) по всем `IEntityComponent`. Висит на **`[InitializeOnLoadMethod]`
  И `[MenuItem("Tools/GenerateEntityAPI")]` одновременно**, в коде комментарий
  `// может багать`. Риск из доков **на месте**.
- **Mono-мост**: `MonoEntity` (`LinkedEntity`, `Link/Cleanup` — цепляет
  `EntityView`, `MonoEntityRegistrator`, регистрирует коллайдеры в
  `CollidersRegistryService`), `MonoEntitiesFactory`, `EntityView`.

### 2.4 Реактивные примитивы (`Utilities/Reactive`)
- `ReactiveVariable<T>`, `ReactiveEvent<T>`, `ReactiveEvent`, `IReadOnlyVariable/
  Event`, `Subscriber`. Equality-check перед инвоком, buffered add/remove
  подписчиков. `Conditions` (`ICompositeCondition/FuncCondition/CompositeCondition`)
  — декларативные guard'ы, массово используются в фабриках (`CanDash`, `CanJump`,
  `CanGlide`, `CanApplyDamage` и т.д.).

### 2.5 State machines (`Utilities/StateMachineCore` + `Gameplay/States`)
- Дженерик `StateMachine<TState> : State` — вложенные машины через `StateNode`/
  `StateTransition`/`ICondition`. `Dispose()` при `_isRunning` зовёт
  `CurrentState.Exit()`, диспозит стейты и `_disposables` — **правило #4 из
  CLAUDE.md соблюдено**.
- **Геймплейный флоу** (`GameplayStatesFactory`): корневая `gameplayCycle` =
  { `coreLoop`, `WinState`, `DefeatState` }, где `coreLoop` = { `LevelIntroState`
  (катсцена/диалог, скип при рестарте) → `LevelScoutingState` (свободная камера,
  ждёт подтверждения) → `LevelProcessState` (спавн героя, таймер, стиль) }.
  Переходы: coreLoop→Win при `StageResults.Completed`; coreLoop→Defeat при
  `MainHero.IsDead && !InDeathProcess`.

### 2.6 Герой (`EntitiesFactory.CreateHero` + системы)
Самая нагруженная сущность. Компоненты/условия/системы собираются в 3 метода
(`AddHeroComponents/Conditions/Systems`). Реализовано по факту:
- **Движение**: `RigidbodyMovementSystem`, `JumpSystem`, `AirJumpSystem` +
  `AirJumpsRecoverySystem`, `WallJumpSystem`, `DashSystem`, slope-триада
  (`SlopeSlip/SlopeSlide/SlopeJump`), `SlideSystem`, `GlideSystem`,
  `PlungeSystem` + `PlungeDamageOnImpactSystem`, `WallHangSystem`,
  `GrappleSystem`. `SurfaceCheckSystem` — граунд/стены.
- **Бой**: `StartAttack`→`AttackProcessTimer`→`AttackDelayEndTrigger`→
  `MeleeAttackHit`→`EndAttack`→`AttackCooldownTimer`, `AttackInvulnerability`
  (i-frames), `HitStopSystem` (через `HitStopService`+`CameraService`),
  `DoubleAttack(+Cooldown)`, заряженный slash (`SlashAttackCharge`+`SlashAttackSpawn`),
  `AerialHitSuspensionSystem` (джаггл), `ApplyDamage(+Cooldown)`.
- **Скорость = урон** (design pillar): `BodyContactDetectingSystem` +
  `BodyContactsEntitiesFilterSystem` + `LethalContactMovementSystem` —
  реализовано.
- **Инвентарь**: `InventorySystem` (переключение колесом, использование
  throwable/potion; potion-эффект — заглушка `Debug.Log`, применение бафа
  закомментировано).
- **Lifecycle**: `SpawnProcessTimer`, `DeathSystem`, `DeathProcessTimer`,
  `SelfReleaseSystem` (всегда последней).

**Таймстеп/корутины (проверка по факту):**
- `DashSystem`/`SlideSystem`: буфер ввода, заряд и кулдаун — уже на
  frame-tick float-таймерах в `OnUpdate` (используют `Time.unscaledDeltaTime`
  для кулдауна — совместимо с hit-stop). НО **само окно движения
  (`DashCoroutine`/`SlideCoroutine`) всё ещё корутина** через `ICoroutinesPerformer`.
- `InventorySystem.ResetUsingFlag` — корутина `WaitForSeconds(0.15f)`.
- `GrappleSystem` тоже получает `ICoroutinesPerformer`.
- Итог: миграция на reactive-таймеры **частичная**; корутинные тайминги в
  Dash/Slide/Inventory/Grapple присутствуют. Диагноз доков в силе.

### 2.7 Враги + AI
- **`EnemiesFactory.Create`**: `switch` по конфигу — **реализован только `GhostConfig`**;
  любой другой тип → `throw ArgumentException`. Призрак: здоровье, контактный
  урон, knockback (`DamageKnockback*`), смерть, self-release, дроп лута,
  `IsGrappledTarget`, brain.
- **Слайм**: `EntitiesFactory.CreateSlime` целиком закомментирован, `return null`,
  блок помечен `REWORK`. В `EnemiesFactory.Create` кейса Slime **нет вовсе**
  (даже null не дойдёт). `SlimeConfig.cs` существует, арт слайма есть
  (`Art/Sprites/.../SLIME`, анимации, VFX). Т.е. слайм — только конфиг+арт,
  кода нет.
- **AI** (`BrainsFactory`, `AIBrainsContext.Update` в главном лупе):
  `StateMachineBrain` поверх `AIStateMachine`/`AIParallelState`.
  - Ghost-brain: `RandomMovementState`(2с) ↔ `EmptyState`(3с) по таймерам.
  - Hero-brain: `AIParallelState(RotateToTarget, idle)`; авто-атака
    (`CreateAutoAttackStateMachine`, `AttackTriggerState`) **закомментирована**.
  - Есть таргетинг: `TargetingCoreSystem`, `NearestDamagableTargetSelector`,
    `FindTargetState`, `RotateToTargetState`, `AttackTriggerState`.

### 2.8 Стиль/ранк-система (`StyleFeature`) — реализована
- **`RankStyleService`**: очки, буквенный ранг (F..) + суб-ранги с
  порогами/множителями/префиксами, **decay по idle-времени** (grace-delay,
  warning-прогресс, `IsDecaying`), `ApplyDamagePenalty` (сброс на N суб-рангов
  вниз при получении урона), accent-цвета рангов. Всё через `ReactiveVariable`
  для HUD.
- **`StyleEvaluator`**: `ProcessHit/Dash/WallJump/WallHangAttach/GrappleAttach/
  PlungeSlam/PlayerHit`. **Diversity-множитель**: повтор экшена в истории/на
  кулдауне режет очки, новый тип — бонус (`DiversityMultiplier`). Это ровно
  идея «чейн из разных архетипов» из enemy_design.md, уже в коде.
- HUD: `UI/Gameplay/StyleDisplay`; на герое — `MainHeroStyleSystem`.

### 2.9 Баффы (`BuffsFeature` + `LevelObjects/Buffs`) — отдельная система
- `BuffService.Pickup(hero, config)`: стак по `Id` (`Extend` продлевает),
  иначе `config.CreateEffect().Apply(hero)` + запись в `ActiveBuffsList` на
  entity. `BuffsTimerSystem` тикает длительности. Эффекты:
  `MoveSpeedMultiplierBuffEffect`, `LootCollectRangeAdditiveBuffEffect`
  (+ соответствующие `*BuffConfig`).
- Pre-placed pickup-сферы на уровне: `BuffPickupAuthoring.Construct(...)` в
  бутстрапе; системы `BuffArcMovement`, `BuffMagnet`, `BuffDistanceCollect`,
  `BuffVisualsView`. HUD активных баффов — `UI/Gameplay/Buffs`.

### 2.10 Meta / данные / UI
- **Кошелёк**: `WalletService` (`NonLazy`) — `Dictionary<CurrencyTypes,
  ReactiveVariable<int>>` по всем валютам enum'а → дуальная экономика готова
  инфраструктурно. `WalletPresenter`/`CurrencyPresenter` — образец reactive+MVP.
- **Save/Load**: `SaveLoadService` (JSON), `LocalFileDataRepository`; в редакторе
  пишет в `Application.dataPath`, в билде — `persistentDataPath`.
  `PlayerDataProvider`, `LevelsProgressionService`, `GameStatsService`.
- **UI/MVP**: попапы через `PopupViewBase`/`PopupPresenterBase` + `PopupService`
  (`MainMenu`/`Gameplay` наследники) + DOTween. `ViewsFactory` создаёт View по
  string-ID. HUD геймплея — HP (pizza), стиль, таймеры, баффы, инвентарь, хинты.
- **Главное меню**: `Dojo/Shop/Leaderboard` — `View : MonoBehaviour, IView`,
  открываются **через `_view.X.gameObject.SetActive(true/false)`** в
  `MainMenuScreenPresenter`, без собственных Presenter/Service/анимаций.
  Расхождение из tech_architecture.md **в силе**.

---

## 3. Дельта «доки vs реальность»

| # | Доки говорят | Факт в коде | Тип |
|---|---|---|---|
| 1 | Дизайн-доки в `docs/design/` (CLAUDE.md) | Лежат прямо в `docs/`, папки `design/` нет | Путь в CLAUDE.md неточен |
| 2 | Главный луп на переменном таймстепе | **Подтверждено**: `GameplayBootstrap.Update()` → `Time.deltaTime`; тем же лупом тикает и `AIBrainsContext` | Совпадает |
| 3 | Coroutine-таймеры в Dash/Slide/Inventory | **Частично**: заряд/буфер/кулдаун Dash/Slide уже frame-tick float-таймеры; окна движения (`Dash/SlideCoroutine`) и `Inventory.ResetUsingFlag` + Grapple — всё ещё корутины | Уточнение |
| 4 | Слайм не реализован (фабрика возвращает `null`, REWORK) | **Хуже**: `CreateSlime` возвращает null и закомментирован, а в `EnemiesFactory.Create` кейса Slime нет вообще — спавн кинет `ArgumentException` | Совпадает + деталь |
| 5 | Баунс-механики (Slime bounce) в коде нет | **Есть, но иначе**: баунс реализован как `Trampoline` — интерактивный объект (vanilla `MonoBehaviour` + DOTween + физ-импульс), не как враг-слайм | Расхождение |
| 6 | Стиль/ранк: «степень готовности уточнить» | **Реализована целиком**: ранги/суб-ранги, decay, damage-penalty, diversity-множитель за разные экшены; HUD на reactive | Готовее, чем в доках |
| 7 | (В доках отсутствует) | **Система баффов** (`BuffsFeature`, эффекты, pickup-сферы arc/magnet/distance, таймеры, HUD) — целая фича, не описана нигде | Не документировано |
| 8 | (current_state упоминает только «grappled target» у призрака) | **AI-слой**: `StateMachineBrain`/`AIStateMachine`/`AIParallelState`/таргетинг/states; ghost-brain работает, hero auto-attack закомментирован | Не документировано |
| 9 | Три `NotImplementedException`-заглушки | **Четыре**: `Entity.AddWallMask`, `ReactiveEvent.Invoke(object)` (:81), `ViewsFactory.Create(object,Transform)` (:74) **+ новая** `ThrowableBehaviourFactory` (:43) | Стало больше |
| 10 | `EntityAPIGenerator` на `[InitializeOnLoadMethod]` — риск | **Подтверждено**: атрибут на месте рядом с `[MenuItem]`, комментарий `// может багать` | Совпадает |
| 11 | Dojo/Shop/Leaderboard через `SetActive`, не Presenter | **Подтверждено** в `MainMenuScreenPresenter` | Совпадает |
| 12 | DI: `Initialize()` не зовётся для lazy | **Подтверждено**: `Registration.OnInitialize` зовёт `IInitializable.Initialize()` только если инстанс уже создан | Совпадает |

---

## 4. Открытые вопросы (нужен твой ответ, не догадка)

1. **Таймстеп — целевое состояние.** Оставляем ли осознанно переменный
   `Update()`-луп (тогда ghost-replay/детерминизм из gameplay_design.md
   откладываются), или планируется перевод на fixed-step? Это влияет на то,
   стоит ли вообще дочищать корутины в Dash/Slide/Inventory/Grapple сейчас.
2. **Слайм.** REWORK-заглушка — это «переписать под новую механику» или
   «удалить как устаревшее»? Баунс уже закрыт `Trampoline`; нужен ли слайм-враг
   отдельно, и если да — как bounce-архетип (дублирует трамплин) или как обычный
   melee-fodder с лутом?
3. **`Trampoline` как vanilla-MonoBehaviour.** Осознанное исключение из
   DI/ECS-архитектуры (CLAUDE.md против vanilla-подхода без запроса), или его
   надо привести к Entity/системному виду? Не трогал.
4. **`ThrowableBehaviourFactory` (:43 NotImplementedException).** Это активный
   путь кода (throwable-инвентарь) или мёртвая ветка? Если активный — какой
   throwable-тип должен туда попадать (сейчас кинет исключение при попытке).
5. **Hero auto-attack AI (закомментирован в `BrainsFactory`).** Это выпиленная
   фича или задел «на потом»? Влияет на то, считать ли таргетинг-системы
   (`TargetingCoreSystem`, селекторы) живым кодом или dead-code.
6. **Potion-эффекты в инвентаре.** `ApplyInternalEffect` — заглушка `Debug.Log`,
   применение бафа закомментировано. Зелья должны идти через готовый `BuffService`
   (естественная связка) или планировался отдельный путь?
7. **`Empty`/`Tutorial` сцены.** `Tutorial.unity` существует — это рабочая
   ветка туториала из lore/gameplay-доков или черновик? В флоу (`GameEntryPoint`
   → `MainMenu`) я её входа не увидел; откуда она грузится?
