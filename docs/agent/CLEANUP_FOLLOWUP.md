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

### РЕШЕНО: владелец выбрал вариант A — удалено (коммит `9c151897`)

Перед удалением выполнена целенаправленная GUID-проверка (узко по текстовым
типам, т.к. полный project-wide grep падал по таймауту на бинарниках):
GUID `MasterLootProvider.asset`, скрипта `MasterLootProviderConfig` и
`SecretChestsLootTable.asset` — **ни одной прямой `[SerializeField]`-ссылки** в
`.prefab`/`.unity`/сериализованных `.asset` (кроме собственной пары
скрипт↔.asset). Проверен и префолд: `LoadAsync` итерирует словарь
`_configsResourcesPath` и грузит все конфиги скопом, но `MasterLootProviderConfig`
после загрузки никто не запрашивает — удаление строки словаря безопасно.

Удалено:
- `Configs/Gameplay/Loot/MasterLootProviderConfig.cs` (класс)
- `Resources/Configs/Gameplay/Loot/MasterLootProvider.asset`
- `Resources/Configs/Gameplay/Loot/SecretChestsLootTable.asset` (осиротел после №3)
- `ResourcesConfigsLoader.cs`: строка регистрации (32) + ставший неиспользуемым
  `using ...Configs.Gameplay.Loot`

Оставлено (живое): `EnemyLootTable.asset` (ссылки из `GhostConfig`/`SlimeConfig`),
`PropsLootTable.asset` (ссылка из `SimpleProp`). Реальная раздача лута идёт
напрямую через per-entity конфиги (`GhostConfig.LootTable` и т.п.) — поэтому
«мастер-провайдер» и оказался мёртв целиком: заброшенная попытка централизации.

Компиляция чистая, missing-reference предупреждений нет. **Дальнейшего слоя
орфанов этот шаг не вскрыл** — обе оставшиеся таблицы имеют живых потребителей.

> Не трогалось: `SecretLootConfig.asset` в той же папке — отдельный ассет, не
> входил в вариант A; в скоуп не брал.
