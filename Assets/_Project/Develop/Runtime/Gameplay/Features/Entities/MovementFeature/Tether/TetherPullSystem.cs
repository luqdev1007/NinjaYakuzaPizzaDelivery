using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Tether
{
    /// <summary>
    /// Притяг героя языком слайма. Живёт НА ГЕРОЕ и владеет его rigidbody —
    /// слайм только шлёт запросы (схема BounceImpulseRequest).
    /// </summary>
    /// <remarks>
    /// ═══════════════════════════════════════════════════════════════════
    ///  ЧАСТИЧНЫЙ КОНТРОЛЬ ДЕРЖИТСЯ НА ОТСУТСТВИИ СТРОКИ В ЧУЖОМ ГЕЙТЕ
    /// ═══════════════════════════════════════════════════════════════════
    /// Это главное, что нужно понять перед правкой этого файла.
    ///
    /// RigidbodyMovementSystem имеет собственный single-writer гейт:
    ///     if (_isDashing || _isSliding || _isPlunging) return;
    /// IsTethered В ЭТОТ ГЕЙТ НЕ ДОБАВЛЕН, И ДОБАВЛЯТЬ ЕГО НЕЛЬЗЯ.
    /// Именно поэтому во время захвата база продолжает вести X по вводу
    /// игрока, а тяга накладывается ПОВЕРХ — герой сохраняет возможность
    /// сопротивляться и доворачивать. Добавь IsTethered в тот гейт — и
    /// захват мгновенно превратится в полную потерю управления, то есть
    /// ровно в то, чего фича избегает.
    ///
    /// КАК ЭТО ФИЗИЧЕСКИ РАБОТАЕТ, честно и без прикрас. База каждый
    /// fixed-тик ПЕРЕЗАПИСЫВАЕТ X своим накопленным _currentSpeedX
    /// (RigidbodyMovementSystem.ApplyVelocity), то есть горизонталь тяги
    /// она бы затирала. Спасает ресинк в её же начале:
    ///     if (Mathf.Abs(rb.linearVelocity.x - _currentSpeedX) > 0.5f)
    ///         _currentSpeedX = rb.linearVelocity.x;
    /// Когда тяга успевает изменить X больше чем на 0.5, база принимает
    /// чужое значение за своё и продолжает от него. Так что связка живёт
    /// на ПОРОГЕ 0.5, а не на честном сложении сил. GrappleSystem работает
    /// ровно на этом же механизме — здесь сознательно скопирована её схема,
    /// а не изобретена своя. Практическое следствие: тяга слабее ~0.5 ед/тик
    /// по X будет съедена базой, поэтому PullAcceleration нельзя занижать
    /// «на глазок».
    ///
    /// ═══════════════════════════════════════════════════════════════════
    ///  КАНАЛ
    /// ═══════════════════════════════════════════════════════════════════
    /// Колбэк TetherRequest НЕ применяет тягу немедленно — он только взводит
    /// внутреннее состояние. Вся работа со скоростью идёт в OnFixedUpdate.
    /// Запрос приходит из fixed-системы слайма (TongueSystem), применение
    /// тоже на fixed, поэтому задержка максимум один тик и плавающей на кадр
    /// рассинхронизации не возникает. Это осознанное отличие от BounceSystem:
    /// та применяет прямо в колбэке, потому что её источник (трамплин) стреляет
    /// из OnCollisionEnter2D внутри Physics2D.Simulate, а не из ECS-тика.
    ///
    /// ═══════════════════════════════════════════════════════════════════
    ///  GRAVITYSCALE НЕ ТРОГАЕТСЯ ВООБЩЕ
    /// ═══════════════════════════════════════════════════════════════════
    /// GrappleSystem снимает BaseGravityScale в OnInit и восстанавливает его
    /// в КАЖДОМ своём Release. Научи тягу тоже менять гравитацию — и на
    /// пересечении двух механик чужой Release восстановит гравитацию посреди
    /// активной фазы этой системы, причём молча. Не трогая gravityScale, мы
    /// делаем этот класс багов невозможным ПО ПОСТРОЕНИЮ, а не по дисциплине.
    /// Вес героя компенсируется силой тяги (PullAcceleration), а не отключением
    /// гравитации.
    ///
    /// ═══════════════════════════════════════════════════════════════════
    ///  ПРЕДОХРАНИТЕЛЬ
    /// ═══════════════════════════════════════════════════════════════════
    /// Система ведёт СВОЙ таймер от TetherData.MaxDuration и отпускает сама,
    /// даже если TetherReleaseRequest не пришёл. Причина конкретная:
    /// EntitiesLifeContext.Release кладёт заявку в очередь, сливаемую в конце
    /// Update, а слайм тикается на fixed — существует окно, в котором источник
    /// уже фактически мёртв, а запрос до героя не дошёл. Полагаться на
    /// дисциплину слайма нельзя: цена ошибки — герой, залипший в захвате
    /// навсегда.
    /// </remarks>
    public class TetherPullSystem : IInitializableSystem, IFixedUpdatableSystem, IDisposableSystem
    {
        // Взяты из GrappleSystem как стартовые значения: отпуск языка должен
        // ощущаться той же «породы», что и отпуск крюка.
        private const float ReleaseUpBias = 0.35f;
        private const float MinReleaseSpeed = 12f;

        // Скорость отскока от слайма при Arrived. Отдельная константа, а не
        // переиспользованная MinReleaseSpeed, потому что величины РАЗНОЙ
        // ПРИРОДЫ и сравнивать их в лоб нельзя: MinReleaseSpeed — нижняя
        // граница модуля ПОЛНОГО вектора при отпускании, а эта — компонента
        // ВДОЛЬ ОСИ отскока, поперечная составляющая при этом сохраняется
        // (см. семантику BounceSystem). Тюнить их нужно независимо.
        private const float ArriveBounceVelocity = 14f;

        // Ниже этого модуля вектор считается вырожденным (тот же порог, что
        // в GrappleSystem.Release).
        private const float DegenerateSpeedThreshold = 0.01f;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private ReactiveVariable<bool> _isTethered;
        private ReactiveVariable<Vector2> _anchorPoint;
        private ReactiveEvent<BounceImpulseData> _bounceImpulseRequest;

        private IDisposable _requestDisposable;
        private IDisposable _releaseDisposable;

        // Приватное состояние: наружу торчит только IsTethered.
        private bool _isActive;
        private float _pullSpeed;
        private float _pullAcceleration;
        private float _remainingTime;
        private Vector2 _lastPullDirection;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

            _isTethered = entity.IsTethered;
            _anchorPoint = entity.TetherAnchorPoint;
            _bounceImpulseRequest = entity.BounceImpulseRequest;

            _requestDisposable = entity.TetherRequest.Subscribe(OnTetherRequested);
            _releaseDisposable = entity.TetherReleaseRequest.Subscribe(OnTetherReleaseRequested);

            _isActive = false;
            _isTethered.Value = false;
        }

        /// <summary>
        /// Только взводит состояние. Скорость здесь НЕ трогается — см. раздел
        /// «канал» в шапке класса.
        /// </summary>
        private void OnTetherRequested(TetherData data)
        {
            // Повторный запрос поверх активного захвата игнорируется: писатель
            // один (TongueSystem), но у слайма нет способа узнать, что герой
            // уже захвачен ДРУГИМ слаймом. Гейт CanBeTethered на стороне слайма
            // это отсекает, однако между его проверкой и доставкой запроса есть
            // тик — поэтому дублируем защиту здесь.
            if (_isActive)
            {
                return;
            }

            _pullSpeed = data.PullSpeed;
            _pullAcceleration = data.PullAcceleration;
            _remainingTime = data.MaxDuration;
            _lastPullDirection = Vector2.zero;

            _isActive = true;
            _isTethered.Value = true;
        }

        private void OnTetherReleaseRequested(TetherReleaseReason reason)
        {
            if (_isActive == false)
            {
                return;
            }

            if (reason == TetherReleaseReason.Arrived)
            {
                ReleaseWithBounce();
                return;
            }

            ReleaseWithInertia();
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_isActive == false)
            {
                return;
            }

            if (_rigidbody == null)
            {
                Deactivate();
                return;
            }

            // Предохранитель проверяется ДО тяги: истёкший захват не должен
            // успеть применить ещё один тик ускорения.
            _remainingTime -= deltaTime;

            if (_remainingTime <= 0f)
            {
                ReleaseWithInertia();
                return;
            }

            ApplyPull(deltaTime);
        }

        /// <summary>
        /// Формулы скопированы из GrappleSystem.UpdatePull намеренно: тяга
        /// языка обязана ощущаться однородно с грэплом.
        ///
        /// Ускорение добавляется К ТЕКУЩЕМУ вектору скорости, а не заменяет
        /// его, поэтому ПЕРПЕНДИКУЛЯРНАЯ ЯКОРЮ СОСТАВЛЯЮЩАЯ СОХРАНЯЕТСЯ —
        /// это и есть физический источник частичного контроля и естественных
        /// дуг вместо струны.
        /// </summary>
        private void ApplyPull(float deltaTime)
        {
            Vector2 toAnchor = _anchorPoint.Value - (Vector2)_transform.position;
            float distance = toAnchor.magnitude;

            if (distance < DegenerateSpeedThreshold)
            {
                return;
            }

            Vector2 direction = toAnchor / distance;
            _lastPullDirection = direction;

            Vector2 velocity = _rigidbody.linearVelocity;
            velocity += direction * _pullAcceleration * deltaTime;

            // Кап общего модуля вместо AddForce: детерминированно и без
            // зависимости от массы.
            if (velocity.magnitude > _pullSpeed)
            {
                velocity = velocity.normalized * _pullSpeed;
            }

            _rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        /// Cut / Timeout / SourceDied. Пошаговая копия GrappleSystem.Release:
        /// направление берётся из РЕАЛЬНОГО вектора скорости (это и есть
        /// набранный импульс), а не из направления на якорь — иначе разруб
        /// на дуге терял бы всю поперечную составляющую.
        /// </summary>
        private void ReleaseWithInertia()
        {
            if (_rigidbody != null)
            {
                Vector2 currentVelocity = _rigidbody.linearVelocity;
                float currentSpeed = currentVelocity.magnitude;

                Vector2 baseDirection;

                if (currentSpeed > DegenerateSpeedThreshold)
                {
                    baseDirection = currentVelocity / currentSpeed;
                }
                else
                {
                    baseDirection = _lastPullDirection;
                }

                // Вырожденный случай: разруб в первый же тик, до единой
                // записи направления. Инерцию придумывать не из чего.
                if (baseDirection.sqrMagnitude > DegenerateSpeedThreshold)
                {
                    Vector2 launchDirection = (baseDirection + Vector2.up * ReleaseUpBias).normalized;
                    float finalSpeed = Mathf.Max(currentSpeed, MinReleaseSpeed);

                    _rigidbody.linearVelocity = launchDirection * finalSpeed;
                }
            }

            Deactivate();
        }

        /// <summary>
        /// Arrived. Инерция НЕ применяется — вместо неё отскок от слайма через
        /// штатный BounceImpulseRequest, то есть через ту же систему, что
        /// обслуживает трамплины.
        ///
        /// Контактный урон здесь НЕ наносится и наноситься не должен: герой
        /// прилетает в слайма, у которого есть BodyContactDamage и
        /// DealDamageOnContactSystem — урон прилетит сам, существующей цепочкой.
        ///
        /// ИЗВЕСТНАЯ ПОБОЧКА, ПРИНЯТА ОСОЗНАННО: MainHeroStyleSystem подписана
        /// на BounceImpulseRequest и начислит за этот отскок очки стиля Bounce.
        /// Источник отскока она не различает (так и задокументировано в ней).
        /// То есть игрок получит немного очков за то, что НЕ справился. Цена
        /// признана приемлемой — разводить источники ради этого пришлось бы
        /// менять контракт Bounce на всех вызывающих.
        /// </summary>
        private void ReleaseWithBounce()
        {
            if (_rigidbody != null && _bounceImpulseRequest != null)
            {
                Vector2 fromAnchor = (Vector2)_transform.position - _anchorPoint.Value;

                Vector2 upAxis;

                if (fromAnchor.sqrMagnitude > DegenerateSpeedThreshold)
                {
                    upAxis = fromAnchor.normalized;
                }
                else
                {
                    // Герой ровно в точке якоря — отскок строго вверх.
                    upAxis = Vector2.up;
                }

                BounceImpulseData bounce = new BounceImpulseData
                {
                    UpAxis = upAxis,
                    LaunchVelocity = ArriveBounceVelocity
                };

                _bounceImpulseRequest.Invoke(bounce);
            }

            Deactivate();
        }

        /// <summary>
        /// Единственная точка снятия флага. Все пути завершения обязаны идти
        /// через неё — включая OnDispose.
        /// </summary>
        private void Deactivate()
        {
            _isActive = false;
            _remainingTime = 0f;
            _lastPullDirection = Vector2.zero;

            _isTethered.Value = false;
        }

        public void OnDispose()
        {
            _requestDisposable?.Dispose();
            _requestDisposable = null;

            _releaseDisposable?.Dispose();
            _releaseDisposable = null;

            // Жёсткий teardown сцены не должен оставить глобально видимый флаг
            // взведённым (то же правило, что для StateMachine.Dispose -> Exit).
            Deactivate();
        }
    }
}
