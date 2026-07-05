# NYPD — Кандидаты на удаление мёртвого кода

> Read-only анализ. **Ничего не удалено и не изменено.** Только отчёт.
> Метод: извлечены все объявления типов из `Assets/_Project/Develop` (652 шт.),
> отфильтрованы те, чьё имя встречается только в собственном файле; для
> MonoBehaviour/ScriptableObject дополнительно проверены ссылки по GUID в
> `.prefab`/`.unity`/`.asset`; для plain-классов — отсутствие `new X(`/регистрации
> по всему `Assets`. IEntityComponent-типы, попадающие в сгенерированный
> `EntityAPI.cs`, из «мёртвых» исключены (у них появляется вторая ссылка).

Легенда уверенности:
- **ВЫСОКАЯ** — plain C#-класс, не в DI, не через рефлексию, не через Inspector,
  ноль ссылок в коде и ассетах; есть живая замена или тело мертво.
- **НИЗКАЯ / подтвердить** — либо MonoBehaviour с `[SerializeField]` (мог бы
  вешаться в Inspector, но сейчас не на одном префабе/сцене), либо законченный,
  но нигде не подключённый код, похожий на задел под запланированную фичу.

---

## 1. Можно удалять сразу (ВЫСОКАЯ уверенность)

Redundant / superseded — есть работающая замена, либо тело кода мёртвое.

| Файл | Что именно | Уверенность | Обоснование |
|---|---|---|---|
| `Gameplay/Features/ThrowableFeature/ThrowableBehaviourFactory.cs` | класс `ThrowableBehaviourFactory` + интерфейс `IThrowableBehaviourFactory` | ВЫСОКАЯ | Не зарегистрирован в DI, нигде не инстанцируется (имя — только в своём файле). Живой путь метания — `ProjectileFactory.CreateThrowableProjectile` (вызывается из `InventorySystem`). Второй `Create(config, rb, transform)` — `NotImplementedException` (см. §4). Параллельная заброшенная фабрика. |
| `Gameplay/Features/Entities/Combat/Attack/Shoot/InstantShootSystem.cs` | класс `InstantShootSystem` | ВЫСОКАЯ | Система нигде не добавляется в Entity (`AddSystem`). Всё тело `OnInit` закомментировано; `OnDispose` обратился бы к `null` (`_attackDelayDisposable`). Ссылается на несуществующий `CreateFireballProjectile`. Заброшено. |
| `Utilities/DataManagment/DataRepository/PlayerPrefsDataRepository.cs` | класс `PlayerPrefsDataRepository : IDataRepository` | ВЫСОКАЯ | Альтернативная реализация `IDataRepository`, которую никто не создаёт. Живая — `LocalFileDataRepository` (в `ProjectContextRegistrations.CreateSaveLoadService`). |
| `UI/Gameplay/HealthDisplay/EntitiesHealthDisplay.cs` | класс `EntitiesHealthDisplay : ElementsListView<BarWithText>` | ВЫСОКАЯ | Ноль ссылок в коде, GUID не встречается ни в одном префабе/сцене/ассете. Живой HP-виджет — `PizzaHealthView`. Вытеснено. |
| `UI/UIBackgroundFloat.cs` | класс `UIBackgroundFloat : MonoBehaviour` | ВЫСОКАЯ | GUID не встречается нигде (refFiles=0), имя — только в своём файле. Комментарии в битой кодировке (mojibake) — признак давно брошенного файла. |

**Связанный orphan-эффект при удалении `InstantShootSystem`:** ничего не тянет.
**Связанный orphan-эффект при удалении `ThrowableBehaviourFactory`:** классы
`ShurikenProjectile` / `SleepDartProjectile` остаются вообще без ссылок (сейчас
они упомянуты только в нём) — см. §2, решать вместе.

---

## 2. Нужно подтверждение (НИЗКАЯ уверенность / не трогать без решения)

### 2a. Законченный код без подключения — похоже на задел под фичу

| Файл | Что именно | Почему подтвердить |
|---|---|---|
| `Gameplay/Features/Entities/Combat/Attack/AttackCancelSystem.cs` | класс `AttackCancelSystem` | Полностью реализован, но не добавляется ни в одну Entity. **Его единственные потребители компонентов** `AttackCanceledEvent` и `MustCancelAttack` — только он сам (плюс генерация в `EntityAPI.cs` и объявление в `AttackComponents.cs`). Удаление системы делает эти два компонента полностью осиротевшими. Похоже на незаконченную механику «отмена атаки». |
| `Gameplay/Features/Sensors/AnotherTeamTouchDetectorSystem.cs` | класс `AnotherTeamTouchDetectorSystem` | Система-сенсор, нигде не добавляется. Возможен задел под боевую/AI-логику (рядом живёт таргетинг, который тоже частично отключён). |
| `Gameplay/Features/LootFeature/SecretChestCollectService.cs` | класс `SecretChestCollectService` | Законченный сервис (счётчик секретных сундуков), но **нигде не регистрируется и не создаётся**. ⚠️ `current_state_and_roadmap.md` утверждает, что он «регистрируется как сервис» — по факту нет (расхождение доки↔код). Явный задел под фичу секреток/звезды-3 из `gameplay_design.md`. |
| `Gameplay/Features/StageFeature/StartGameTriggerService.cs` | класс `StartGameTriggerService` | Нигде не используется. Живой аналог — `FinalPointTriggerService`. Возможно, задел под стартовый триггер уровня. |
| `Gameplay/Features/ThrowableFeature/ShurikenProjectile.cs` | класс `ShurikenProjectile : ThrowableProjectile` | Ссылается только закомментированная ветка мёртвого `ThrowableBehaviourFactory`. НО: содержит богаче поведение (втыкание в стену, вращение, дроп лута), чем живой инлайн-путь в `ProjectileFactory`. Конфиг `ShurikenConfig` и `Shuriken.prefab` — **живые**. Решать вместе с судьбой throwable-rework: удалить как redundant или, наоборот, вернуть вместо инлайн-ветки. |
| `Gameplay/Features/ThrowableFeature/SleepDartProjectile.cs` | класс `SleepDartProjectile : ThrowableProjectile` | То же: упомянут только в мёртвом `ThrowableBehaviourFactory`. `SleepDartConfig` и `SleepDart.prefab` — живые (метаются generic-путём `ProjectileFactory`). Behavior-класс — redundant, но это половина запланированной механики «усыпляющий дротик». |

### 2b. MonoBehaviour без единого размещения (мог бы вешаться в Inspector)

Все — `refFiles=0`: GUID не встречается ни в одном `.prefab`/`.unity`/`.asset`.
Формально мертвы сейчас, но по природе рассчитаны на ручную установку в редакторе,
поэтому — подтвердить, не удалять вслепую.

| Файл | Что именно | Обоснование / примечание |
|---|---|---|
| `Configs/Gameplay/Context/ChestSpawnMarker.cs` | `ChestSpawnMarker : MonoBehaviour` | Маркер спавна сундука, не стоит ни на одном уровне. Связан с WIP-фичей секреток (`SecretChestCollectService`). |
| `Gameplay/Features/LifeCycle/HealthBarPointRegistrator.cs` | `HealthBarPointRegistrator : MonoEntityRegistrator` | `Register()` — тело закомментировано (`// entity.AddHealthBarPoint(_point);`), т.е. no-op, и не стоит ни на одном префабе. Связан с вытесненным `EntitiesHealthDisplay`. |
| `Gameplay/Features/Entities/Combat/Attack/CurrentTargetView.cs` | `CurrentTargetView : EntityView` | Визуал текущей цели; не на одном префабе. Связан с частично отключённым авто-таргетингом/авто-атакой (см. `BrainsFactory`, закомментированный auto-attack). |
| `UI/CommonViews/ConstantRotator.cs` | `ConstantRotator : MonoBehaviour` | `[SerializeField]`, комментарий «Сюрикен должен крутиться быстро!». Не на одном префабе (в т.ч. не на `Shuriken.prefab`). |
| `UI/MainMenu/ExtrasView.cs` | `ExtrasView : MonoBehaviour, IView` | Экран «Extras» главного меню, не на сцене/префабе `MainMenu`. |
| `UI/SafeAreaContainer.cs` | `SafeAreaContainer : MonoBehaviour` | Safe-area для мобильного вывода. Не размещён нигде — вероятно, резерв под PS/мобильный порт. Подтвердить намерение. |

> Прочие однофайловые MonoBehaviour из первичного среза (`DashView`, `JumpView`,
> `AttackView`, `PizzaHealthView`, `Trampoline`, `BodyColliderRegistrator`,
> `TransformEntityRegistrator`, `MainMenuBootstrap`, `GameplayBootstrap` и т.д.)
> **НЕ мёртвые** — их GUID реально встречается в префабах/сценах (refFiles≥1).
> Одиночность имени в `.cs` у них ожидаема: они цепляются рантайм-поиском
> `GetComponentsInChildren<EntityView/MonoEntityRegistrator>()` или лежат на сцене.

---

## 3. Осознанные заглушки — НЕ ТРОГАТЬ

Помечены `REWORK`/комментарием, объясняющим намерение, или явно «отложенная фича».

| Файл | Что именно | Пометка |
|---|---|---|
| `Gameplay/EntitiesCore/EntitiesFactory.cs:679` | `CreateSlime(...)` — тело целиком в `/* ... */`, `return null` | Блок обрамлён `// REWORK`. Слайм-враг: конфиг+арт+`Slime.prefab` есть, код осознанно заморожен. Не удалять. |
| `Gameplay/Features/InventoryFeature/InventorySystem.cs:105` | `ApplyInternalEffect` — эффект зелья закомментирован, вместо него `Debug.Log` | Заглушка под интеграцию с `BuffService`. Намеренная. |
| `Gameplay/Features/StageFeature/ClearAllEnemiesStage.cs:88` | Блок подписки на смерть врагов закомментирован | ⚠️ Класс **живой** (создаётся в `StagesFactory`, есть `.asset`), но из-за закомментированного трекинга `_spawnedEnemiesToRemoveReason` стадия завершается мгновенно (логический недочёт, не мёртвый код). Не удалять класс — это баг-заглушка. |
| `Gameplay/Features/ThrowableFeature/ShurikenProjectile.cs:49,66` | Нанесение урона (`DamageData`/`DamageType.Cut`) и дроп лута с пропсов закомментированы | Задел под систему типов урона/дропа. (Сам класс — см. §2a.) |
| `Gameplay/Features/Entities/Gadgets/Grapple/GrappleSystem.cs:60` | Комментарий `// CHANGED: убрана мёртвая else-ветка` | Осознанная зачистка автором; трогать нечего. |
| `Gameplay/Features/Entities/MovementFeature/Plunge/PlungeView.cs:22` | Поле с `[Tooltip("Устарело: теперь всё зависит от физической скорости")]` | Сериализованное поле помечено устаревшим, но остаётся для Inspector-совместимости. Подтвердить перед удалением поля. |

---

## 4. NotImplementedException-заглушки (перечень, НЕ удалять и НЕ реализовывать)

| Файл:строка | Сигнатура | Есть ли вызовы |
|---|---|---|
| `Gameplay/EntitiesCore/Entity.cs:127` | `internal object AddWallMask(object wallMask)` | Вызовов не найдено. (Живой путь — сгенерированный `AddWallMask(LayerMask)` в `EntityAPI.cs`; эта ручная перегрузка-дубликат мертва.) |
| `Utilities/Reactive/ReactiveEvent.cs:79` | `internal IDisposable Invoke(object onEndAttack)` | Вызовов не найдено (только объявление). Имя параметра `onEndAttack` — leftover. |
| `UI/Core/ViewsFactory.cs:72` | `internal T Create<T>(object hintView, Transform popupLayer)` | Вызовов не найдено. Живая перегрузка — `Create<TView>(string viewID, Transform)`. |
| `Gameplay/Features/ThrowableFeature/ThrowableBehaviourFactory.cs:41` | `object Create(ThrowableItemConfig, Rigidbody2D, Transform)` | Вызовов не найдено. Весь класс — кандидат из §1. |

---

## 5. Закомментированные блоки кода (inline, внутри живых файлов)

Не отдельные файлы, а мёртвые фрагменты внутри рабочего кода. Кандидаты на
локальную зачистку (после подтверждения намерения по связанной фиче):

| Файл:строка | Фрагмент |
|---|---|
| `ThrowableFeature/ThrowableBehaviourFactory.cs:29` | Ветка `ShurikenConfig => new ShurikenProjectile(...)` закомментирована |
| `ThrowableFeature/ShurikenProjectile.cs:49` | Блок `target.TakeDamageRequest.Invoke(new DamageData{...})` |
| `Attack/Shoot/InstantShootSystem.cs:28,46` | Всё тело `OnInit` + тело `OnAttackDelayEnd` |
| `StageFeature/ClearAllEnemiesStage.cs:88` | Подписка на смерть врагов (трекинг завершения стадии) |
| `AI/BrainsFactory.cs:33,94` | `CreateAutoAttackStateMachine` + его подключение в hero-brain закомментированы |
| `InventoryFeature/InventorySystem.cs:107` | `AddSpeedBuffModifier`/`AddBuffDuration` |

---

## 6. Прочие заметки

- **Неиспользуемые `using`**: точечно присутствуют в брошенных файлах
  (напр. `InstantShootSystem.cs`, `ShurikenProjectile.cs`, `SecretChestCollectService.cs`
  тянет `System.Linq/Text/Threading.Tasks` без нужды). Массовый разбор `using`
  не проводил — это шум низкой ценности, вычищается IDE/`dotnet format` за один
  проход; отдельными кандидатами на удаление не выношу.
- **Осиротевшие ассеты**:
  - `Resources/Entities/Enemies/Slime.prefab` — сейчас недостижим в рантайме
    (грузится только из `CreateSlime`, а тот в `/* REWORK */` → `null`). Не
    удалять — часть замороженной фичи слайма (§3).
  - `Shuriken.prefab` / `SleepDart.prefab` — **живые** (грузятся `ProjectileFactory`
    по `PrefabPath` из `ShurikenConfig`/`SleepDartConfig`, которые назначены в
    `MainHeroConfig.asset`). НЕ орфаны.
  - `Configs/Gameplay/Stages/ClearAllEnemiesStage.asset` — живой (класс создаётся
    в `StagesFactory`).
  - Полный аудит осиротевших `.asset`/`.prefab` по GUID не делал (конфиги
    грузятся ещё и по строковым путям через `ResourcesConfigsLoader`, поэтому
    отсутствие GUID-ссылки ≠ orphan). Требует отдельного прохода — вынести в
    следующий шаг при желании.
- **Doc-drift, попутно**: `SecretChestCollectService` в
  `current_state_and_roadmap.md` заявлен как зарегистрированный сервис — по факту
  не зарегистрирован (см. §2a).

---

### Итог по приоритету
- **Безопасно удалить сейчас (§1):** `ThrowableBehaviourFactory`(+интерфейс),
  `InstantShootSystem`, `PlayerPrefsDataRepository`, `EntitiesHealthDisplay`,
  `UIBackgroundFloat`.
- **Спросить решение (§2):** судьба throwable-rework (`Shuriken/SleepDartProjectile`),
  фичи секреток (`SecretChestCollectService`, `ChestSpawnMarker`), отмены атаки
  (`AttackCancelSystem` + компоненты `AttackCanceledEvent`/`MustCancelAttack`),
  `AnotherTeamTouchDetectorSystem`, `StartGameTriggerService`, mobile safe-area,
  остальные zero-ref MonoBehaviour.
- **Не трогать (§3, §4):** REWORK-заглушки и `NotImplementedException`-перегрузки.
