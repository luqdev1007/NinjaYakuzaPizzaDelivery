using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Tongue
{
    /// <summary>
    /// Язык слайма: поиск цели в арке вверх → телеграф → выстрел с упреждением →
    /// полёт → втягивание → кулдаун отращивания.
    ///
    /// ЖИВЁТ НА СЛАЙМЕ. Собственный приватный enum состояния (по образцу
    /// GrappleSystem), стейт-машина мозга здесь НЕ используется: мозг продолжает
    /// «хотеть» идти к точке маршрута, а движение гасится условием CanMove по
    /// флагу IsTongueActive. Это штатный приём проекта — мозг решает, системы
    /// исполняют, условия гасят.
    ///
    /// SINGLE-WRITER. Система пишет три вещи: IsTongueActive и
    /// IsTongueTelegraphing на СВОЕЙ сущности и TongueOriginPoint на сущности
    /// СОЗДАННОГО ЕЮ ЯЗЫКА. Последнее выглядит как запись «в чужую» сущность, но
    /// писатель у этого компонента по-прежнему ровно один: язык создаётся,
    /// ведётся и освобождается исключительно этой системой, других писателей у
    /// TongueOriginPoint нет. Копии компонента на слайме нет намеренно —
    /// единственный читатель живёт на префабе языка (TongueView).
    ///
    /// ЭТАП 3a: попадание в героя = язык коснулся и втянулся. Притягивания,
    /// урона и очков стиля здесь нет — это 3b.
    ///
    /// ПРО ОСВОБОЖДЕНИЕ ЯЗЫКА. EntitiesLifeContext.Release только кладёт заявку
    /// в очередь, а очередь сливается в конце Update — тогда как эта система
    /// тикается на fixed. Между Release и фактическим удалением сущность языка
    /// проживёт ещё несколько fixed-тиков с ЖИВЫМ и всё ещё
    /// ЗАРЕГИСТРИРОВАННЫМ коллайдером. Поэтому поведение гасится немедленно
    /// сбросом состояния, ссылка обнуляется сразу, и на мгновенное исчезновение
    /// объекта никто не рассчитывает.
    /// </summary>
    public class TongueSystem : IInitializableSystem, IFixedUpdatableSystem, IDisposableSystem
    {
        private enum TongueState
        {
            Idle,
            Telegraph,
            Flying,
            Retracting,
            Cooldown
        }

        // Ниже этого порога вектор считается вырожденным и направление берётся
        // запасное — иначе normalized вернёт ноль и язык улетит «в никуда».
        private const float DegenerateVectorThreshold = 0.0001f;

        // Потолок хитов за один каст. Буфер переиспользуется, а не создаётся
        // каждый тик: ровно та претензия, что висит к SurfaceCheckSystem, —
        // повторять её не будем.
        private const int CastHitsBufferSize = 16;

        // ==================== ВРЕМЕННАЯ ДИАГНОСТИКА (этап 3a) ====================
        // Удаляется целиком по префиксу TONGUE-DIAG. Логика условий не затронута:
        // диагностика только ЧИТАЕТ те же величины и печатает их раз в секунду.
        private const string DiagnosticsPrefix = "[TONGUE-DIAG]";
        private const float DiagnosticsInterval = 1f;

        private float _nextDiagnosticsTime;
        private bool _isDiagnosticsTick;
        // =========================================================================

        private readonly ProjectileFactory _projectileFactory;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly MainHeroHolderService _mainHeroHolderService;

        private Transform _transform;
        private Transform _shootPoint;

        private ReactiveVariable<bool> _isTongueActive;
        private ReactiveVariable<bool> _isTongueTelegraphing;
        private ReactiveVariable<bool> _isDead;

        private float _range;
        private float _speed;
        private float _retractSpeed;
        private float _telegraphDuration;
        private float _cooldown;
        private float _aimArcDot;

        private LayerMask _sightBlockMask;
        private LayerMask _targetMask;
        private string _prefabPath;

        // Переиспользуемые буфер и фильтры кастов. Оба каста системы (линия
        // взгляда и полёт языка) идут по одному буферу — они никогда не
        // выполняются в одном тике, поэтому за перетирание результатов можно
        // не переживать.
        private readonly RaycastHit2D[] _castHits = new RaycastHit2D[CastHitsBufferSize];
        private ContactFilter2D _sightFilter;
        private ContactFilter2D _flightFilter;

        private TongueState _state;

        // Таймеры держим приватными полями системы, а не компонентами: читателей
        // снаружи у них нет, а заводить реактивку без подписчика — против
        // правила реактивности проекта (см. шапку TongueComponents).
        private float _telegraphTimer;
        private float _cooldownTimer;

        private Entity _tongueEntity;
        private IDisposable _tongueDamageDisposable;
        private IDisposable _isDeadDisposable;

        // Помечает, что заявка на Release уже подана. Нужен именно флаг, а не
        // проверка ссылки: в окне до фактического удаления язык ещё можно
        // разрубить, и этот удар обязан уйти в никуда.
        private bool _isTongueReleased;

        private Vector2 _tipPosition;
        private Vector2 _flyDirection;
        private float _traveledDistance;

        public TongueSystem(
            ProjectileFactory projectileFactory,
            EntitiesLifeContext entitiesLifeContext,
            MainHeroHolderService mainHeroHolderService)
        {
            _projectileFactory = projectileFactory;
            _entitiesLifeContext = entitiesLifeContext;
            _mainHeroHolderService = mainHeroHolderService;
        }

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _shootPoint = entity.ShootPoint;

            _isTongueActive = entity.IsTongueActive;
            _isTongueTelegraphing = entity.IsTongueTelegraphing;
            _isDead = entity.IsDead;

            _range = entity.TongueRange;
            _speed = entity.TongueSpeed;
            _retractSpeed = entity.TongueRetractSpeed;
            _telegraphDuration = entity.TongueTelegraphDuration;
            _cooldown = entity.TongueCooldown;
            _aimArcDot = entity.TongueAimArcDot;

            _sightBlockMask = entity.SightBlockMask;
            _targetMask = entity.TargetMask;
            _prefabPath = entity.TonguePrefabPath;

            // useTriggers = true сохраняет прежнее поведение: глобальный
            // Physics2D.queriesHitTriggers в проекте включён, и одиночный
            // Linecast, который здесь стоял раньше, триггеры видел.
            _sightFilter = new ContactFilter2D();
            _sightFilter.useTriggers = true;
            _sightFilter.SetLayerMask(_sightBlockMask);

            _flightFilter = new ContactFilter2D();
            _flightFilter.useTriggers = true;
            _flightFilter.SetLayerMask(_sightBlockMask | _targetMask);

            _state = TongueState.Idle;

            // Смерть слайма с активным языком — обязательный сценарий: бесхозный
            // язык оставил бы свой коллайдер в CollidersRegistryService, и тот
            // вечно ловился бы атаками.
            _isDeadDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }

        public void OnFixedUpdate(float deltaTime)
        {
            // TONGUE-DIAG: троттлинг общий на весь тик — одна строка в секунду.
            _isDiagnosticsTick = Time.unscaledTime >= _nextDiagnosticsTime;

            if (_isDiagnosticsTick)
            {
                _nextDiagnosticsTime = Time.unscaledTime + DiagnosticsInterval;
            }

            if (_isDead.Value)
            {
                LogDiagnostics("ранний выход: IsDead = true");
                return;
            }

            if (_state != TongueState.Idle)
            {
                LogDiagnostics($"состояние = {_state} (не Idle, поиск цели не выполняется), " +
                               $"cooldownTimer = {_cooldownTimer:F2}, telegraphTimer = {_telegraphTimer:F2}");
            }

            switch (_state)
            {
                case TongueState.Idle:
                    TickIdle();
                    break;

                case TongueState.Telegraph:
                    TickTelegraph(deltaTime);
                    break;

                case TongueState.Flying:
                    TickFlying(deltaTime);
                    break;

                case TongueState.Retracting:
                    TickRetracting(deltaTime);
                    break;

                case TongueState.Cooldown:
                    TickCooldown(deltaTime);
                    break;
            }
        }

        public void OnDispose()
        {
            _isDeadDisposable?.Dispose();
            _isDeadDisposable = null;

            CancelAll();
        }

        // — Поиск цели —

        private void TickIdle()
        {
            LogIdleDiagnostics();

            Entity hero = ResolveHero();

            if (hero == null)
            {
                return;
            }

            Vector2 origin = GetOrigin();
            Vector2 heroPosition = hero.Transform.position;
            Vector2 toHero = heroPosition - origin;

            float distance = toHero.magnitude;

            if (distance > _range)
            {
                return;
            }

            if (distance < DegenerateVectorThreshold)
            {
                return;
            }

            // Арка ~180° вверх. Вниз слайм не стреляет никогда.
            if (Vector2.Dot(toHero / distance, Vector2.up) < _aimArcDot)
            {
                return;
            }

            if (TryFindBlockingHit(origin, heroPosition, _sightFilter, out RaycastHit2D _))
            {
                return;
            }

            EnterTelegraph();
        }

        /// <summary>
        /// Ссылка на героя берётся КАЖДЫЙ ТИК и не кэшируется в конструкторе:
        /// враги создаются раньше героя, там ещё null. Проверок две, обе
        /// обязательны — сервис не обнуляет ссылку после смерти героя, поэтому
        /// сама Entity остаётся живым C#-объектом, а вот её Transform к тому
        /// моменту уже уничтожен.
        /// </summary>
        private Entity ResolveHero()
        {
            if (_mainHeroHolderService == null)
            {
                return null;
            }

            Entity hero = _mainHeroHolderService.MainHero;

            if (hero == null)
            {
                return null;
            }

            if (hero.Transform == null)
            {
                return null;
            }

            return hero;
        }

        /// <summary>
        /// Неаллоцирующий каст с отсевом ОХВАТЫВАЮЩИХ коллайдеров.
        ///
        /// Хит считается препятствием, только если точка старта НЕ лежит внутри
        /// задевшего коллайдера. Коллайдеры вроде LevelBounds охватывают всю
        /// игровую зону и потому всегда содержат стреляющего, физически не
        /// являясь преградой между ним и целью. При
        /// Physics2D.queriesStartInColliders = 1 такой коллайдер возвращается
        /// как хит с дистанцией 0 и намертво глушит любую проверку видимости.
        ///
        /// Глобальный queriesStartInColliders СПЕЦИАЛЬНО не трогаем: его
        /// мутация посреди кадра задела бы любые другие касты в том же тике.
        ///
        /// Возвращается БЛИЖАЙШИЙ валидный хит: порядок результатов в буфере
        /// не гарантирован, поэтому минимум по дистанции ищем явно.
        /// </summary>
        private bool TryFindBlockingHit(Vector2 start, Vector2 end, ContactFilter2D filter, out RaycastHit2D blockingHit)
        {
            blockingHit = default;

            int count = Physics2D.Linecast(start, end, filter, _castHits);
            float closestDistance = float.MaxValue;
            bool found = false;

            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = _castHits[i];

                if (hit.collider == null)
                {
                    continue;
                }

                // Охватывающий коллайдер — не преграда.
                if (hit.collider.OverlapPoint(start))
                {
                    continue;
                }

                if (hit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hit.distance;
                blockingHit = hit;
                found = true;
            }

            return found;
        }

        private Vector2 GetOrigin()
        {
            // Точка вылета берётся через transform слайма, а не через ручной знак
            // направления: FlipDirectionSystem крутит localRotation по Y на 0/180,
            // дочерние узлы зеркалятся вместе с родителем, поэтому мировая
            // позиция ShootPoint корректна при любом фейсинге.
            if (_shootPoint != null)
            {
                return _shootPoint.position;
            }

            return _transform.position;
        }

        // — Телеграф —

        private void EnterTelegraph()
        {
            _state = TongueState.Telegraph;
            _telegraphTimer = _telegraphDuration;

            _isTongueActive.Value = true;
            _isTongueTelegraphing.Value = true;
        }

        private void TickTelegraph(float deltaTime)
        {
            _telegraphTimer -= deltaTime;

            if (_telegraphTimer > 0f)
            {
                return;
            }

            // Условия входа ПОВТОРНО НЕ ПРОВЕРЯЮТСЯ: если герой ушёл, выстрел всё
            // равно состоится. Промах — честная награда игроку за реакцию.
            _isTongueTelegraphing.Value = false;

            Shoot();
        }

        // — Выстрел —

        /// <summary>
        /// Направление считается ИМЕННО ЗДЕСЬ, а не в начале телеграфа: иначе
        /// смена направления во время windup ничего не давала бы и скилл-чек
        /// исчезал.
        /// </summary>
        private void Shoot()
        {
            Vector2 origin = GetOrigin();

            _flyDirection = ResolveShootDirection(origin);
            _tipPosition = origin;
            _traveledDistance = 0f;

            _tongueEntity = _projectileFactory.CreateSlimeTongue(origin, _prefabPath);
            _isTongueReleased = false;

            _tongueDamageDisposable = _tongueEntity.TakeDamageRequest.Subscribe(OnTongueDamaged);

            _state = TongueState.Flying;
        }

        /// <summary>
        /// Упреждение РОВНО В ОДНУ ИТЕРАЦИЮ. Рекурсивное уточнение точки встречи
        /// даёт почти неизбегаемое попадание и убивает скилл-чек.
        /// </summary>
        private Vector2 ResolveShootDirection(Vector2 origin)
        {
            Entity hero = ResolveHero();

            if (hero == null)
            {
                return ClampToArc(_transform.right);
            }

            Vector2 heroPosition = hero.Transform.position;

            // Скорость героя читается напрямую из Rigidbody2D: готового
            // компонента-вектора скорости в проекте нет, компонент Velocity
            // мёртвый и переиспользовать его нельзя.
            Vector2 heroVelocity = Vector2.zero;

            if (hero.Rigidbody != null)
            {
                heroVelocity = hero.Rigidbody.linearVelocity;
            }

            float distance = Vector2.Distance(heroPosition, origin);
            float timeToTarget = 0f;

            if (_speed > DegenerateVectorThreshold)
            {
                timeToTarget = distance / _speed;
            }

            Vector2 aimPoint = heroPosition + heroVelocity * timeToTarget;
            Vector2 direction = aimPoint - origin;

            if (direction.sqrMagnitude < DegenerateVectorThreshold * DegenerateVectorThreshold)
            {
                return ClampToArc(_transform.right);
            }

            return ClampToArc(direction.normalized);
        }

        /// <summary>
        /// Если упреждение вынесло точку прицела за арку вниз — направление
        /// клампится к границе арки, а не стреляет в землю. Горизонтальный знак
        /// сохраняется.
        /// </summary>
        private Vector2 ClampToArc(Vector2 direction)
        {
            Vector2 normalized = direction.normalized;

            if (Vector2.Dot(normalized, Vector2.up) >= _aimArcDot)
            {
                return normalized;
            }

            float boundaryDot = Mathf.Clamp(_aimArcDot, -1f, 1f);
            float horizontal = Mathf.Sqrt(Mathf.Max(0f, 1f - boundaryDot * boundaryDot));

            float sign;

            if (Mathf.Abs(normalized.x) < DegenerateVectorThreshold)
            {
                // Строго вниз — горизонтального знака в векторе нет, берём фейсинг.
                sign = Mathf.Sign(_transform.right.x);
            }
            else
            {
                sign = Mathf.Sign(normalized.x);
            }

            return new Vector2(horizontal * sign, boundaryDot);
        }

        // — Полёт —

        private void TickFlying(float deltaTime)
        {
            if (_tongueEntity == null)
            {
                CancelAll();
                EnterCooldown();
                return;
            }

            Vector2 origin = GetOrigin();
            UpdateTongueOrigin(origin);

            Vector2 newTip = _tipPosition + _flyDirection * _speed * deltaTime;

            // Та же фильтрация охватывающих коллайдеров, что и у линии взгляда:
            // без неё язык втыкался бы в LevelBounds на первом же тике.
            if (TryFindBlockingHit(_tipPosition, newTip, _flightFilter, out RaycastHit2D hit))
            {
                // ЭТАП 3a: попадание в героя и попадание в препятствие
                // обрабатываются одинаково — язык коснулся и втягивается.
                // ТОЧКА ВЕТВЛЕНИЯ ДЛЯ 3b: здесь развилка по слою hit.collider —
                // Characters уходит в захват, остальное во втягивание.
                _tipPosition = hit.point;
                ApplyTipPosition();
                EnterRetracting();
                return;
            }

            _traveledDistance += Vector2.Distance(_tipPosition, newTip);
            _tipPosition = newTip;
            ApplyTipPosition();

            if (_traveledDistance >= _range)
            {
                EnterRetracting();
            }
        }

        // — Втягивание —

        private void EnterRetracting()
        {
            _state = TongueState.Retracting;
        }

        private void TickRetracting(float deltaTime)
        {
            if (_tongueEntity == null)
            {
                CancelAll();
                EnterCooldown();
                return;
            }

            Vector2 origin = GetOrigin();
            UpdateTongueOrigin(origin);

            Vector2 toOrigin = origin - _tipPosition;
            float distance = toOrigin.magnitude;
            float step = _retractSpeed * deltaTime;

            if (distance <= step || distance < DegenerateVectorThreshold)
            {
                _tipPosition = origin;
                ApplyTipPosition();

                CancelAll();
                EnterCooldown();
                return;
            }

            _tipPosition += toOrigin / distance * step;
            ApplyTipPosition();
        }

        /// <summary>
        /// Разруб катаной. В 3a визуально то же втягивание; отдельное поведение
        /// «обрубок» — за скоупом этапа.
        /// </summary>
        private void OnTongueDamaged(DamageData damageData)
        {
            // Удар мог прилететь в окне между Release и фактическим удалением —
            // такой обязан уйти в никуда, а не в уже мёртвую логику.
            if (_isTongueReleased)
            {
                return;
            }

            if (_state != TongueState.Flying)
            {
                return;
            }

            EnterRetracting();
        }

        // — Кулдаун —

        private void EnterCooldown()
        {
            _state = TongueState.Cooldown;
            _cooldownTimer = _cooldown;
        }

        private void TickCooldown(float deltaTime)
        {
            _cooldownTimer -= deltaTime;

            if (_cooldownTimer > 0f)
            {
                return;
            }

            _cooldownTimer = 0f;
            _state = TongueState.Idle;
        }

        // — Работа с сущностью языка —

        private void UpdateTongueOrigin(Vector2 origin)
        {
            // Слайм мог быть сдвинут тараном уже после выстрела, поэтому корень
            // языка переписывается каждый тик, а не фиксируется на старте.
            _tongueEntity.TongueOriginPoint.Value = origin;
        }

        private void ApplyTipPosition()
        {
            if (_tongueEntity.Transform == null)
            {
                return;
            }

            _tongueEntity.Transform.position = _tipPosition;
        }

        /// <summary>
        /// Централизованный cleanup (по образцу GrappleSystem.CancelAll).
        /// Обязан отработать при разрубе, завершении втягивания, смерти слайма и
        /// OnDispose системы. Идемпотентен: повторный вызов не подаст вторую
        /// заявку на Release — двойной Release дал бы двойной Dispose сущности и
        /// повторный прогон всех её OnDispose.
        /// </summary>
        private void CancelAll()
        {
            _tongueDamageDisposable?.Dispose();
            _tongueDamageDisposable = null;

            if (_tongueEntity != null && _isTongueReleased == false)
            {
                _isTongueReleased = true;
                _entitiesLifeContext.Release(_tongueEntity);
            }

            _tongueEntity = null;

            _isTongueActive.Value = false;
            _isTongueTelegraphing.Value = false;

            _telegraphTimer = 0f;
            _traveledDistance = 0f;

            _state = TongueState.Idle;
        }

        private void OnIsDeadChanged(bool oldValue, bool value)
        {
            if (value == false)
            {
                return;
            }

            CancelAll();
        }

        // ==================== ВРЕМЕННАЯ ДИАГНОСТИКА (этап 3a) ====================
        // Всё ниже удаляется целиком. Ни один метод здесь не пишет состояние —
        // только читает и печатает.

        private void LogDiagnostics(string message)
        {
            if (_isDiagnosticsTick == false)
            {
                return;
            }

            Debug.Log($"{DiagnosticsPrefix} {message}");
        }

        /// <summary>
        /// Пересчитывает ровно те же величины, что проверяет TickIdle, и печатает
        /// их одной строкой. Логика TickIdle не тронута — это независимый
        /// read-only проход, включая собственный Linecast (раз в секунду).
        /// </summary>
        private void LogIdleDiagnostics()
        {
            if (_isDiagnosticsTick == false)
            {
                return;
            }

            Vector2 origin = GetOrigin();
            string originInfo = $"shootPointSet = {_shootPoint != null}, origin = {origin}";

            if (_mainHeroHolderService == null)
            {
                LogDiagnostics($"герой НЕ найден: MainHeroHolderService = null. {originInfo}");
                return;
            }

            Entity hero = _mainHeroHolderService.MainHero;

            if (hero == null)
            {
                LogDiagnostics($"герой НЕ найден: MainHero = null (герой ещё не создан). {originInfo}");
                return;
            }

            if (hero.Transform == null)
            {
                LogDiagnostics($"герой НЕ найден: hero.Transform уничтожен. {originInfo}");
                return;
            }

            Vector2 heroPosition = hero.Transform.position;
            Vector2 toHero = heroPosition - origin;
            float distance = toHero.magnitude;

            float dot = 0f;

            if (distance >= DegenerateVectorThreshold)
            {
                dot = Vector2.Dot(toHero / distance, Vector2.up);
            }

            // Тот же путь, что и в TickIdle, — с отсевом охватывающих
            // коллайдеров. Иначе лог показывал бы LevelBounds и после фикса.
            string sightInfo;

            if (TryFindBlockingHit(origin, heroPosition, _sightFilter, out RaycastHit2D hit))
            {
                int layer = hit.collider.gameObject.layer;

                sightInfo = $"ПЕРЕКРЫТА коллайдером '{hit.collider.name}', " +
                            $"слой = {LayerMask.LayerToName(layer)}({layer}), " +
                            $"дистанция хита = {hit.distance:F3}";
            }
            else
            {
                sightInfo = "свободна";
            }

            LogDiagnostics(
                $"{originInfo}, heroPos = {heroPosition} | " +
                $"дистанция = {distance:F2} / range = {_range:F2} " +
                $"({(distance <= _range ? "ok" : "ДАЛЕКО")}) | " +
                $"dot = {dot:F3} / arcDot = {_aimArcDot:F3} " +
                $"({(dot >= _aimArcDot ? "ok" : "ВНЕ АРКИ")}) | " +
                $"линия взгляда: {sightInfo} | " +
                $"cooldownTimer = {_cooldownTimer:F2}, sightBlockMask = {_sightBlockMask.value}");
        }

        // ================== КОНЕЦ ВРЕМЕННОЙ ДИАГНОСТИКИ ==========================
    }
}
