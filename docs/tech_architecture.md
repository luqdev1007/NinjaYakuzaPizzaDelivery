# NYPD — Tech Architecture

> Статус: v0.1, собрано по итогам разбора реального кода (DI, ECS-lite, Reactive, MVP UI).

## Стек
Unity 2D, DOTween, Cinemachine + самописное: DI-контейнер, ECS-lite (Entity = компоненты+системы), реактивные примитивы, MVP для UI.

## DI-контейнер
- Иерархический (`DIContainer` с `_parent` fallback) — project-level контейнер (глобальные сервисы) → gameplay scene-level контейнер (level-сессионные системы). Соответствует паттерну Project/Scene scope (как в Zenject).
- Только singleton lifetime, lazy по умолчанию, `.NonLazy()` для eager-инициализации.
- Cycle-detection при `Resolve<T>()` — даёт читаемую ошибку вместо StackOverflow.
- Массовое создание игровых сущностей (враги, проджектайлы, герой) идёт НЕ через DI lifetime, а через отдельные `*Factory`-классы — чистое разделение ответственности.

**Известный гэп**: `IInitializable.Initialize()` вызывается автоматически только для `.NonLazy()`-регистраций (через `container.Initialize()` на старте). Lazy-сервисы, резолвящиеся позже через `Resolve<T>()`, не получают автоматический вызов `Initialize()`. Нужно подтвердить, что это намеренное поведение.

## ECS-lite (Entity/Component/System)
- `Entity` — контейнер компонентов (`Dictionary<Type, IEntityComponent>`) + собственный список систем (`_systems`/`_initializables`/`_updatables`/`_disposables`). Не классический DOTS-ECS (нет batch-обработки по архетипам) — больше похоже на per-instance композицию поведения, что оправдано при текущем масштабе сущностей (короткие уровни, не тысячи одновременных юнитов).
- `EntityAPIGenerator` — Editor-тул на reflection, генерирует fluent API (`entity.AddX(...)`, геттеры, `TryGetX()`) по всем `IEntityComponent`-типам в сборке. Удобный, но рискованный в `[InitializeOnLoadMethod]`-режиме (гоняет reflection на каждый домен-релоад). **Рекомендация**: оставить только ручной `[MenuItem]`-запуск.
- `Conditions` (`ICompositeCondition`/`FuncCondition`/`CompositeCondition`) — декларативные guard-условия (`CanJump`, `CanDash` и т.д.), отделённые от систем, которые их используют. Чистый паттерн.
- `MonoEntity`/`EntityView`/`MonoEntityRegistrator` — адаптер между Unity GameObject-миром и абстрактным Entity.
- `EntitiesLifeContext.Update()` оборачивает обновление каждой entity в try/catch с логированием — не даёт одной сломанной entity подвесить весь уровень.

**Критическая находка**: основной игровой луп (`GameplayBootstrap.Update()` → `_entitiesLifeContext.Update(Time.deltaTime)`) работает на **переменном таймстепе** (`Update()`, не `FixedUpdate()`), а ряд механик (`DashSystem`, `SlideSystem`, `InventorySystem.ResetUsingFlag`) использует Unity-корутины с `WaitForSeconds` для таймингов. Это несовместимо с детерминированной симуляцией, нужной для ghost-replay/лидербордов/будущего редактора уровней. **Рекомендация**: перевести основной геймплейный update на фиксированный таймстеп, заменить coroutine-таймауты на явные `ReactiveVariable<float>`-таймеры, тикающиеся в `OnUpdate(deltaTime)` — паттерн уже используется в проекте (`AttackProcessTimerSystem` и т.д.), нужно применить его последовательно везде, где сейчас стоят корутины.

## Реактивные примитивы
- `ReactiveVariable<T>` / `ReactiveEvent<T>` / `ReactiveEvent` — equality-check перед инвоком (не шлёт лишние уведомления) и buffered add/remove подписчиков (`_toAdd`/`_toRemove`), что корректно защищает от модификации коллекции во время итерации — реализовано верно.
- Поведенческая особенность (не баг): отписка во время текущего `Invoke()` применяется только со следующего вызова.

## MVP для UI
- `IView`/`IPresenter` базовые интерфейсы, `PopupViewBase`/`PopupPresenterBase` — общий паттерн для попапов с DOTween-анимациями показа/скрытия и event-цепочкой View → Presenter → Service.
- `PopupService` (абстрактный, с `MainMenuPopupService`/`GameplayPopupService`-наследниками под разные UI-root'ы) — трекает открытые попапы, корректно закрывает/диспозит.
- `ViewsFactory` — создание View по string ID через словарь путей в Resources.
- **Образцовый пример reactive+MVP**: `CurrencyPresenter`/`WalletPresenter` — Presenter подписывается на `IReadOnlyVariable<int>`, синхронизирует начальное значение до подписки, корректно отписывается в `Dispose()`. Использовать как референс для HUD-элементов геймплея (HP-бар, стиль-метр, таймер уровня).

**Несоответствие, стоит решить до развития додзё**: `Dojo`/`Shop`/`LeaderBoard` в `MainMenuScreenPresenter` управляются напрямую через `.gameObject.SetActive(true/false)`, в отличие от попапов с полноценным Presenter+Service+анимации+lifecycle. Додзё — хаб с минигеймами (кухня, тренировка, найм курьеров из metagame_design.md/killer_features.md), которому нужна логика и состояние — стоит завести ему отдельный Presenter по образцу попапов прежде, чем наполнять контентом.

## Технический долг / находки при разборе
- Три отдельных leftover `throw new NotImplementedException()`-заглушки найдены в разных файлах (`Entity.AddWallMask`, `ReactiveEvent.Invoke(object)`, `ViewsFactory.Create(object hintView, Transform)`) — рекомендуется прогнать project-wide поиск по `NotImplementedException` и почистить.
- `[InitializeOnLoadMethod]` на `EntityAPIGenerator.Generate()` — риск медленных/нестабильных домен-релоадов при росте числа компонентов.
