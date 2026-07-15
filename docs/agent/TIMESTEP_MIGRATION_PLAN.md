# NYPD — План миграции core-геймплея на фиксированный таймстеп

> **✅ МИГРАЦИЯ ЗАВЕРШЕНА. Все 9 этапов закрыты и подтверждены плейтестом.**
> Итог, вскрывшиеся дочистки, оставшийся техдолг и чек-лист финального прогона —
> в **§8**. Оставшийся долг (R5–R9) — отдельные заходы, не блокеры.
>
> Документ начинался как read-only план и по ходу работы стал журналом: отметки
> этапов содержат не только «сделано», но и разбор мест, где прогноз разошёлся с
> плейтестом. Это намеренно — расхождения полезнее, чем итоговая правда.
> Ветка `really_actual_develop_branch_mcp`. Unity 6000.4.11f1.
> Сопутствующий снимок кода — `docs/agent/ORIENTATION.md`.

---

## 0. TL;DR — зачем вообще миграция

Факт из `ProjectSettings/Physics2DSettings.asset`:

```
m_SimulationMode: 0          # 0 = FixedUpdate → физика уже тикает на 50 Гц (0.02с)
m_AutoSyncTransforms: 0
m_Gravity: {x: 0, y: -9.81}
```

`ProjectSettings/TimeManager.asset`: `Fixed Timestep: 0.02`, `Maximum Allowed
Timestep: 0.333`.

При этом **вся геймплейная логика тикает из переменного `Update()`**
(`GameplayBootstrap.Update()`, строки 112–119):

```csharp
private void Update()
{
    float deltaTime = Time.deltaTime;      // ПЕРЕМЕННЫЙ таймстеп
    _brainsContext?.Update(deltaTime);     // AI
    _entitiesLifeContext?.Update(deltaTime);// все ECS-системы, включая движение
    _gameplayStatesContext?.Update(deltaTime);// уровневый флоу, стиль, таймер
}
```

**Корень проблемы.** Все системы движения пишут `rigidbody.linearVelocity`
напрямую из этого переменного Update-тика, а Unity-физика интегрирует эту
скорость на фиксированных 50 Гц. Возникает классический рассинхрон:

- При 144 fps velocity перезаписывается ~3 раза между двумя физ-шагами —
  физика видит только последнее значение, промежуточные записи «съедаются».
- При 30 fps один физ-шаг может не случиться между кадрами, при 200 fps —
  несколько записей на один шаг. Итог — **фил движения зависит от FPS**, а
  прогон недетерминирован (это же блокирует ghost-replay/лидерборды из
  `gameplay_design.md`).
- Ускорение через `Mathf.MoveTowards(..., rate * deltaTime)` сглаживается на
  частоте кадров, но реальное перемещение делает физика на 50 Гц → двойное
  несоответствие.

**Цель миграции.** Перенести запись скорости и связанные с ней окна/тайминги на
`FixedUpdate` (тот же тик, что и интеграция физики), сохранив субъективный фил.
Немешанные с физикой подсчёты (стиль-decay, баффы, уровневый таймер, попапы,
VFX) осознанно **оставляем на Update** — переносить их незачем, а перенос
затянет их под `Time.timeScale`-хитстоп и добавит риска на пустом месте.

**Спасительная деталь для поэтапности.** `RigidbodyMovementSystem.OnUpdate`
(строка 50) выходит, если `CurrentMovementState != Default`. То есть владение
скоростью арбитрируется через реактивный `CurrentMovementState`: в каждый момент
рулит одна система. Это позволяет мигрировать по одной — **при условии**, что
для каждой мигрируемой системы проверено, что она честно ставит/снимает
`CurrentMovementState` (иначе она пишет velocity параллельно с базовой
локомоцией и обязана ехать в одном этапе с ней). См. риск R2 в §5.

---

## 1. Инвентаризация: где логика зависит от таймстепа

### 1.1 Тиковые контексты (главный луп)

| Контекст | Файл | Тик сейчас | Что тикает |
|---|---|---|---|
| `AIBrainsContext.Update(dt)` | `Features/AI/AIBrainsContext.cs` | Update / `Time.deltaTime` | AI-мозги врагов (пока только Ghost) |
| `EntitiesLifeContext.Update(dt)` | `EntitiesCore/EntitiesLifeContext.cs` | Update / `Time.deltaTime` | ВСЕ ECS-системы всех сущностей |
| `GameplayStatesContext.Update(dt)` | `Gameplay/States/GameplayStatesContext.cs` | Update / `Time.deltaTime` | Флоу уровня + `LevelProcessState` (таймер, стиль-decay, evaluator) |

`Entity.OnUpdate(dt)` (`EntitiesCore/Entity.cs:33`) прогоняет единый список
`_updatables` — **сейчас у Entity ровно один тиковый канал** (`IUpdatableSystem`).
Фиксированного канала нет; его добавление — enabling-инфраструктура для миграции
(см. Этап 2).

### 1.2 Системы, пишущие в физику из Update (ядро «фила»)

Все тикают через `EntitiesLifeContext` → `Time.deltaTime`, пишут
`rigidbody.linearVelocity` / `AddForce`:

| Система | Файл | Что делает с физикой |
|---|---|---|
| `RigidbodyMovementSystem` | `MovementFeature/Move/RigidbodyMovementSystem.cs` | Горизонтальный бег: `MoveTowards(rate*dt)` → пишет `linearVelocity` каждый тик |
| `SimpleRigidbodyMovementSystem` | `MovementFeature/Move/SimpleRigidbodyMovementSystem.cs` | Движение врага (Ghost): `linearVelocity = dir*speed` |
| `JumpSystem` | `MovementFeature/Jump/JumpSystem.cs` | Заряд/буфер на `deltaTime`; прыжок — `AddForce(Impulse)` (one-shot) |
| `AirJumpSystem` / `AirJumpsRecoverySystem` | `MovementFeature/AirJump/*` | Двойной прыжок + восстановление зарядов на таймере |
| `WallJumpSystem` | `MovementFeature/Jump/WallJumpSystem.cs` | Импульс от стены |
| `DashSystem` | `MovementFeature/Dash/DashSystem.cs` | Буфер/заряд/кулдаун на float-таймерах; **окно движения — корутина** `DashCoroutine` (`elapsed += Time.deltaTime`, `yield return null`) |
| `SlideSystem` | `MovementFeature/Slide/SlideSystem.cs` | То же: таймеры + **корутина** `SlideCoroutine` |
| `SlopeSlipSystem` / `SlopeSlideSystem` / `SlopeJumpSystem` | `MovementFeature/Slope/*` | Скольжение/прыжок по склону — работа с velocity/нормалями |
| `PlungeSystem` | `MovementFeature/Plunge/PlungeSystem.cs` | Пикирование вниз: `linearVelocity.y -= ramped*dt`, гасит gravityScale |
| `PlungeDamageOnImpactSystem` | `MovementFeature/Plunge/*` | Урон по приземлению |
| `WallHangSystem` | `MovementFeature/HangWall/WallHangSystem.cs` | Прилипание к стене |
| `GlideSystem` | `Gadgets/Glider/*` | Планирование |
| `GrappleSystem` | `Gadgets/Grapple/GrappleSystem.cs` | Фаза притяжки `UpdatePull(dt)` пишет `linearVelocity += dir*acc*dt`; **уже частично мигрирована** (pull переехал из корутины в `OnUpdate`), но летящий крюк (`GrappleHookProjectile`) — ещё корутина |
| `LethalContactMovementSystem` | `Combat/Contact/LethalContactMovementSystem.cs` | «Скорость = урон»: читает `linearVelocity.magnitude`, наносит урон при dash/plunge-контакте |
| `SurfaceCheckSystem` | `MovementFeature/Jump/SurfaceCheckSystem.cs` | `Collider2D.Cast` вниз (граунд/склон/coyote-timer на `deltaTime`) — **должен идти на физ-частоте**, иначе граунд-детект дрожит |

### 1.3 Тайминги-гейты на переменном dt (влияют на фил)

| Что | Файл | Природа |
|---|---|---|
| Coyote-time (0.1с) | `SurfaceCheckSystem.cs:10,84` | `_coyoteTimer -= deltaTime` — окно прыжка после схода с края = **фил прыжка** |
| Jump buffer/charge (0.15с) | `JumpSystem.cs:58,77` | `deltaTime` |
| i-frames после удачного удара | `Combat/Attack/AttackInvulnerabilitySystem.cs:36` | `_timer -= deltaTime` |
| Attack-process timer | `Combat/Attack/AttackProcessTimerSystem.cs:29` | `_currentTime += deltaTime` — тайминг окна атаки |
| Dash/Slide charge, buffer, cooldown | `DashSystem.cs`, `SlideSystem.cs` | Буфер/заряд — `deltaTime`; **кулдаун — `Time.unscaledDeltaTime`** (иммунитет к хитстопу) |

### 1.4 Корутины с таймингами (остатки)

| Корутина | Файл | Тик |
|---|---|---|
| ~~`DashCoroutine`~~ | `DashSystem.cs` | ✅ снята (Этап 6) — окно на fixed-тике |
| ~~`SlideCoroutine`~~ | `SlideSystem.cs` | ✅ снята (Этап 6) — окно на fixed-тике |
| ~~`ResetUsingFlag`~~ | `InventoryFeature/InventorySystem.cs` | ✅ снята (Этап 1) — reactive-таймер |
| `GrappleHookProjectile` | `Gadgets/Grapple/GrappleHookProjectile.cs` | Полёт крюка через `ICoroutinesPerformer` (pull-фаза уже НЕ корутина) |

`ICoroutinesPerformer` = `CoroutinesPerformer : MonoBehaviour`
(`Utilities/CoroutinesManagment/CoroutinesPerformer.cs`) — гоняет обычные Unity-
корутины, т.е. на Update-частоте (`yield return null`).

### 1.5 Хитстоп (важное взаимодействие)

`HitStopService.cs` крутит **`Time.timeScale`** через DOTween с `SetUpdate(true)`
(unscaled). Т.е. хитстоп глобально масштабирует и `Time.deltaTime`, и частоту
`FixedUpdate`. Поэтому:
- Системы, которые сейчас берут `Time.unscaledDeltaTime` (кулдауны Dash/Slide,
  гейт в `HitStopSystem` через `Time.unscaledTime`), **специально иммунны** к
  хитстопу — их логика продолжает идти, пока мир «заморожен».
- **⚠️ ИСПРАВЛЕНО (Этап 8).** Здесь раньше утверждалось, что «прямой перенос
  `unscaledDeltaTime` в FixedUpdate даст неверную величину». **Это неверно** —
  Unity внутри FixedUpdate сам отдаёт fixed-варианты (`deltaTime` →
  `fixedDeltaTime`, `unscaledDeltaTime` → unscaled fixed framerate delta time).
  Величина корректна без ручной правки. Подробности и цитаты из доков — в F4.
- **`timeScale` не масштабирует значение `fixedDeltaTime`** — он масштабирует
  частоту вызовов FixedUpdate. Двойного применения нет. См. F4.

### 1.6 Чисто визуальное / не-физическое — НЕ ТРОГАЕМ

Оставляем на Update осознанно:
- Все `*View`-классы (DOTween/VFX-анимации HUD и эффектов): `DashView`,
  `SlideView`, `GrappleRopeView`, `GlideView`, `PlungeView`, `WallHangView`,
  `SpeedTrailsView`, `SlopeRotationView`, `MovementView`, `PizzaDisplayView` и т.д.
- Стиль-система: `RankStyleService.UpdateDecay(dt)`, `StyleEvaluator.Tick(dt)` —
  вызываются из `LevelProcessState.Update` (`States/LevelProcessState.cs:78–80`).
- Баффы: `BuffsFeature/BuffsTimerSystem.cs` (decay длительностей).
- Уровневый таймер: `InGameTimers/InGameTimerFeatureService.Tick(dt)`. Он
  result-relevant (спидран-время!), но **timestep-инвариантен**: сумма дельт =
  реальному времени при любом тике. Переносить не нужно.
- AI-таймеры смены направления: `RandomMovementState.Update(dt)`. Это решение
  (какое направление), не интеграция — оставляем на Update/brains-тике.

### 1.7 Особые случаи — интерактивные объекты

**`Trampoline`** (`Features/InteractiveObjects/Trampoline.cs`) — vanilla
`MonoBehaviour`, триггерится через **`OnCollisionEnter2D`** (не Trigger). Что
делает мост (строки 36–54):
- Читает `MonoEntity.LinkedEntity.Rigidbody`.
- **Пишет velocity напрямую в обход системы движения**: гасит текущую
  вертикальную составляющую (`rb.linearVelocity -= up*currentUpVelocity`) и
  добавляет `AddForce(up * launch * mass, Impulse)`.
- **Никакого интента/события в ECS не шлёт** — это прямая физическая запись.

Важно: `OnCollisionEnter2D` — физический колбэк, он уже вызывается **внутри
физ-шага (FixedUpdate-контекст)**. То есть трамплин, парадоксально, уже «на
фиксированном тике», а системы движения — на Update. После миграции движения на
FixedUpdate трамплин станет **согласован** с ними (сейчас — рассинхрон). Но он
пишет velocity в обход арбитража `CurrentMovementState` — это архитектурная
развилка F5.

**`WindmillPhysicsHandler`** (`Features/PhysicsFeature/WindmillPhysicsHandler.cs`) —
уже правильный: чистый `FixedUpdate`, крутит `HingeJoint2D.motor`. **Миграции
не требует, эталон.**

---

## 2. Матрица «система → состояние → риск»

Риск-уровень по критерию: **ВЫСОКИЙ** — система напрямую формирует ощущение
движения/боя (пере­движение, дэш, прыжок, атака, i-frames, хитстоп, grapple);
**НИЗКИЙ** — второстепенное (UI/VFX/decay/таймеры-подсчёты).

| Система / узел | Состояние миграции | Риск |
|---|---|---|
| `RigidbodyMovementSystem` (бег/ускорение) | ✅ мигрировано (Этап 4) | **ВЫСОКИЙ** |
| `SurfaceCheckSystem` (граунд/склон/coyote) | ✅ мигрировано (Этап 4) | **ВЫСОКИЙ** |
| `JumpSystem` / `AirJumpSystem` / `WallJumpSystem` | ✅ мигрировано (Этап 5) | **ВЫСОКИЙ** |
| `DashSystem` | ✅ мигрировано (Этап 6, окно — fixed state-машина) | **ВЫСОКИЙ** |
| `SlideSystem` | ✅ мигрировано (Этап 6, окно — fixed state-машина) | **ВЫСОКИЙ** |
| `GrappleSystem` | ✅ мигрировано (Этап 7, pull на fixed; крюк — корутина, R5) | **ВЫСОКИЙ** |
| `PlungeSystem` | ✅ мигрировано (Этап 7) | **ВЫСОКИЙ** |
| `PlungeDamageOnImpactSystem` | событийный (тикового канала нет) — миграции не требовал | **ВЫСОКИЙ** |
| Slope-триада (`Slip`/`Slide`/`Jump`) | ✅ мигрировано (Этап 7); `SlopeSlide` без Y-guard — см. R6 | **ВЫСОКИЙ** |
| `WallHangSystem` / `GlideSystem` | ✅ мигрировано (Этап 7) | **ВЫСОКИЙ** |
| `AttackInvulnerabilitySystem` (i-frames) | ✅ мигрировано (Этап 8) | **ВЫСОКИЙ** |
| `AttackProcessTimerSystem` / cooldown-таймеры атаки | ✅ мигрировано (Этап 8); цепочка delay→хитбокс реактивная, уехала следом | **ВЫСОКИЙ** |
| `HitStopSystem` + `HitStopService` (`Time.timeScale`) | событийный, тикового канала нет — миграции не требовал (см. F4) | **ВЫСОКИЙ** |
| `LethalContactMovementSystem` («скорость=урон») | ✅ мигрировано (Этап 8); контактный буфер остался на Update — см. R7 | **ВЫСОКИЙ** |
| `Trampoline` (bounce) | ✅ F5 реализован (Этап 9): шлёт `BounceImpulseRequest`, применяет `BounceSystem` героя; прямой записи в чужой rigidbody больше нет | **ВЫСОКИЙ** (bounce = фил) |
| `SimpleRigidbodyMovementSystem` (Ghost) | не мигрировано (velocity на dt) | НИЗКИЙ (враг, не хватает игрока за душу) |
| `AIBrainsContext` / `RandomMovementState` | не мигрировано (Update-тик, решения) | НИЗКИЙ |
| `InventorySystem.ResetUsingFlag` | корутина `WaitForSeconds` | НИЗКИЙ (сброс флага, не физика) |
| `RankStyleService.UpdateDecay` / `StyleEvaluator.Tick` | Update (осознанно оставляем) | НИЗКИЙ |
| `BuffsTimerSystem` | Update (оставляем) | НИЗКИЙ |
| `InGameTimerFeatureService.Tick` | Update, timestep-инвариантен (оставляем) | НИЗКИЙ |
| Все `*View` (DOTween/VFX) | Update (оставляем) | НИЗКИЙ |
| `WindmillPhysicsHandler` | уже FixedUpdate | — (готово) |

---

## 3. Целевой механизм миграции (архитектурное решение — см. F1)

Рекомендуемый подход — **двухканальный тик Entity**, а не «big-bang перенос
всего `Update()` в `FixedUpdate()`». Причины: (а) задача требует поэтапности и
запрет на «всё одним PR»; (б) `CurrentMovementState`-арбитраж позволяет
переносить системы по одной; (в) не-физические подсчёты должны остаться на
Update.

Что добавляется (enabling-инфра, Этап 2):
- Новый интерфейс `IFixedUpdatableSystem { void OnFixedUpdate(float dt); }` рядом
  с `IUpdatableSystem` (в `EntitiesCore/Systems`).
- В `Entity` — второй список `_fixedUpdatables` + метод `OnFixedUpdate(dt)`
  (по образцу `_updatables`/`OnUpdate`, `Entity.cs:33–40, 98–99`).
- В `EntitiesLifeContext` — метод `FixedUpdate(dt)` (обратный проход с тем же
  try/catch, что `Update`).
- В `GameplayBootstrap` — метод `FixedUpdate()` c
  `_entitiesLifeContext?.FixedUpdate(Time.fixedDeltaTime)`.
- Дальше система мигрируется, меняя интерфейс `IUpdatableSystem` →
  `IFixedUpdatableSystem` (и `OnUpdate` → `OnFixedUpdate`), не трогая остальные.

Пока в `_fixedUpdatables` никто не добавлен — поведение идентично текущему
(канал дормантный). Это делает Этап 2 безопасным no-op с точки зрения фила.

Альтернатива (big-bang) описана в развилке F1 — её НЕ рекомендую.

---

## 4. Порядок этапов (низкий риск → высокий)

> Правило: **каждый этап — отдельный PR/коммит**. После каждого — обязательный
> ручной прогон в Play Mode по критерию «стоп-и-проверь». Ощущение сравниваем с
> предыдущим билдом (держать под рукой билд/ветку до этапа для A/B).
>
> Общий приём проверки фила на всех этапах: временно ограничить `targetFrameRate`
> до 30 и до 200 (`Application.targetFrameRate` уже ставится в `GameEntryPoint`)
> и убедиться, что **фил перестал зависеть от FPS** — это и есть главный
> критерий успеха миграции, а не только «не сломалось».

### Этап 1 — Инвентарь: корутина → reactive-таймер (LOW, без фила)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `38c57250`).** Ручной плейтест пройден:
> поведение идентично прошлому билду, флаг занятости снимается через ~0.15с и
> **не зависит от FPS**. Этап закрыт — не только закоммичен, но и подтверждён в
> Play Mode. (Коммиты: `a35eb8fc` — корутина→таймер; `38c57250` — снят неиспользуемый
> `ICoroutinesPerformer` из системы.)

- **Входит:** `InventoryFeature/InventorySystem.cs`.
- **Что меняется:** `ResetUsingFlag` (`WaitForSeconds(0.15f)` + корутина) →
  float-таймер в `OnUpdate` (поле `_useResetTimer`, декремент на `deltaTime`,
  по нулю `_isUsingItem.Value = false`). Убрать зависимость от
  `ICoroutinesPerformer` в этой системе. **Остаётся на Update** — это не физика.
- **Зачем первым:** обкатать паттерн «корутина → явный таймер» на безобидной
  системе, где ошибка не ломает фил.
- **Стоп-и-проверь:** бросить throwable-предмет, зажать спам-использование —
  частота использования не должна отличаться от текущего билда; флаг занятости
  снимается через те же ~0.15с. Проверить на 30 и 200 fps — интервал стабилен.

### Этап 2 — Enabling-инфра: фиксированный тиковый канал (LOW, no-op)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `ec87a550`).** Fixed-канал добавлен и
> **подтверждён как no-op**: `_fixedUpdatables` пуст (ни одна система не реализует
> `IFixedUpdatableSystem`), fixed-проход крутит пустой `foreach` — 0 записей в
> физику. Проверено статически по всем 4 осям возможного двойного тика
> (bootstrap / lifecontext / `Entity.OnFixedUpdate` / пустота канала) — двойного
> тика нет. Плейтест пройден, регрессии нет. Этап закрыт.
>
> **Диагностический эпизод (важно, не терять):** при плейтесте показалось, что
> персонаж движется быстрее. Разбор: код fixed-канала пуст, ускорить ничего не
> может. Корень — **ненадёжный A/B из-за FPS-зависимости движения** (тот самый
> дефект, что мигрируем): `RigidbodyMovementSystem` пишет velocity из Update на
> `Time.deltaTime`, разгон `MoveTowards(rate*deltaTime)` — при разном FPS между
> прогонами фил отличается. Типовой триггер: «before» шёл через `Init`
> (`targetFrameRate=60`), «after» — прямо со сцены `Gameplay` (кап не выставлен,
> редактор на сотнях fps). При залоченном FPS разница исчезает. **Вывод для
> будущих этапов: A/B делать только при жёстко залоченном `targetFrameRate`,
> иначе FPS-дрейф маскируется под регрессию.**

- **Входит:** `EntitiesCore/Systems/IFixedUpdatableSystem.cs` (новый),
  `EntitiesCore/Entity.cs`, `EntitiesCore/EntitiesLifeContext.cs`,
  `Gameplay/Infrastructure/GameplayBootstrap.cs`.
- **Что меняется:** добавить интерфейс + `_fixedUpdatables` + `OnFixedUpdate` в
  Entity, `FixedUpdate(dt)` в `EntitiesLifeContext`, `FixedUpdate()` в
  `GameplayBootstrap` (тикает только entities-fixed-канал через
  `Time.fixedDeltaTime`). **Никого в канал пока не добавляем.**
- **Стоп-и-проверь:** пройти уровень целиком — поведение обязано быть
  **идентичным** (канал пустой). Убедиться, что в консоли нет двойных тиков,
  entity не крашатся, teardown сцены чистый.

### Этап 3 — Движение врага на фиксированный канал (LOW)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `12c1e336`).** `SimpleRigidbodyMovementSystem`
> (движение Ghost) переведена `IUpdatableSystem` → `IFixedUpdatableSystem`
> (`OnUpdate` → `OnFixedUpdate`), логика не менялась (`velocity = dir * speed`).
> Первый реальный пассажир fixed-канала. Регистрация в `EnemiesFactory` не
> трогалась — `Entity.AddSystem` роутит по интерфейсу. `RandomMovementState`
> (решение о направлении) осталась на brains-тике (Update). Плейтест пройден:
> Ghost движется нормально, скорость та же, без дрожи, смена направления
> работает. Этап закрыт. `Ghost.prefab` `Interpolate = None` не менялся —
> джиттер не проявился, правка не потребовалась.

- **Входит:** `MovementFeature/Move/SimpleRigidbodyMovementSystem.cs`
  (при необходимости — регистрация в `EnemiesFactory`).
- **Что меняется:** `IUpdatableSystem` → `IFixedUpdatableSystem`
  (`OnUpdate` → `OnFixedUpdate`). Ghost — единственный враг; фил игрока не
  затрагивается. Первый реальный «пассажир» фиксированного канала.
- **Стоп-и-проверь:** посмотреть на блуждание Ghost 30–60с — без рывков/дрожи,
  скорость визуально та же. `RandomMovementState` остаётся на brains-тике
  (Update) — убедиться, что смена направления работает (мозг решает, физика
  исполняет на fixed).
- **Заметка по префабу:** `Ghost.prefab` имеет `Rigidbody2D.Interpolate = None`.
  Если на высоком refresh-rate появится джиттер призрака — включить Interpolate
  на префабе (правка префаба, не кода). У героя интерполяция уже включена.

### Этап 4 — Базовая локомоция + граунд-детект (HIGH)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммиты `21dedc03`, `5433c4e3`, `6012ab95`).**
> `RigidbodyMovementSystem` и `SurfaceCheckSystem` переведены `IUpdatableSystem` →
> `IFixedUpdatableSystem` (`OnUpdate` → `OnFixedUpdate`). `Cast` граунд-детекта и
> coyote-таймер (0.1с) теперь на физ-частоте; константу coyote не подстраивали —
> фил на 5 физ-шагах совпал. Плейтест пройден: бег/торможение, coyote, прыжок со
> склона, ходьба по склону — всё ок. **FPS-инвариантность 30/200 подтверждена:**
> длина тормозного пути совпадает между FPS (раньше — зависела).
>
> Уточнение по факту: FPS-инвариантность бега **фактически проверена вместе с
> Этапом 5** — единым прогоном на 30/200, где одновременно подтверждались и
> прыжки, и база (бег fps-инвариантен). Отдельного изолированного замера Этапа 4
> на 30/200 до Этапа 5 не было; результат от этого не меняется, но хронология
> зафиксирована честно.
>
> Перенос потянул за собой два fix-а поверх — оба оказались дочисткой давнего
> single-writer долга базовой локомоции (R2), вскрытого при переезде на fixed:
> - `5433c4e3` — база уступает velocity dash/slide/plunge (единственный писатель
>   в момент их владения `CurrentMovementState`), а не пишет параллельно.
> - `6012ab95` — база сохраняет положительный Y на склоне (прыжок переживает
>   арбитраж, вертикаль движения не затирается slope-логикой в тот же шаг).
>
> Оба — устранение параллельной записи velocity, ровно тот класс дефекта, что
> R2 предписывает закрывать при миграции базы. Этап закрыт.

- **Входит:** `RigidbodyMovementSystem`, `SurfaceCheckSystem`. **Только вместе:**
  граунд/склон-данные читаются локомоцией в тот же тик.
- **Что меняется:** обе → `IFixedUpdatableSystem`. `SurfaceCheckSystem.Cast`
  теперь на физ-частоте (корректнее). Coyote-таймер тикает `fixedDeltaTime`
  (проверить, что 0.1с ≈ 5 физ-шагов ощущается так же — при необходимости
  подстроить константу). **Предусловие:** подтвердить, что все прочие velocity-
  писатели, ещё живущие на Update (dash/slide/plunge/jump), в момент своей
  работы держат `CurrentMovementState != Default`, иначе конфликт записи (R2).
- **Стоп-и-проверь:** пробежать и резко развернуться на плоском участке уровня
  N; проверить ускорение/торможение и «прилипание» к земле на краю платформы
  (coyote). A/B с билдом до этапа. Прогнать на 30/144/200 fps — разгон и длина
  тормозного пути должны совпасть между FPS (сейчас — нет).

### Этап 5 — Прыжки (HIGH)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммиты `8dcf0294`, `00f6d018` — compile-fix
> `WallJumpSystem.UpdateEntrySpeed`, не получавший `deltaTime` после смены
> сигнатуры).** `JumpSystem`, `AirJumpSystem`, `AirJumpsRecoverySystem`,
> `WallJumpSystem` переведены `IUpdatableSystem` → `IFixedUpdatableSystem`.
> Буфер/заряд-таймеры и импульсы — на `fixedDeltaTime`; константы не
> подстраивали. Плейтест пройден: charged-jump (высота идентична), wall-jump-
> серии по двум стенам, coyote-прыжок «в последний момент», короткие тапы.
>
> **F3 подтверждён: edge-семплинг НЕ понадобился.** Короткие нажатия
> (нажал-отпустил внутри одного физ-шага) НЕ теряются — существующий held-
> источник интента + input-буферы (0.15с) справились. Развилка F3 закрывается
> по «превентивно НЕ вводим»: Update-семплинг edge-событий не добавляем.
>
> **FPS-инвариантность 30/200 подтверждена для прыжков.** Фидл стабилен между
> FPS. Тем же прогоном перепроверена и база Этапа 4 (бег) — см. отметку Этапа 4.
> Этап закрыт.

- **Входит:** `JumpSystem`, `AirJumpSystem`, `AirJumpsRecoverySystem`,
  `WallJumpSystem`.
- **Что меняется:** → `IFixedUpdatableSystem`. Буфер/заряд-таймеры и импульсы
  на `fixedDeltaTime`. Внимание к jump-buffer (0.15с) и coyote (уже на fixed
  после Этапа 4) — вместе они = тайминг прыжка у края.
- **Стоп-и-проверь:** серия прыжков через стандартный гэп с шорткатом на уровне
  N; charged-jump (зажать до максимума) — высота идентична; wall-jump-серия по
  двум стенам; прыжок «в последний момент» после схода с платформы (coyote).
  A/B по высоте/дальности. Проверить: не теряется ли короткое нажатие прыжка,
  если оно случилось и отпустилось внутри одного физ-шага (см. развилку F3 про
  input-edge на fixed).

### Этап 6 — Дэш и слайд: корутины → fixed-таймеры (HIGH)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `569cd118`).** `DashSystem` и `SlideSystem`
> переведены `IUpdatableSystem` → `IFixedUpdatableSystem`. `DashCoroutine`/
> `SlideCoroutine` развёрнуты в state-машину окна внутри `OnFixedUpdate`
> (`_isInDashWindow`/`_isInSlideWindow` + `_windowElapsed`, инкремент на
> `fixedDeltaTime`); кривая `1 - t*t` не менялась. Зависимость от
> `ICoroutinesPerformer` убрана из обеих систем (в `EntitiesFactory` поле
> осталось живым — его использует `GrappleSystem`).
>
> **F4 применён как решено:** кулдаун/буфер-таймеры `Time.unscaledDeltaTime` →
> `Time.fixedUnscaledDeltaTime`. Хитстоп-иммунитет сохранён, поведение не менялось.
>
> **Неочевидная деталь переноса (не потерять для будущих корутин).** Корутина,
> запущенная через `StartPerform`, выполняется синхронно до первого `yield` —
> т.е. на кадре старта успевала сделать пре-луповую запись, первый проход тела
> цикла (`t=0`) И первый инкремент `elapsed`. Наивный перенос («стартовали окно —
> крутим со следующего тика») удлинил бы окно ровно на один физ-шаг (~2% длины
> рывка). В коде окно прокручивается в том же тике, где стартовало — эквивалент
> корутины сохранён.
>
> **Плейтест пройден:** дэш min/max по длине рывка, air-dash с вертикальным
> бустом, слайд под низким проходом (капсула меняется и возвращается корректно),
> хитстоп-иммунитет кулдауна. **FPS-инвариантность 30/200 подтверждена:** длина
> дэша стабильна между FPS.
>
> **Дэш сквозь врага — работает как раньше**, регрессии нет. Это тот самый
> временный кросс-тик, который план предписывал зафиксировать наблюдением:
> `LethalContactMovementSystem` до Этапа 8 живёт на Update и читает velocity
> между физ-шагами, пока дэш пишет её на fixed. На практике связка «скорость =
> урон» этого не заметила. Кросс-тик исчезнет сам на Этапе 8, когда система
> переедет на fixed; отдельного действия не требует.
>
> Этап закрыт полностью — открытых пунктов нет.

- **Входит:** `DashSystem`, `SlideSystem`.
- **Что меняется:** окна движения `DashCoroutine`/`SlideCoroutine` → state-
  машина внутри `OnFixedUpdate` (поля `_isInWindow`, `_windowElapsed`,
  декремент/инкремент на `fixedDeltaTime`, та же кривая `1 - t*t`). Сами системы
  → `IFixedUpdatableSystem`. **Развилка F4:** кулдаун-таймеры сейчас на
  `Time.unscaledDeltaTime` (иммунитет к хитстопу) — на fixed это
  `Time.fixedUnscaledDeltaTime`; решить, сохраняем ли хитстоп-иммунитет
  кулдауна. Убрать зависимость от `ICoroutinesPerformer` в этих системах.
- **Стоп-и-проверь:** заряженный дэш min/max по длине рывка; air-dash (с
  вертикальным бустом); дэш сквозь врага (проверить связку с
  `LethalContactMovementSystem` — «скорость=урон» ещё на Update до Этапа 8, это
  временный кросс-тик, зафиксировать наблюдение); слайд под низким проходом на
  уровне N, слайд-хитбокс (капсула) меняется/возвращается корректно; дэш/слайд
  во время хитстопа. A/B по длине рывка и «весу» слайда.

### Этап 7 — Grapple, plunge, slope, wall-hang, glide (HIGH)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `acfcc421`).** `GrappleSystem`,
> `PlungeSystem`, `SlopeSlipSystem`, `SlopeSlideSystem`, `SlopeJumpSystem`,
> `WallHangSystem`, `GlideSystem` переведены `IUpdatableSystem` →
> `IFixedUpdatableSystem`. Чистый перенос канала: логика и конфиги не менялись.
> `GrappleSystem.UpdatePull` был структурно готов — сменился только источник dt.
> **F6 соблюдён:** летящий крюк `GrappleHookProjectile` остаётся на корутине
> (хвост R5). Плейтест пройден: grapple/plunge/slope/hang/glide, **FPS 30/200
> стабильно**. Этап закрыт.
>
> **`PlungeDamageOnImpactSystem` кода не потребовал — расхождение с §2.** Он
> событийный (`IInitializableSystem` + `IDisposableSystem`, подписка на
> `PlungeImpactEvent`), тикового канала не имеет вовсе. Его `OverlapCircleAll`
> и так исполняется на физ-тике, потому что `PlungeSystem.HandleLanding` переехал
> на fixed. В матрице §2 он числился «не мигрировано (dt)» — это была ошибка
> снимка, а не состояние кода.
>
> **Слоуп-прыжок из зажатого слайда выживает — структурно, проверено.** При
> миграции был прогноз, что `SlopeSlideSystem` (без Y-guard, безусловная
> абсолютная запись velocity) затрёт вертикаль прыжка на тике N+1. Плейтест
> прогноз опроверг, разбор сошёлся с плейтестом. Причина: `SurfaceCheckSystem`
> кастует вниз на `castDistance = 0.15м`, а слоуп-прыжок даёт
> `targetY = BaseJumpForce(15) + |vx| * 3` — минимум ~26, обычно 45–57. За один
> физ-шаг это подъём 0.5–1+ м, т.е. персонаж выносится из зоны каста с запасом
> 3.5–7×. На тике N+1 `_isOnSlope = false` → `SlopeSlideSystem` уходит в
> `Default` и делает `return` ДО записи velocity. Плюс внутри тика прыжка защищает
> порядок: `SlopeSlide` (472) пишет раньше, `SlopeJump` (473) перезаписывает
> последним, физика интегрирует прыжок. **Y-guard в `SlopeSlideSystem` не
> добавлен — не нужен.** НО безопасность держится на числах конфига, а не на
> инварианте: занесено в §5 как R6.
>
> **Побочный эффект переноса (ожидаемый, конфиги НЕ тюнились).** `_lastFrameVelocityY`
> в `SlopeSlideSystem` на fixed — буквально скорость предыдущего физ-шага, а не
> случайный семпл между шагами: на Update при высоком FPS он успевал
> перезаписаться нулём после того, как физика гасила падение, и авто-слайд при
> приземлении (`MinFallVelocityForAutoSlide = -25`) вместе с `fallImpactSpeed`
> терялись. Теперь детект честный.

- **Входит:** `GrappleSystem` (+ дочистка `GrappleHookProjectile` корутины
  полёта, если решим), `PlungeSystem`, `PlungeDamageOnImpactSystem`,
  `SlopeSlipSystem`, `SlopeSlideSystem`, `SlopeJumpSystem`, `WallHangSystem`,
  `GlideSystem`.
- **Что меняется:** все → `IFixedUpdatableSystem`. `GrappleSystem.UpdatePull` уже
  структурно готов (реактивная тяга на dt) — просто меняет источник dt на fixed;
  перетюнить `PullAccelerationFactor`/`GrappleSpeed` при необходимости
  (комментарии в файле уже предупреждают, что это макс.скорость, а не сила).
  Летящий крюк можно перевести с корутины на fixed-тик (опционально, F6).
- **Стоп-и-проверь:** grapple-качели через большой пролёт на уровне N (ранний
  отпуск сохраняет дугу?); plunge с высоты → импакт-урон/приземление; спуск/
  въезд по склону (нет ли дребезга на стыке склон↔плоскость); wall-hang и
  срыв/переход в wall-jump; глайд-дальность. A/B по дуге грэпла и скорости
  плунжа. FPS-инвариантность 30/200.
- **⚠️ НЕ ТЮНИТЬ КОНФИГИ ДО ЭТОГО ЭТАПА (заметка после Этапа 2).** После
  добавления пустого fixed-прохода (Этап 2) вертикальные системы, которые всё
  ещё живут на Update и сами пишут velocity (`PlungeSystem`, `SlopeSlideSystem`),
  на плейтесте ощущаются чуть иначе по таймингу («плавнее, но быстро»). **Код
  этих систем НЕ менялся.** Это ожидаемый сдвиг фазы Update-записи velocity
  относительно физшага (Update-писатель + fixed-интеграция), **не регрессия**.
  Отсюда правило: `Speed` / `AccelerationMultiplier` / `SlideAcceleration` и
  прочие числа Plunge/Slope **не трогаем**, пока эти системы не переехали на
  `IFixedUpdatableSystem`. Финальный фил и калибровка чисел — только ПОСЛЕ
  переезда на fixed, здесь, на Этапе 7. Тюнинг до миграции = переделывать дважды
  (fixed-переезд снова сдвинет тайминг).

### Этап 8 — Боевые тайминги и «скорость=урон» (HIGH)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `8f197a34`).**
> Переведены `IUpdatableSystem` → `IFixedUpdatableSystem`:
> `LethalContactMovementSystem`, `AttackProcessTimerSystem`,
> `AttackInvulnerabilitySystem`, `AttackCooldownTimerSystem`,
> `DoubleAttackCooldownSystem`, `ApplyDamageCooldownSystem`. Чистый перенос
> канала: пороги `MinSpeedThreshold=10` / `MaxSpeedThreshold=20` и конфиги не
> тронуты.
>
> **Кросс-тик Этапа 6 закрыт.** `LethalContact` (рег. 519) идёт после Dash (468)
> и Plunge (482) → читает velocity в том же тике, где они её пишут, а не
> промежуточное значение между физ-шагами.
>
> **Атакующая цепочка уехала на fixed автоматически.** `AttackProcessTimerSystem` —
> единственный драйвер окна атаки; `AttackDelayEndTriggerSystem`,
> `MeleeAttackHitSystem`, `EndAttackSystem` тикового канала не имеют и висят на
> подписке на `AttackProcessCurrentTime`. Поэтому перевод таймера на fixed втянул
> всю цепочку (delay → `OverlapCircleAll` хитбокса → `SuccessfulHitEvent` →
> i-frames/хитстоп) в тот же физ-тик. Расщепления нет.
>
> **`HitStopSystem` не тронут** — событийный, тикового канала нет,
> `Time.unscaledTime` самоадаптируется. Двойного масштабирования `timeScale` нет.
> Прежняя формулировка F4 содержала фактическую ошибку — исправлена, см. F4.
>
> **`ApplyDamageCooldownSystem` включён по трактовке** (в §4 явно не назван): это
> окно неуязвимости при получении урона, оно гейтит `canApplyDamage` — тот самый
> контактный урон, что уехал на fixed. Окно тикает там же, чтобы совпадать с
> физикой контактов.
>
> **Осознанно НЕ тронуто:** контактный буфер (`BodyContactDetecting`/`Filter`)
> остался на Update — перенос сломал бы снаряды, см. **R7**. `StartAttackSystem` и
> `SlashAttackChargeSystem` остались на Update — это решения по инпуту, не окна
> физики.
>
> **Плейтест пройден:** дэш/пике сквозь врага (пороги 10/20 — **fps-инвариантно
> на 30/200**), хитстоп по длине как раньше, i-frames (второй удар внутри окна не
> проходит), джаггл. **Хитбокс атаки на физ-тике — попадание не съехало**
> (ожидавшийся сдвиг по быстро движущемуся врагу не проявился).
>
> **Наблюдение, уточняющее R7:** предполагалось, что при FPS < 50 протухание
> контактного буфера на шаг может смазать дэш сквозь врага на 30 fps. На плейтесте
> **не проявилось** — «скорость = урон» отработала fps-инвариантно. Это не
> отменяет R7: причина, по которой сенсоры не переехали, — не эта staleness, а
> регрессия снарядов (детект на fixed при движении на Update). Хвост остаётся
> открытым по своей причине, но известный риск по герою на практике не выстрелил.
>
> Этап закрыт.

- **Входит:** `AttackInvulnerabilitySystem` (i-frames), `AttackProcessTimerSystem`
  и cooldown-таймеры атаки, `LethalContactMovementSystem`, а также ревизия
  `HitStopSystem`/`HitStopService` под fixed.
- **Что меняется:** i-frames и attack-process-таймеры → fixed (чтобы окна
  совпадали с физикой контактов/хитбоксов). `LethalContactMovementSystem` читает
  `linearVelocity` — на fixed читает «настоящую» физ-скорость шага (сейчас — на
  Update, между шагами). **Хитстоп:** подтвердить, что `Time.timeScale`-подход
  корректно тормозит fixed-тик геймплея (частота FixedUpdate масштабируется
  timeScale) и что unscaled-гейты переведены на `fixedUnscaled*` там, где нужен
  иммунитет (F4).
- **Стоп-и-проверь:** цепочка ударов с хитстопом (заморозка ощущается так же по
  длине?); получить урон и проверить окно i-frames (нельзя получить второй удар
  в течение окна); dash/plunge сквозь врага — «скорость=урон» срабатывает при
  тех же порогах скорости (`MinSpeedThreshold=10`, `MaxSpeedThreshold=20`).
  Джаггл (`AerialHitSuspensionSystem`) — подвес врага в воздухе. A/B по длине
  хитстопа и «читаемости» i-frames.

### Этап 9 — Trampoline: сверка с новой fixed-локомоцией (HIGH, + развилка F5)

> **✅ ВЫПОЛНЕНО и ПРОВЕРЕНО (коммит `3e346584`).**
> F5 реализован: трамплин остался `MonoBehaviour` (ручная расстановка — рабочий
> процесс геймдизайнера), но **перестал писать velocity в чужой rigidbody**.
> Нарушение single-writer снято.
>
> **Модель (по образцу `TakeDamageRequest` → `ApplyDamageSystem`):** новый
> компонент `BounceImpulseRequest : IEntityComponent`
> (`ReactiveEvent<BounceImpulseData>`) + payload-структура
> `BounceImpulseData { UpAxis, LaunchVelocity }` — ось передаётся явно, потому что
> семантика отскока это «заменить компоненту вдоль оси, тангенциальную сохранить»,
> и голым вектором она не выражается. Новый `BounceSystem` живёт на герое и
> владеет его rigidbody легально. Трамплин сохранил свои `[SerializeField]`
> (`_baseBounceForce` / `_bouncinessMultiplier` / `_maxBounceForce`) и сам считает
> `finalLaunchVelocity` — упругость это его зона, применение к телу — зона систем
> сущности. Конвенция `*Request` (входящая просьба) / `*Event` (исходящее
> уведомление) соблюдена.
>
> **Применение немедленное, в колбэке запроса (не отложенное на `OnFixedUpdate`).**
> Прецедент — `DamageKnockbackSystem`, который делает ровно так. Обоснование:
> `OnCollisionEnter2D` стреляет внутри `Physics2D.Simulate`, т.е. уже в
> физ-контексте шага (это фиксирует и §1.7), а системы движения своё
> `OnFixedUpdate` к этому моменту отработали — гонки нет. Откладывание дало бы
> **+1 физ-шаг (20 мс)** задержки перед стартом, т.е. изменение фила; этап —
> рефактор архитектуры, а не механики. Нарушение single-writer снимается не
> таймингом, а тем, **кто** пишет.
>
> **Поведение не менялось:** гейт `CanBounce` НЕ добавлялся (отскок остаётся
> безусловным, только `IsMainHero`), формула силы и кап `_maxBounceForce` те же.
> Отскок реализован **вертикальным** (`UpAxis = Vector2.up`) — наклонных
> трамплинов в планах нет; см. **R9** про lockout, если появятся.
>
> **Вертикаль отскока переживает арбитраж** так же, как прыжок: вне склона база
> сохраняет Y, на склоне — Y-guard (`6012ab95`). Guard читает живую velocity, ему
> безразлично, кто её записал.
>
> **Заметка по генератору:** новый компонент потребовал перегенерации
> `EntityAPI.cs` (+24 строки, только новый API). Использован **ручной**
> `Tools/GenerateEntityAPI`. Всплыл bootstrap-тупик: генератор читает
> `Assembly-CSharp` рефлексией, а та не собиралась из-за ссылок на ещё не
> сгенерированный API. Порядок, который сработал: временно перевести обращения на
> несгенерированную форму (`GetComponent<T>().Value` / `AddComponent(...)`) →
> компиляция → генерация → вернуть сгенерированную форму → компиляция.
>
> **Плейтест пройден:** отскок с разной входной скоростью (высота масштабируется,
> кап `_maxBounceForce` работает), отскок после дэша, визуал-punch не рассинхронен.
> **Пике-в-батут проверено отдельно — отскок не проглатывается:** опасение, что
> `PlungeSystem` (пишет `new Vector2(x, newVelocityY)`, т.е. заменяет Y) съест
> вертикаль отскока на следующем тике, не подтвердилось — трамплин детектится как
> земля, плунж останавливается сам. Этап закрыт.

- **Входит:** `InteractiveObjects/Trampoline.cs`.
- **Что меняется:** технически трамплин уже на физ-колбэке (`OnCollisionEnter2D`),
  но пишет velocity в обход `CurrentMovementState`. Решить (F5): оставить как
  vanilla-мост с прямой записью (после Этапа 4+ он согласован с движением) или
  привести к интент/событийной модели (шлёт «bounce-impulse» в ECS, применяется
  на fixed-тике движения). Минимальный вариант — оставить, но убедиться, что
  запись velocity трамплином не конфликтует с системами, которые теперь тоже
  пишут на fixed в тот же шаг.
- **Стоп-и-проверь:** прыжок на трамплин с разной входной скоростью (низкой/
  высокой) — высота отскока масштабируется как раньше, `_maxBounceForce` кап
  работает; отскок сразу после дэша/плунжа (входная скорость большая); визуал-
  punch не рассинхронен с физикой. A/B по высоте отскока.

### Финальный прогон (после всех этапов)

- Полное прохождение 2–3 уровней с фокусом на «стильные» связки (dash → grapple
  → plunge → атака в воздухе) на 30/60/144/200 fps. Критерий: связки
  воспроизводятся одинаково по таймингам между FPS. Опционально — заготовка под
  детерминизм-тест (одинаковый вход → одинаковая траектория) как фундамент под
  ghost-replay.

---

## 5. Ключевые риски миграции (сквозные)

- **R1 — Рендер-джиттер на fixed-тике.** Если позиция интегрируется на 50 Гц, а
  экран 144 Гц, спрайт дёргается без интерполяции. **Проверено по префабам
  (см. F2): у героя интерполяция уже включена, у Ghost — нет.** Для игрока риск
  закрыт из коробки; для Ghost — правка префаба на Этапе 3, не кодом.
- **R2 — Гонка за `linearVelocity` между Update- и fixed-системами.** Пока часть
  писателей на Update, а часть на fixed, важно, чтобы в каждый момент velocity
  писал ровно один (арбитраж `CurrentMovementState`). Перед каждым этапом
  проверять, что мигрируемая система корректно ставит/снимает состояние. Если
  система пишет velocity вне арбитража — тащить её в один этап с базовой
  локомоцией.
- **R3 — Input-edge на fixed.** Edge-детект (`isPressedDown = intent &&
  !_wasLastFrame`) сейчас в Update-системах. При переносе в FixedUpdate короткое
  нажатие, случившееся и отпущенное внутри одного физ-шага, может потеряться.
  Частично закрыто существующими input-буферами (jump/dash/slide, 0.15с). См.
  F3.
- **R4 — Хитстоп × fixed.** `Time.timeScale`-хитстоп меняет частоту FixedUpdate;
  unscaled-иммунные таймеры при переносе требуют `fixedUnscaled*`. Легко
  ошибиться в величине. См. F4.
- **R5 — Незакрытый хвост: летящая фаза `GrappleHookProjectile` (корутина).**
  По решению F6 полёт крюка остаётся на корутине (Update-частота) — Этап 7
  мигрирует только pull-фазу. Это **сознательно оставленный недетерминизм**: пока
  крюк летит на переменном тике, полный детерминизм прогона (фундамент под
  ghost-replay/лидерборды) недостижим. Закрывать отдельным заходом, когда
  детерминизм станет активной целью. До тех пор — известный технический долг.

- **R6 — Отложенная мина: `SlopeSlideSystem` без Y-guard (config-dependent).**
  `SlopeSlideSystem` делает **безусловную абсолютную запись velocity**
  (`_rigidbody.linearVelocity = downSlopeDirection * _currentSlideSpeed`) без
  Y-guard — в отличие от базы `RigidbodyMovementSystem`, где guard добавлен
  коммитом `6012ab95` (не опускать восходящую Y, выданную другой системой).

  **Сейчас это безвредно ТОЛЬКО из-за чисел конфига.** Слоуп-прыжок даёт
  `targetY = BaseJumpForce + |vx| * JumpForceModifier.y` = `15 + |vx| * 3`; при
  `fixedDeltaTime = 0.02` и `SurfaceCheckSystem.castDistance = 0.15м` персонаж за
  один физ-шаг выносится из зоны каста склона (подъём 0.5–1+ м против каста
  0.15 м), поэтому на тике N+1 `_isOnSlope = false` и слайд уходит в `Default`
  до записи velocity. **Порог срабатывания мины: `targetY < 7.5`** (0.15 / 0.02).
  Текущий запас — 3.5–7×.

  **РИСК:** если при тюнинге уронить силу слоуп-прыжка примерно вдвое
  (`BaseJumpForce` → ~7 и ниже, либо `JumpForceModifier.y` → 0 при малом `|vx|`),
  баг проснётся — прыжок со склона начнёт **«клевать»** (стартует и тут же
  придавливается вниз). **Симптом будет выглядеть как проблема ПРЫЖКА, без
  видимой связи с правкой конфига слайда** — призрак, которого легко искать не
  там. Отдельно коварно то, что мина спит при высокой силе прыжка: правка
  конфига и проявление бага разнесены и по файлу, и по времени.

  **Страховка на будущее:** добавить в `SlopeSlideSystem` тот же Y-guard, что в
  базе (`if (_rigidbody.linearVelocity.y > slopeVelocity.y) slopeVelocity.y =
  _rigidbody.linearVelocity.y;`), сделав безопасность структурной, а не
  зависящей от чисел. **Не критично сейчас** (плейтест Этапа 7 подтвердил, что
  слоуп-прыжок из слайда выживает) — зафиксировано, чтобы не искать призрак.

- **R7 — Хвост Этапа 8: детект контактов остался на Update (осознанно).**
  На Этапе 8 на fixed переехал только `LethalContactMovementSystem` (потребитель),
  а producer'ы контактного буфера — `BodyContactDetectingSystem` и
  `BodyContactsEntitiesFilterSystem` — **остались на Update**. Причина: классы
  общие для 6 типов сущностей (Hero, ContactTrigger, Ghost, Slime[мёртвый],
  SlashProjectile, Shuriken), и их перенос **сломал бы снаряды**: снаряды ездят
  `TransformMovementSystem` на Update через Transform, поэтому детект на fixed
  (50 Гц) при движении на Update (до 200 Гц) стал бы семплить позицию вчетверо
  реже → сюрикен/слэш проскакивали бы врага насквозь. Это регрессия, поэтому
  Вариант «перенести всю цепочку» отклонён.

  **Почему это не регрессия сейчас:** при FPS ≥ 50 буфер фактически свежий
  (`m_AutoSyncTransforms: 0` → между физ-шагами `OverlapCollider` отдаёт то же
  самое, т.к. позиции меняются только на физ-шаге); при FPS < 50 частота детекта
  остаётся равной Update-частоте — ровно как до переноса. Velocity-стык при этом
  починен: `LethalContact` (519) зарегистрирован после Dash (468) и Plunge (482)
  и читает скорость в том же тике, где они её пишут.

  **Проверено плейтестом Этапа 8:** ожидавшегося смазывания дэша сквозь врага на
  30 fps **не обнаружено** — «скорость = урон» отработала fps-инвариантно на
  30/200. То есть по герою эта staleness на практике не ощущается. **R7 остаётся
  открытым не из-за неё**, а из-за снарядов: их детект нельзя увести на fixed,
  пока `TransformMovementSystem` живёт на Update.

  **Отдельный будущий этап (НЕ часть миграции героя):** fps-инвариантный детект
  контактов для снарядов/врагов — перенос `BodyContactDetectingSystem` +
  `BodyContactsEntitiesFilterSystem` + `DealDamageOnContactSystem` +
  `DeathMaskTouchDetectorSystem` + `TransformMovementSystem` снарядов на fixed.
  Свой blast radius (урон врагов, слэш, сюрикен) и свой плейтест.
  **Ограничение, которое всплывёт:** `FinalPointTriggerService` читает
  `ContactEntitiesBuffer` из своего `Update(dt)` и на fixed уехать не может —
  `GameplayBootstrap.FixedUpdate` тикает только `_entitiesLifeContext`, у сервисов
  fixed-канала нет. Его протухание безобидно (проверка «герой на финише»), но
  учесть надо.

- **R8 — Пред-существующий баг: инвертированный порядок сенсоров на слэш-снаряде.**
  В `ProjectileFactory.cs:83-84` системы зарегистрированы как
  `BodyContactsEntitiesFilterSystem` (83) → `BodyContactDetectingSystem` (84),
  т.е. **filter стоит РАНЬШЕ detect** — в отличие от всех остальных мест
  (Hero 517→518, Ghost 655→656, Shuriken 132→133), где порядок правильный.
  Поскольку `Entity` тикает системы в порядке регистрации, entity-буфер слэш-
  снаряда собирается из collider-буфера **прошлого тика** → детект попадания
  протух на тик.

  **Живёт и сейчас на Update; миграцией не создан и не лечится** — обнаружен при
  разборе зоны влияния на Этапе 8. Фикс — поменять строки 83 и 84 местами.
  Не делалось: вне скоупа Этапа 8, требует своего плейтеста слэш-атаки.

- **R9 — Наклонные трамплины: боковой импульс не защищён (заметка на будущее).**
  На Этапе 9 отскок реализован **чисто вертикальным** (`UpAxis = Vector2.up`) —
  наклонных трамплинов в планах уровней нет. Прежний код читал `transform.up`,
  т.е. поворот был технически возможен, но не использовался.

  **Если позже появятся повёрнутые трамплины:** у отскока возникнет
  горизонтальная (тангенциальная) компонента, и **базовая локомоция её съест** —
  `RigidbodyMovementSystem` на следующем тике подхватит её ресинком
  `_currentSpeedX` и погасит через `MoveTowards` к target по инпуту. Вертикали
  это не касается: её защищает Y-guard (`6012ab95`).

  **Готовый путь решения** — тот же приём, что у `WallJumpSystem`: флаг владения
  + lockout-таймер (`IsWallJumping` + `LockoutDuration`) и исключение этого флага
  из `canMove`. На время lockout база не трогает горизонталь, и боковой импульс
  доживает. **Сейчас НЕ реализовано осознанно** — вертикальному отскоку не нужно,
  а вводить неиспользуемый механизм незачем.

---

## 6. Развилки — нужно твоё решение ДО старта

**F1 — Механизм: двухканальный тик vs big-bang.**
Рекомендую двухканальный (`IFixedUpdatableSystem`, §3): поэтапно, не тащит
не-физику под fixed, совместимо с запретом «всё одним PR». Альтернатива —
перенести весь `GameplayBootstrap.Update()` в `FixedUpdate()` одним махом: проще
в коде, но затягивает под fixed и стиль-decay/таймер/AI-решения, ломает
поэтапность и усложняет A/B.

**РЕШЕНО:** двухканальный тик (`IFixedUpdatableSystem` рядом с
`IUpdatableSystem`), НЕ big-bang. Мигрируем поэтапно, по одной системе за раз,
через арбитраж `CurrentMovementState`. Big-bang-альтернатива отклонена.

**F2 — Рендер-интерполяция между физ-шагами.**
При fixed-тике на 50 Гц и мониторе 144 Гц без интерполяции будет джиттер.

**Фактические значения `Rigidbody2D.Interpolate` (проверено по префабам; enum:
0=None, 1=Interpolate, 2=Extrapolate):**

| Префаб / тело | BodyType | Interpolate |
|---|---|---|
| `MainHero` (root, движковый `entity.Rigidbody`) | Dynamic | **1 = Interpolate** |
| `MainHero` / `PizzaBody` (дочерний, hinge-подвес пиццы) | Dynamic | **1 = Interpolate** |
| `MainHero` / `PizzaPivot` | Kinematic | 0 = None (кинематик — норма) |
| `Ghost.prefab` | Dynamic | **0 = None** |
| `Slime.prefab` | Dynamic | **0 = None** (мёртвый код) |
| `Trampoline.prefab` | — | нет Rigidbody2D вовсе |

Выводы:
- **У героя интерполяция уже включена** → рендер-джиттер для игрока закрыт из
  коробки, вариант 1 для героя фактически уже применён. Отдельного решения по
  герою не требуется.
- **`Extrapolate` не используется нигде** → выбор «Interpolate vs Extrapolate»
  снят: везде, где интерполяция есть, это Interpolate.
- **Ghost — `None`.** На Этапе 3 при переносе движения врага на 50 Гц возможен
  лёгкий джиттер призрака на высоких refresh-rate. Риск **низкий** (враг),
  чинится включением Interpolate на `Ghost.prefab` — **правка префаба, не кода**.
  Внесено в чек-лист Этапа 3.

**Остаточный вопрос (только если джиттер проявится):** нужна ли своя
Transform-прокси-интерполяция (вариант 2) где-то помимо штатной Rigidbody2D —
по умолчанию НЕ требуется, штатной хватает.

**РЕШЕНО:** отдельного решения не требует. У героя `Interpolate` уже включён —
рендер-джиттер для игрока закрыт из коробки. `Ghost` на `None` чинится правкой
префаба `Ghost.prefab` на Этапе 3 (правка префаба, не кода). `Extrapolate` не
используется нигде — выбор между Interpolate/Extrapolate снят. Своя
Transform-прокси-интерполяция НЕ вводится, пока джиттер реально не проявится.

**F3 — Обработка edge-инпута на fixed.**
Оставляем текущие input-буферы как достаточную митигацию (риск потерять
одиночный release внутри физ-шага), или выносим сэмплинг edge-событий инпута на
Update в буфер, потребляемый на fixed?

**РЕШЕНО:** edge-инпут оставляем на текущих input-буферах (jump/dash/slide,
0.15с) — они уже сглаживают риск потери одиночного нажатия. Отдельный
Update-семплинг-буфер edge-событий добавляем ТОЛЬКО если стоп-и-проверь Этапа 5
реально обнаружит потерю коротких нажатий (нажал-отпустил внутри одного
физ-шага). Превентивно НЕ вводим.

**ЗАКРЫТО по факту Этапа 5:** потери коротких нажатий на плейтесте НЕ выявлено
(held-источник интента + буферы 0.15с справились). Условие для введения
edge-семплинга не наступило — **не вводим**. Развилка закрыта.

**F4 — Хитстоп-иммунитет кулдаунов на fixed.**
Сейчас Dash/Slide-кулдаун и гейт `HitStopSystem` берут `unscaledDeltaTime`/
`unscaledTime` (тикают сквозь заморозку). Сохраняем ли этот иммунитет (дэш-
кулдаун капает во время хитстопа) или хотим, чтобы хитстоп замораживал и
кулдауны?

**РЕШЕНО:** хитстоп-иммунитет кулдаунов сохраняем как есть. Поведение НЕ меняем:
это требование параллельности с текущим фидлом (дэш-кулдаун продолжает капать
сквозь заморозку), а не пересмотр механики.

**⚠️ ИСПРАВЛЕНО ПО ФАКТУ Этапа 8 — прежняя формулировка вводила в заблуждение.**
Здесь раньше утверждалось, что «прямой перенос `unscaledDeltaTime` в FixedUpdate
даст неверную величину» и что менять надо руками. **Это неверно.** По официальной
документации Unity:
- `Time.deltaTime` — *«When called from inside MonoBehaviour.FixedUpdate or
  anywhere in the Physics update loop, including coroutines that yield
  WaitForFixedUpdate, deltaTime returns Time.fixedDeltaTime»*;
- `Time.unscaledDeltaTime` — *«When called from inside MonoBehaviour's
  MonoBehaviour.FixedUpdate, it returns the unscaled fixed framerate delta
  time»*.

То есть Unity **подставляет fixed-варианты автоматически** — `Time.unscaledTime`/
`unscaledDeltaTime` самоадаптируются к контексту вызова. Следствия:
- **`HitStopSystem` менять НЕ надо и он НЕ тронут (Этап 8).** Он событийный
  (`IInitializableSystem` + `IDisposableSystem`), тикового канала не имеет; его
  гейт `Time.unscaledTime` корректен в обоих контекстах.
- **Правка Этапа 6** (`unscaledDeltaTime` → `fixedUnscaledDeltaTime` в Dash/Slide)
  была поведенчески **no-op**: не фикс величины, а явное выражение намерения.
  Вреда нет (форма даже честнее и устойчива к смене канала), но считать её
  «исправлением бага» нельзя.

**Двойного масштабирования `timeScale` НЕТ (проверено на Этапе 8).** Ключевая
механика: `timeScale` **не меняет значение** `Time.fixedDeltaTime` (оно остаётся
0.02) — он меняет **частоту вызовов FixedUpdate** в реальном времени. Поэтому
таймеры на `fixedDeltaTime` накапливают игровое время ровно так же, как раньше
на `deltaTime`, и замирают в хитстопе идентично. Вручную на `timeScale` не
умножает никто: `timeScale` пишет только `HitStopService`, читателей-множителей
в проекте нет.

**F5 — `Trampoline` как vanilla-мост.**
Оставляем прямую запись velocity из `OnCollisionEnter2D` (после Этапа 4 она
согласована с fixed-движением) или приводим к интент/событийной модели ECS
(bounce-impulse применяется системой движения на fixed-тике)? Также CLAUDE.md
против vanilla-MonoBehaviour без явного запроса.

**РЕШЕНО:** `Trampoline` остаётся `MonoBehaviour` — ручная расстановка на уровне
это осознанный workflow-выбор геймдизайнера (явный запрос, исключение из
CLAUDE.md-правила зафиксировано). НО прямая запись velocity в обход
`CurrentMovementState` убирается: трамплин шлёт `bounce-impulse` интент/событием
в ECS, а применяет его система движения на fixed-тике (в согласии с арбитражем
владения скоростью). Реализовать на Этапе 9.

**✅ РЕАЛИЗОВАНО на Этапе 9** (`BounceImpulseRequest` + `BounceSystem`, см. отметку
этапа). Уточнение к формулировке выше: «применяет на fixed-тике» реализовано как
**немедленное применение в колбэке запроса**, а не отложенное до следующего
`OnFixedUpdate`. `OnCollisionEnter2D` уже стреляет внутри `Physics2D.Simulate`
(см. §1.7), т.е. в физ-контексте шага, и системы движения к этому моменту уже
отработали — гонки нет, а откладывание стоило бы +1 физ-шаг (20 мс) задержки, то
есть изменило бы фил. Суть F5 — убрать чужого писателя velocity — соблюдена
полностью: пишет система сущности, а не внешний `MonoBehaviour`. Развилка закрыта.

**F6 — Крюк грэпла (`GrappleHookProjectile`).**
Полёт крюка — ещё корутина. Дочищаем до fixed-тика в Этапе 7 или оставляем
(pull-фаза, которая формирует фил, уже не корутина)?

**РЕШЕНО:** летящую фазу `GrappleHookProjectile` в этом заходе НЕ трогаем —
минимальный объём Этапа 7 (мигрируем только pull-фазу, которая формирует фил).
Корутинный полёт крюка занесён в §5 как незакрытый хвост (R5) под будущий полный
детерминизм / ghost-replay.

---

## 7. Что НЕ входит в миграцию (явно)

- Стиль-decay/evaluator, баффы-таймеры, уровневый спидран-таймер, AI-решения
  (`RandomMovementState`), все `*View`/VFX/DOTween-HUD — остаются на Update
  осознанно (§1.6).
- `WindmillPhysicsHandler` — уже корректный FixedUpdate, не трогаем.
- Слайм — мёртвый код (`CreateSlime` возвращает null, кейса в `EnemiesFactory`
  нет), из скоупа миграции исключён.

---

## 8. ИТОГ МИГРАЦИИ — все 9 этапов закрыты

> Статус: **миграция завершена**. Каждый этап отдельным коммитом, каждый —
> подтверждён ручным плейтестом в Play Mode, не только компиляцией.

### 8.1 Что достигнуто

**Core-геймплей героя целиком тикает на фиксированном канале.** Запись скорости
и связанные с ней окна/тайминги идут тем же тиком, что и интеграция физики
(50 Гц). Исходный дефект из §0 — «velocity пишется из переменного Update, а
физика интегрирует на 50 Гц» — устранён для всей цепочки: базовая локомоция и
граунд-детект, прыжки (обычный/двойной/у стены/со склона), дэш и слайд, grapple
pull, plunge, slope-триада, wall-hang, glide, боевые тайминги и «скорость = урон».

**Фидл fps-инвариантен — подтверждено плейтестами 30/200 на каждом HIGH-этапе:**
длина тормозного пути, высота charged-jump, длина рывка дэша, дуга грэпла,
пороги «скорость = урон». Раньше всё это зависело от FPS. Это и был главный
критерий успеха — не «не сломалось», а «перестало зависеть от частоты кадров».

**Фундамент под детерминизм заложен.** Прогон стал воспроизводимым в той части,
что живёт на fixed-канале, — это база под ghost-replay и лидерборды из
`gameplay_design.md`. **Полного детерминизма пока НЕТ:** мешает R5 (крюк грэпла
на корутине) и R7 (детект контактов снарядов на Update). Оба — известные хвосты,
см. §8.3.

**Не-физика осознанно осталась на Update** (§1.6): стиль-decay, баффы, уровневый
таймер, AI-решения, все `*View`/VFX. Переносить их незачем — это затянуло бы их
под `timeScale`-хитстоп и добавило риска на пустом месте.

**Механизм** — двухканальный тик (F1): `IFixedUpdatableSystem` рядом с
`IUpdatableSystem`, `Entity` роутит систему по интерфейсу при регистрации.
Поэтапность обеспечил арбитраж `CurrentMovementState`. Big-bang отклонён и не
понадобился.

### 8.2 Ключевые архитектурные дочистки, вскрывшиеся по ходу

Миграция сработала как рентген: перенос на fixed вскрыл давние долги, которые на
переменном тике маскировались last-writer-wins-случайностью.

- **R2 — гейт single-writer в базе** (`5433c4e3`). `RigidbodyMovementSystem`
  писала velocity параллельно с dash/slide/plunge; держалось «случайно» на общем
  Update-тике. Теперь база явно уступает, пока скоростью владеет другая система.
- **Y-guard базы на склоне** (`6012ab95`). База затирала восходящую Y, выданную
  чужой системой (прыжок/wall-jump/трамплин), тангентной записью по склону.
  Теперь не опускает Y, которой не владеет. Guard читает живую velocity —
  поэтому он же бесплатно закрыл вертикаль отскока трамплина на Этапе 9.
- **`Trampoline` → event-модель** (`3e346584`, F5). Внешний `MonoBehaviour` писал
  velocity в чужой rigidbody мимо арбитража. Теперь шлёт `BounceImpulseRequest`,
  применяет `BounceSystem` героя. Долг снят без изменения фила.
- **Корутины сняты с фил-критичных путей**: `ResetUsingFlag` (Этап 1),
  `DashCoroutine`/`SlideCoroutine` (Этап 6) → явные fixed-таймеры.

**Дважды прогноз о поломке не подтвердился плейтестом** — и оба раза корень был
один: вывод строился на «может случиться X» без проверки числа, которое решает,
случится ли X вообще. Слоуп-прыжок из слайда выживает структурно (каст 0.15 м
против подъёма 0.5–1+ м за физшаг, см. отметку Этапа 7); дэш сквозь врага на
30 fps не смазался (R7). **Урок на будущее: проверять решающее число, а не
рассуждать о механизме.**

### 8.3 Явный технический долг — отдельные будущие заходы, НЕ блокеры

Ни один из пунктов не блокирует релиз и не ломает текущий фил. Все
зафиксированы в §5 с обоснованием и готовым путём решения.

| Долг | Суть | Когда браться |
|---|---|---|
| **R5** | Летящая фаза `GrappleHookProjectile` — корутина (Update-частота). Сознательный недетерминизм (F6). | Когда детерминизм станет активной целью (ghost-replay) |
| **R7** | Детект контактов снарядов/врагов на Update. Перенос требует тащить `DealDamageOnContact` + `DeathMaskTouchDetector` + `TransformMovementSystem` снарядов, иначе сюрикен проскочит врага. `FinalPointTriggerService` не уедет вовсе — у сервисов нет fixed-канала. | Отдельный этап со своим плейтестом (урон врагов, слэш, сюрикен) |
| **R8** | `ProjectileFactory:83-84` — filter зарегистрирован раньше detect, буфер слэш-снаряда протух на тик. **Пред-существующий баг**, миграцией не создан. Фикс — поменять строки местами. | Вместе с R7 или отдельно, нужен плейтест слэш-атаки |
| **R9** | Наклонные батуты: боковой импульс съест база. Путь — lockout по образцу wall-jump. | Только если появятся повёрнутые батуты |
| **R6** | `SlopeSlideSystem` без Y-guard. Сейчас безвредно, но держится на числах конфига (`targetY > ~7.5`), а не на инварианте. **Мина:** уронишь силу слоуп-прыжка вдвое — прыжок начнёт «клевать», симптом укажет на прыжок, а не на слайд. | Страховка; либо при тюнинге слоуп-прыжка |

**Инфраструктурный хвост — `EntityAPIGenerator` bootstrap-тупик.** Генератор
читает `Assembly-CSharp` рефлексией, поэтому при добавлении **любого** нового
`IEntityComponent` возникает замкнутый круг: сборка не проходит из-за ссылок на
ещё не сгенерированный API → генератор не видит новый компонент. Рабочий порядок
(проверен на Этапе 9): временно перевести обращения на несгенерированную форму
(`GetComponent<T>().Value` / `AddComponent(...)`) → компиляция → **ручной**
`Tools/GenerateEntityAPI` → вернуть сгенерированную форму → компиляция. На
автозапуск `[InitializeOnLoadMethod]` не полагаться (known risk из CLAUDE.md;
цель — оставить только ручной `[MenuItem]`).

### 8.4 Финальный прогон — приёмка целостности (не код)

Цель: убедиться, что **стильные связки воспроизводятся одинаково по таймингам
между FPS**. Отдельные механики уже проверены поэтапно — здесь проверяются
переходы между ними.

**Условие корректного замера:** `targetFrameRate` жёстко залочен на каждом
прогоне. Свободный FPS маскирует дрейф под регрессию — см. диагностический
эпизод Этапа 2. Прогон на **30 / 60 / 144 / 200**, 2–3 уровня.

Чек-лист связок:

1. **dash → grapple** — дэш, из него сразу крюк. Инерция дэша переходит в тягу?
2. **grapple → ранний отпуск → plunge** — дуга сохраняется при раннем отпуске,
   из верхней точки пике. Стык pull-фазы (fixed) и летящего крюка (корутина, R5)
   — единственное место, где fps-зависимость ещё возможна по дизайну.
3. **plunge → импакт-урон → воздушная атака** — приземление, урон по площади,
   джаггл врага в воздухе.
4. **dash сквозь врага → атака в воздухе** — «скорость = урон» на порогах 10/20,
   затем хитстоп. Длина заморозки одинакова между FPS?
5. **plunge → трамплин → воздушная атака** — отскок после пике (кап
   `_maxBounceForce`), из отскока удар.
6. **slope-slide → slope-jump → glide** — слоуп-прыжок из зажатого слайда
   (см. R6), из него планирование.
7. **wall-hang → wall-jump → dash** — серия по двум стенам с выходом в дэш.

**Критерий приёмки:** одна и та же последовательность входов даёт субъективно
одинаковый результат по таймингам и дистанциям на 30/60/144/200. Расхождение —
регрессия, а не «особенность железа».

**Опционально, как задел под ghost-replay:** заготовка детерминизм-теста
(одинаковый вход → одинаковая траектория). Помнить, что R5 и R7 пока делают
полный детерминизм недостижимым — тест покажет расхождение именно на них.
