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
    /// Мельтешение при удачном уклонении: короткий burst афтеримиджей на EvadedEvent.
    ///
    /// Механика копирования спрайта — та же, что в DashView.SpawnAfterimage (спрайт,
    /// мировая позиция, lossyScale, поворот, затем пулованный AfterimageInstance
    /// сам гасит альфу и возвращается). Цвет НАМЕРЕННО другой: дэш голубой, уворот
    /// белый. Во время дэша герой и так неуязвим (canApplyDamage требует
    /// IsDashing == false), поэтому одинаковая картинка на два разных события
    /// читалась бы игроком как один и тот же эффект.
    ///
    /// ЗАЧЕМ ОТДЕЛЬНАЯ ВЬЮХА, А НЕ ВЕТКА В DashView: у дэша к мельтешению
    /// пришиты SFX ("DashExecute") и флаг аниматора IsDashing. Переиспользование
    /// дало бы на уворот звук рывка и позу рывка — то есть враньё в анимации.
    /// Здесь нет ни аниматора, ни обязательного звука.
    ///
    /// ПУЛ ЗДЕСЬ ЖИВЁТ ПРАВИЛЬНО, в отличие от исходной схемы DashView: объекты
    /// лежат под собственным рут-объектом, а Cleanup возвращает недоигравшие
    /// афтеримиджи и уничтожает рут целиком. Рут при этом НЕ ребёнок героя —
    /// иначе следы таскались бы за ним вместо того, чтобы висеть в мире.
    /// </summary>
    public class AfterimageView : EntityView, IRequireAudioService
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Afterimage (VFX)")]
        [SerializeField] private GameObject _afterimagePrefab;
        [SerializeField, Min(1)] private int _burstCount = 4;
        [SerializeField, Min(0f)] private float _burstInterval = 0.03f;
        [SerializeField, Min(0.01f)] private float _afterimageLifetime = 0.2f;
        [SerializeField] private Color _afterimageColor = new Color(1f, 1f, 1f, 0.8f);
        [SerializeField, Min(1)] private int _poolSize = 6;

        [Header("SFX Keys")]
        [Tooltip("Пусто — звук не проигрывается. Уворот по умолчанию беззвучный.")]
        [SerializeField] private string _evadeSfxKey = string.Empty;

        private IDisposable _evadedDisposable;

        private GameObjectPool _pool;
        private GameObject _poolRoot;
        private Coroutine _burstCoroutine;

        // Выданные наружу и ещё не догоревшие афтеримиджи. Нужны только затем,
        // чтобы Cleanup мог вернуть их до уничтожения рута.
        private readonly List<GameObject> _liveAfterimages = new();

        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Рут не парентится к герою СПЕЦИАЛЬНО: афтеримидж — это отпечаток в
            // мире, он обязан остаться там, где был снят. Ребёнок героя ехал бы
            // за ним и превращался в шлейф, приклеенный к спрайту.
            _poolRoot = new GameObject($"AfterimagePool (Evade) [{name}]");

            _pool = new GameObjectPool(_afterimagePrefab, _poolRoot.transform, _poolSize);

            if (entity.TryGetEvadedEvent(out Utilities.Reactive.ReactiveEvent evadedEvent))
            {
                _evadedDisposable = evadedEvent.Subscribe(OnEvaded);
            }
        }

        private void OnEvaded()
        {
            // Уворот может прилететь повторно, пока предыдущий burst ещё идёт
            // (окно неуязвимости после уклонения НЕ открывается — это осознанное
            // решение из ApplyDamageSystem). Перезапуск, а не наложение: два
            // параллельных burst'а просто сожгли бы пул вдвое быстрее.
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
                SpawnAfterimage();

                // После последней копии не ждём: хвостовая пауза ничего не
                // отрисовывает, но держала бы корутину живой лишний интервал.
                if (i < _burstCount - 1)
                {
                    yield return new WaitForSeconds(_burstInterval);
                }
            }

            _burstCoroutine = null;
        }

        private void SpawnAfterimage()
        {
            GameObject obj = _pool.Get();

            if (obj.TryGetComponent(out AfterimageInstance instance))
            {
                _liveAfterimages.Add(obj);

                instance.Initialize(
                    _spriteRenderer.sprite,
                    _spriteRenderer.transform.position,
                    _spriteRenderer.transform.lossyScale,
                    _afterimageLifetime,
                    _afterimageColor,
                    ReturnToPool);

                obj.transform.rotation = _spriteRenderer.transform.rotation;

                return;
            }

            // Префаб без AfterimageInstance гасить некому — вернём сразу, иначе
            // объект остался бы активным навсегда.
            _pool.Return(obj);
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

            // Уничтожение рута забирает с собой весь пул — и свободные объекты,
            // и только что возвращённые. После этого в корне сцены не остаётся
            // ничего, что пережило бы героя.
            if (_poolRoot != null)
            {
                Destroy(_poolRoot);
                _poolRoot = null;
            }

            _pool = null;
        }
    }
}
