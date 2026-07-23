using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.ObjectsManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    /// <summary>
    /// Мельтешение при удачном уклонении: силуэт героя «раздваивается» на
    /// несколько полупрозрачных копий, которые веером расходятся в стороны и
    /// растворяются. Референс — иллюзии Мастера Клинка из WarCraft 3.
    ///
    /// ПОЧЕМУ НЕ ПРОСТО КОПИЯ ДЭШЕВОГО ЭФФЕКТА. Первая версия была именно ей, и
    /// эффекта не было видно вообще — по двум причинам сразу:
    ///   1. Копии рождались в одной точке. У рывка разлёт даёт само движение:
    ///      герой улетает, копии остаются вдоль траектории. Контактный урон
    ///      прилетает в почти неподвижного героя, поэтому все копии ложились друг
    ///      на друга и на самого героя.
    ///   2. Порядок отрисовки у префаба следа — 0, у спрайта героя — 10, слой
    ///      один. Совпав с героем, копии оказывались полностью за ним.
    /// Отсюда здесь и собственный разлёт (offset + снос), и явный sorting order.
    ///
    /// ОТДЕЛЬНАЯ ВЬЮХА, А НЕ ВЕТКА В DashView: к дэшевому мельтешению пришиты SFX
    /// "DashExecute" и флаг аниматора IsDashing. Переиспользование дало бы на
    /// уворот звук рывка и позу рывка — враньё в анимации.
    ///
    /// ПУЛ: объекты лежат под собственным рут-объектом, Cleanup возвращает
    /// недоигравшие копии и уничтожает рут целиком. Рут НЕ ребёнок героя —
    /// иначе копии таскались бы за ним вместо того, чтобы расходиться в мире.
    /// </summary>
    public class AfterimageView : EntityView, IRequireAudioService
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Afterimage (VFX)")]
        [SerializeField] private GameObject _afterimagePrefab;
        [SerializeField, Min(1)] private int _burstCount = 5;
        [SerializeField, Min(0f)] private float _burstInterval = 0.02f;
        [SerializeField, Min(0.01f)] private float _afterimageLifetime = 0.35f;
        [SerializeField] private Color _afterimageColor = Color.white;
        [SerializeField, Range(0f, 1f)] private float _startAlpha = 0.75f;
        [SerializeField, Min(1)] private int _poolSize = 8;

        [Header("Разлёт")]
        [Tooltip("Насколько копия рождается в стороне от героя.")]
        [SerializeField, Min(0f)] private float _spawnOffset = 0.18f;

        [Tooltip("Начальная скорость сноса наружу. Снос затухающий.")]
        [SerializeField, Min(0f)] private float _driftSpeed = 3.5f;

        [Tooltip("Подъём веера. 0 — копии расходятся строго по горизонтали.")]
        [SerializeField, Min(0f)] private float _verticalSpread = 0.6f;

        [Tooltip("Случайный разброс направления в градусах, чтобы веер не был идеально ровным.")]
        [SerializeField, Min(0f)] private float _jitterAngle = 12f;

        [Tooltip("Прирост масштаба к концу жизни — копия расплывается, а не просто гаснет.")]
        [SerializeField, Min(0f)] private float _scaleGrowth = 0.35f;

        [Tooltip("Порядок отрисовки копий. У героя 10 — значение выше кладёт копии поверх него.")]
        [SerializeField] private int _sortingOrder = 11;

        [Header("SFX Keys")]
        [Tooltip("Пусто — звук не проигрывается. Уворот по умолчанию беззвучный.")]
        [SerializeField] private string _evadeSfxKey = string.Empty;

        private IDisposable _evadedDisposable;

        private GameObjectPool _pool;
        private GameObject _poolRoot;
        private Coroutine _burstCoroutine;

        // Выданные наружу и ещё не догоревшие копии. Нужны только затем, чтобы
        // Cleanup мог вернуть их до уничтожения рута.
        private readonly List<GameObject> _liveAfterimages = new();

        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _poolRoot = new GameObject($"AfterimagePool (Evade) [{name}]");

            _pool = new GameObjectPool(_afterimagePrefab, _poolRoot.transform, _poolSize);

            if (entity.TryGetEvadedEvent(out Utilities.Reactive.ReactiveEvent evadedEvent))
            {
                _evadedDisposable = evadedEvent.Subscribe(OnEvaded);
            }
        }

        private void OnEvaded()
        {
            // Уворот может прилететь повторно, пока предыдущий разлёт ещё идёт
            // (окно неуязвимости после уклонения НЕ открывается — это осознанное
            // решение в ApplyDamageSystem). Перезапуск, а не наложение: два
            // параллельных веера просто сожгли бы пул вдвое быстрее.
            if (_burstCoroutine != null)
            {
                StopCoroutine(_burstCoroutine);
            }

            _burstCoroutine = StartCoroutine(BurstCoroutine());

            PlayEvadeSound();
        }

        private IEnumerator BurstCoroutine()
        {
            for (int i = 0; i < _burstCount; i++)
            {
                SpawnAfterimage(i);

                // Пауза только между копиями и только если она задана: при нулевом
                // интервале весь веер обязан появиться в одном кадре, как иллюзии
                // Мастера Клинка, а yield return WaitForSeconds(0) растянул бы его
                // на пять кадров.
                if (i < _burstCount - 1 && _burstInterval > 0f)
                {
                    yield return new WaitForSeconds(_burstInterval);
                }
            }

            _burstCoroutine = null;
        }

        private void SpawnAfterimage(int index)
        {
            GameObject obj = _pool.Get();

            if (obj.TryGetComponent(out AfterimageInstance instance) == false)
            {
                // Префаб без AfterimageInstance гасить некому — вернём сразу,
                // иначе объект остался бы активным навсегда.
                _pool.Return(obj);

                return;
            }

            _liveAfterimages.Add(obj);

            Vector2 direction = GetSpreadDirection(index);
            Vector3 spawnPosition = _spriteRenderer.transform.position + (Vector3)(direction * _spawnOffset);

            instance.Initialize(
                _spriteRenderer.sprite,
                spawnPosition,
                _spriteRenderer.transform.lossyScale,
                _afterimageLifetime,
                _afterimageColor,
                ReturnToPool,
                direction * _driftSpeed,
                _scaleGrowth,
                _sortingOrder,
                _startAlpha);

            obj.transform.rotation = _spriteRenderer.transform.rotation;
        }

        /// <summary>
        /// Направление разлёта для копии по её номеру. Веер СИММЕТРИЧНЫЙ: копии
        /// уходят попеременно вправо и влево, поэтому силуэт раздваивается вокруг
        /// героя, а не сползает в одну сторону (последнее читалось бы как рывок).
        /// Чем дальше копия по вееру, тем выше её подъём — иначе на неподвижном
        /// герое весь веер вырождается в одну горизонтальную полосу.
        /// </summary>
        private Vector2 GetSpreadDirection(int index)
        {
            float side = (index % 2 == 0) ? 1f : -1f;
            int rank = index / 2;

            float vertical = _verticalSpread * (rank + 1f) / Mathf.Max(1f, _burstCount);

            Vector2 direction = new Vector2(side, vertical).normalized;

            // Визуальный джиттер идёт через UnityEngine.Random, а НЕ через
            // IGameplayRandom: засеянный поток предназначен для геймплейных
            // решений (сам бросок уклонения там и живёт), а визуал в него
            // сознательно не ходит — см. док-комментарий IGameplayRandom.
            float jitter = UnityEngine.Random.Range(-_jitterAngle, _jitterAngle);

            return Quaternion.Euler(0f, 0f, jitter) * direction;
        }

        private void ReturnToPool(GameObject obj)
        {
            _liveAfterimages.Remove(obj);

            _pool.Return(obj);
        }

        private void PlayEvadeSound()
        {
            if (string.IsNullOrEmpty(_evadeSfxKey))
            {
                return;
            }

            _audioService?.PlaySfx(_evadeSfxKey, transform.position);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _evadedDisposable?.Dispose();

            if (_burstCoroutine != null)
            {
                StopCoroutine(_burstCoroutine);
                _burstCoroutine = null;
            }

            // Обратный проход: ReturnToPool вычёркивает элемент из этого же списка.
            for (int i = _liveAfterimages.Count - 1; i >= 0; i--)
            {
                GameObject afterimage = _liveAfterimages[i];

                if (afterimage != null)
                {
                    _pool.Return(afterimage);
                }
            }

            _liveAfterimages.Clear();

            if (_poolRoot != null)
            {
                Destroy(_poolRoot);
                _poolRoot = null;
            }

            _pool = null;
        }
    }
}
