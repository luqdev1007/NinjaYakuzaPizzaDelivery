# NYPD — Отчёт об удалении мёртвого кода

> Выполнено по `CLEANUP_CANDIDATES.md`, все пункты подтверждены владельцем.
> Каждый пункт перед удалением заново проверен на ноль-ссылок (код + GUID
> скрипта в `.prefab`/`.unity`/`.asset`). После каждого логического блока —
> компиляция Unity проверялась через MCP-консоль: **0 ошибок** на всех шагах.
> Рабочее дерево чистое. Ничего не пропущено — живых ссылок, отсутствовавших в
> исходном отчёте, не обнаружено ни по одному пункту.

## Коммиты (checkpoint → cleanup)
```
cd0dd24e pre-cleanup snapshot                (чекпоинт для отката)
384d04a2 cleanup: throwable-behaviour cluster
a6c13b0e cleanup: standalone dead classes
f56289b9 cleanup: secret-chest scaffolding
ec20c962 cleanup: AttackCancelSystem + orphaned components
d35c3806 cleanup: sensor system + start-trigger service
6c3f0574 cleanup: mobile SafeAreaContainer
```
Откат всего: `git revert 6c3f0574..HEAD` (или пофайлово по коммитам выше).

---

## Что удалено (по пунктам)

| # | Пункт | Удалённые файлы (`.cs` + `.meta`) | Коммит |
|---|---|---|---|
| 1 | ThrowableBehaviourFactory + интерфейс | `ThrowableFeature/ThrowableBehaviourFactory.cs`, `ThrowableFeature/IThrowableBehaviourFactory.cs` | 384d04a2 |
| 6 | Throwable behavior-классы | `ThrowableFeature/ShurikenProjectile.cs`, `ThrowableFeature/SleepDartProjectile.cs` | 384d04a2 |
| 2 | InstantShootSystem | `Combat/Attack/Shoot/InstantShootSystem.cs` | a6c13b0e |
| 3 | PlayerPrefsDataRepository | `Utilities/DataManagment/DataRepository/PlayerPrefsDataRepository.cs` | a6c13b0e |
| 4 | EntitiesHealthDisplay | `UI/Gameplay/HealthDisplay/EntitiesHealthDisplay.cs` | a6c13b0e |
| 5 | UIBackgroundFloat | `UI/UIBackgroundFloat.cs` | a6c13b0e |
| 7 | SecretChestCollectService, ChestSpawnMarker | `LootFeature/SecretChestCollectService.cs`, `Configs/Gameplay/Context/ChestSpawnMarker.cs` | f56289b9 |
| 8 | AttackCancelSystem + компоненты | `Combat/Attack/AttackCancelSystem.cs` + правка `AttackComponents.cs` (удалены классы `MustCancelAttack`, `AttackCanceledEvent`) + правка `Generated/EntityAPI.cs` (удалены их блоки) | ec20c962 |
| 9 | AnotherTeamTouchDetectorSystem | `Sensors/AnotherTeamTouchDetectorSystem.cs` | d35c3806 |
| 10 | StartGameTriggerService | `StageFeature/StartGameTriggerService.cs` | d35c3806 |
| 11 | SafeAreaContainer (mobile) | `UI/SafeAreaContainer.cs` | 6c3f0574 |

**Итого:** удалено 14 `.cs` (+ их `.meta`); изменено 2 файла (`EntityAPI.cs`,
`AttackComponents.cs`) в рамках синхронного удаления компонентов п.8.

### Про throwable (п.6) — проверка на GUID-ссылку префабов
`ShurikenProjectile`/`SleepDartProjectile` — plain C#-классы (не MonoBehaviour),
компонентами на префабах быть не могут; GUID их скриптов в `.prefab`/`.unity`/
`.asset` **не встречается**. `Shuriken.prefab`/`SleepDart.prefab` и конфиги
`ShurikenConfig`/`SleepDartConfig` не тронуты (живой путь — `ProjectileFactory`
по `PrefabPath`). Живых ссылок именно на behavior-классы не найдено — удаление
безопасно, останавливаться не пришлось.

### Про AttackCancel (п.8) — синхронность генерённого API
`EntityAPI.cs` генерируется рефлексией по `IEntityComponent`. Удаление классов
`MustCancelAttack`/`AttackCanceledEvent` без правки API дало бы ошибку
компиляции, поэтому три части (система, классы компонентов, генерённые блоки
API) удалены одним коммитом. После — `grep` по трём именам пуст, компиляция
чистая.

---

## Пропущено из-за живых ссылок
**Ничего.** Все 11 пунктов имели подтверждённые ноль-ссылок; неожиданных живых
ссылок не найдено, остановок по инерции не было.

---

## Дополнительно осиротело (НЕ удалено — для следующего прохода)

Освободилось в результате этой чистки либо найдено попутно. Оставлено как есть,
т.к. вне подтверждённого скоупа. Кандидаты на отдельное подтверждение:

| Объект | Файл | Почему осиротело / статус |
|---|---|---|
| Компонент `IsTouchAnotherTeam` | `Sensors/SensorsComponents.cs:43` (+ генерённый `EntityAPI.cs`) | **Освобождён удалением п.9.** Единственным потребителем был удалённый `AnotherTeamTouchDetectorSystem`. Теперь — только объявление + генерённый API, ни одна фабрика его не добавляет. Полный аналог кейса п.8: удалять надо синхронно классом + блоком в `EntityAPI.cs`. |
| Компоненты `ThrowEvent`, `ThrowRequest` | `ThrowableFeature/ThrowableComponents.cs` (+ генерённый `EntityAPI.cs`) | **Пред-существующие орфаны** (не следствие чистки). Ссылок из игрового кода нет — только генерённый API. Ни к одной Entity не добавляются. Кандидаты на удаление тем же паттерном (класс + блок API). |
| Поле `SecretChestLoot` | `Configs/Gameplay/Loot/MasterLootProviderConfig.cs:9` | Относится к удалённой фиче секреток (п.7). Это **live сериализованное поле** живого конфига (`[field: SerializeField]`), может держать данные в `.asset` — Inspector-зависимое, требует ручного решения, не текстового удаления. |
| Пустой файл `EntitiesHealthDisplayPresenter.cs` | `UI/Gameplay/HealthDisplay/` | Найден попутно: 4586 байт, **0 непробельных символов** — файл из одних пробелов, класса нет. Мусор, но вне скоупа задачи. |

> Проверено попутно и **НЕ осиротело** (живое, не трогать):
> - `EntityHealthPresenter` — используется в `GameplayPresentersFactory` (это и
>   есть живой HP-презентер; подтверждает безопасность удаления
>   `EntitiesHealthDisplay` в п.4).
> - `ThrowableProjectile` (база) и `GrappleHookProjectile` — живой grapple-путь.

---

## Не тронуто (вне скоупа, по инструкции)
- Slime REWORK-заглушка (`CreateSlime → null`)
- Заглушка потион-эффектов в `InventorySystem`
- Закомментированный трекинг в `ClearAllEnemiesStage`
- 4 `NotImplementedException`-перегрузки без вызовов
