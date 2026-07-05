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
9c151897 cleanup: fully-orphaned MasterLootProviderConfig layer (variant A)
a913e78a cleanup: orphaned SecretLootConfig.asset (secret-loot feature)
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

---

## SecretLootConfig.asset — проверен и удалён (коммит `a913e78a`)

**Тип:** `MetaLootConfig` (ScriptableObject), НЕ `LootTableConfig` (в отличие от
удалённой `SecretChestsLootTable`). Поля: `PrefabPath=Entities/Loot/SecretLoot`,
`LootType=SecretLoot(5)`, `CollectSoundId=SecretLootCollect`, `SecretLootId`.

**Вердикт: орфан → удалён.** Проверка:
- GUID ассета (`9dda2644…`) — 0 ссылок в `.prefab`/`.unity`/`.asset` (ни одна
  `LootTableConfig` его не содержит).
- По имени в коде не упоминается; в словаре `ResourcesConfigsLoader` его нет;
  не грузится ни `GetConfig<>`, ни строковым путём, ни `LoadAll`.
- Сравнение с живыми: `CoinLootConfig`/`SoulShardLootConfig` имеют по 2 GUID-
  ссылки из лут-таблиц — механизм именно такой; у `SecretLootConfig` их 0.
- Это **та же** секретная фича, что выпиливали (SecretLootId / SecretLootCollect
  / LootType.SecretLoot), а не совпадение имени: живого потребителя нет вовсе
  (в отличие от `EnemyLoot`/`PropsLoot`, которые остались из-за реальных ссылок
  из `GhostConfig`/`SlimeConfig`/`SimpleProp`).

## ⚠️ Вскрылся ещё слой (НЕ трогал — нужно решение)

`SecretLootConfig.asset` был **единственным** `MetaLootConfig`-ассетом в проекте.
Как следствие:

| Объект | Статус | Примечание |
|---|---|---|
| `Entities/Loot/SecretLoot.prefab` | орфан | 0 ссылок в `.prefab`/`.unity`/`.asset`. Целевой префаб удалённого конфига (`PrefabPath`). Явно часть той же секретной фичи. Кандидат на удаление. |
| Ветки `config is MetaLootConfig` в `LootFactory.cs:28,81` | мёртвые в рантайме | Инстансов `MetaLootConfig` больше не существует → ветки недостижимы. Но это живой-выглядящий код; тип `MetaLootConfig` (класс) остаётся. Требует решения: удалять ли ветки/класс `MetaLootConfig` или оставить как каркас. |
| `LootTypes.SecretLoot` (enum) + `MetaLootConfig.SecretLootId` (поле) | остатки фичи | Больше не используются на уровне данных. Малозначимо; трогать только по явному решению. |

> Рекомендация: не удалять эти по инерции. `SecretLoot.prefab` — безопасный
> кандидат (0 ссылок). Ветки/класс `MetaLootConfig` — судить отдельно: заброшено
> совсем или задел под будущую «мета-раздачу» лута.

---

## Секретные остатки лута — удалены (коммит `13ada635`)

По решению владельца снесены три объекта, специфичные для уже выпиленной
секретной фичи (отдельного потребителя нет):

| Объект | Проверка |
|---|---|
| `Resources/Entities/Loot/SecretLoot.prefab` | 0 GUID-ссылок; целевой префаб удалённого `SecretLootConfig`. |
| `LootTypes.SecretLoot` (enum-значение) | Было **последним** членом (5) → удаление не сдвигает нумерацию; после удаления `SecretLootConfig.asset` ни один ассет не хранил `LootType=5`, код нигде не ссылался. |
| `MetaLootConfig.SecretLootId` (поле) | Читалось нигде. |

**Оставлено по решению владельца** (отдельный архитектурный вопрос):
`MetaLootConfig` (класс, теперь пустой `: LootConfig`) и ветка
`config is MetaLootConfig` в `LootFactory.cs:28,81`. Компиляция чистая,
предупреждений нет.

> Мелкое наблюдение (не действие): удалённый `SecretLootConfig` держал
> `CollectSoundId = "SecretLootCollect"`. Если в `AudioLibrary` есть клип под
> этим ключом и он больше нигде не используется — это возможный аудио-остаток
> той же фичи. Не проверял глубоко, не в скоупе; при желании — отдельным проходом.

---

## Аудио-хвост секретной ветки — проверен, удалять нечего

Проверка ключа `SecretLootCollect` (был в `CollectSoundId` удалённого `SecretLootConfig`):

- **`AudioLibrary` устройство:** `Sounds` — `List<SoundData>`, где `SoundData` —
  отдельные ScriptableObject-ассеты; ключ (`AudioData.Key`) лежит внутри каждого,
  поиск через `GetSound(key)`.
- **Ни один `SoundData`-ассет не имеет ключа `SecretLootCollect`**; клипа/файла с
  таким именем нет; строка `SecretLootCollect` во всём проекте больше не
  встречается (ушла вместе с конфигом). → Это была **висячая строка** в конфиге,
  реального звука под неё не существовало. Удалять клип/запись не из чего.

### Другие аудио-ключи секретной ветки — НЕ найдено
Просмотрены все ключи библиотеки — ни `Secret`, ни `Chest`, ни аналогов
(сундук/тайник). Ближайшие collect/spawn-ключи обслуживают **живой** лут и к
секреткам не относятся: `CoinCollectSound`, `CoinSpawn`, `SoulShardCollectSound`,
`SoulShardSpawn`, `BuffPickup`, `ItemEmpty`.

**Вывод:** аудио-остатков у секретной фичи нет. Ветка «секреток» вычищена
полностью. В код не лез (по указанию), удалений на этом шаге нет.

## Итог: ветка «секреток» закрыта
Полностью выпилено за все проходы: `SecretChestCollectService`, `ChestSpawnMarker`,
`SecretChestsLootTable.asset`, поле `MasterLootProviderConfig.SecretChestLoot`
(+ весь осиротевший `MasterLootProviderConfig`), `SecretLootConfig.asset`,
`SecretLoot.prefab`, enum-значение `LootTypes.SecretLoot`, поле
`MetaLootConfig.SecretLootId`. Аудио-остатков нет. Оставлено по отдельному
решению: класс `MetaLootConfig` (пустой) + ветка `is MetaLootConfig` в `LootFactory`.
