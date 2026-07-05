# NYPD — Дочистка орфанов (follow-up к CLEANUP_DONE.md)

> 4 находки из предыдущего прохода, подтверждённые владельцем. Процесс тот же:
> свежая проверка ноль-ссылок перед каждым удалением, компиляция через Unity MCP
> после каждого блока (**везде 0 ошибок**), логические коммиты. Дерево чистое.

## Коммиты
```
add96ed4 cleanup: orphaned IsTouchAnotherTeam component
4bff7323 cleanup: pre-existing orphans ThrowEvent/ThrowRequest
016f3a7a cleanup: dead SecretChestLoot field from MasterLootProviderConfig
5f15ad45 cleanup: empty whitespace file EntitiesHealthDisplayPresenter.cs
```
Также попутно (не блокировало чистку): починена битая ссылка
`refs/remotes/origin/HEAD` — `git remote set-head origin -a` → `origin/main`.
`fatal: bad object` при auto-gc больше не появляется.

## Что удалено

| # | Находка | Файлы / правки | Коммит |
|---|---|---|---|
| 1 | `IsTouchAnotherTeam` (компонент) | `Sensors/SensorsComponents.cs` (класс) + `Generated/EntityAPI.cs` (сгенерённый блок) — синхронно | add96ed4 |
| 2 | `ThrowEvent` / `ThrowRequest` (компоненты) | `ThrowableFeature/ThrowableComponents.cs` удалён целиком (в нём были только эти 2 класса) + их блоки в `Generated/EntityAPI.cs` | 4bff7323 |
| 3 | `MasterLootProviderConfig.SecretChestLoot` (поле) | `Configs/Gameplay/Loot/MasterLootProviderConfig.cs` (property) + `Resources/.../MasterLootProvider.asset` (backing field) | 016f3a7a |
| 4 | Пустой `EntitiesHealthDisplayPresenter.cs` | файл + `.meta` (4586 байт пробелов, типа внутри нет) | 5f15ad45 |

Проверки по каждому пункту:
- **№1:** единственным потребителем был удалённый ранее `AnotherTeamTouchDetectorSystem`; после — только объявление + генерённый API. GUID `SensorsComponents.cs` в префабах/сценах/ассетах отсутствует. Удалено синхронно (класс + блок API), иначе была бы ошибка компиляции.
- **№2:** ссылок из живого кода нет — только генерённый API. GUID файла нигде. Файл содержал ровно 2 класса → удалён целиком.
- **№3:** структура конфига — три **независимых** параллельных поля `LootTableConfig`: `EnemyLoot`, `SecretChestLoot`, `PropsLoot`. Поле изолированное (не часть переплетённой структуры) → удалил **только** `SecretChestLoot` (property + backing field в `.asset`); `EnemyLoot`/`PropsLoot` не тронуты. Ноль код-ссылок на `SecretChestLoot`. Конфиг грузится без ошибок.
- **№4:** 0 непробельных символов, ни типа, ни неймспейса, GUID нигде. Живой `EntityHealthPresenter` (без «s», используется в `GameplayPresentersFactory`) — **не тронут**, это другой файл.

## Пропущено из-за живых ссылок
**Ничего.** Все 4 находки имели подтверждённые ноль-ссылок.

---

## ⚠️ Вскрылся ещё один слой орфанов — ОСТАНОВ, нужно решение

При проверке находки №3 обнаружено, что удаление в прошлый заход
`ShurikenProjectile` (единственного потребителя) осиротило **весь**
`MasterLootProviderConfig`, а не только поле секреток. НЕ удалял — по инструкции
останавливаюсь и спрашиваю. Факты:

| Объект | Статус | Обоснование |
|---|---|---|
| `MasterLootProviderConfig` (класс) + `MasterLootProvider.asset` | зарегистрирован, но **никогда не запрашивается** | `GetConfig<MasterLootProviderConfig>()` не вызывается нигде (есть только строка регистрации в `ResourcesConfigsLoader.cs:32`). Оставшиеся поля `EnemyLoot`/`PropsLoot` не читаются ни в одном `.cs`. Единственным прошлым потребителем был удалённый `ShurikenProjectile` (и то в закомментированной строке). |
| `SecretChestsLootTable.asset` | **полностью осиротел** | После удаления поля `SecretChestLoot` (№3) на этот loot-table больше не ссылается ни один `.asset`/`.prefab`/`.unity` (только собственный `.meta`). |
| `EnemyLoot` loot-table asset (`a54cc72…`) | **живой** | 3 ссылки в ассетах/сценах (используется помимо `MasterLootProvider`). Удаление `MasterLootProviderConfig` его не осиротит. |
| `PropsLoot` loot-table asset (`9dd2772…`) | **живой** | 2 ссылки в ассетах/сценах. |

### Что предлагается решить
Удалять ли этот слой, и насколько глубоко:
- **(A)** Весь `MasterLootProviderConfig`: класс + `MasterLootProvider.asset` +
  строка регистрации в `ResourcesConfigsLoader.cs` + осиротевший
  `SecretChestsLootTable.asset`. (`EnemyLoot`/`PropsLoot` таблицы остаются —
  на них ссылаются другие места.)
- **(B)** Только полностью осиротевший `SecretChestsLootTable.asset`; сам
  `MasterLootProviderConfig` оставить (вдруг задел под будущую лут-раздачу).
- **(C)** Ничего не трогать — только зафиксировано в отчёте.

> Замечание: `MasterLootProviderConfig` может подгружаться скопом при
> `ConfigsProviderService.LoadAsync()` даже без `GetConfig<>()` — проверить,
> прежде чем удалять строку регистрации, чтобы не сломать префолд конфигов.
