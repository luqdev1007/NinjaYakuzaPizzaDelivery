using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Глитч-мерцание призрака: резкие короткие пропадания альфы, нерегулярные по
    // времени — «нестабильная сущность / помехи», а не плавный синус-пульс.
    //
    // Рандом здесь — ГЛОБАЛЬНЫЙ UnityEngine.Random, сознательно: мерцание ни на что
    // в симуляции не влияет. Геймплейная ветка ушла на засеянный IGameplayRandom,
    // поэтому эти вызовы больше не сдвигают ни направление движения, ни разброс фаз.
    //
    // КООРДИНАЦИЯ С ApplyDamageView (оба висят на View и делят один SpriteRenderer):
    // 1. Приоритет у damage-вспышки. На TakeDamageEvent глитч гасит себя и уходит
    //    в паузу на _damageSuspendDuration, после чего перезапускается. Вспышка
    //    отыгрывает по чистому рендереру.
    // 2. Глитч НИКОГДА не зовёт _spriteRenderer.DOKill() в рантайме (только в
    //    Cleanup, на teardown). DOKill бьёт по цели, а цель общая — глитч убил бы
    //    твин вспышки. Поэтому глитч убивает ТОЛЬКО свои хендлы. Обратное
    //    направление безопасно: ApplyDamageView.PlayFlashEffect() зовёт DOKill по
    //    рендереру и может снести твины глитча — это ожидаемо, глитч всё равно
    //    пересобирается заново после паузы.
    // 3. Пауза переживает DOKill вспышки: DOVirtual.DelayedCall не привязан к
    //    рендереру как к цели, поэтому его тот DOKill не заденет.
    // 4. _originalColor в ApplyDamageView снимается в его OnEntityStartedWork.
    //    Глитч в OnEntityStartedWork цвет НЕ трогает (только читает базу) и первую
    //    серию планирует через задержку — иначе вспышка зафиксировала бы случайную
    //    фазу мерцания как «оригинал».
    public class GhostGlitchView : EntityView
    {
        [Header("Renderer")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Alpha")]
        [Tooltip("Альфа в момент глитч-пропадания")]
        [SerializeField] private float _minAlpha = 0.08f;

        [Header("Timing")]
        [Tooltip("Резкость фронта. Держать очень малым — мерцание должно быть рубленым, не плавным")]
        [SerializeField] private float _snapDuration = 0.02f;

        [Tooltip("Сколько призрак держится пропавшим за одно мерцание")]
        [SerializeField] private float _flickerDuration = 0.06f;

        [Tooltip("Мин/макс пауза между сериями мерцаний — источник нерегулярности")]
        [SerializeField] private float _minInterval = 0.15f;
        [SerializeField] private float _maxInterval = 1.2f;

        [Tooltip("Максимум мерцаний подряд в одной серии (1..N, выбирается случайно)")]
        [SerializeField] private int _maxBurstFlickers = 3;

        [Header("Start Offset")]
        [Tooltip("Случайная задержка перед ПЕРВОЙ серией. Без неё все призраки, заспавненные " +
                 "в одном кадре, начинали мерцать в один такт — интервалы разводят их только потом")]
        [SerializeField] private float _minStartDelay = 0f;
        [SerializeField] private float _maxStartDelay = 1.5f;

        [Header("Damage Coordination")]
        [Tooltip("На сколько глитч уступает damage-вспышке. Обязан быть БОЛЬШЕ полной " +
                 "длительности вспышки ApplyDamageView (_flashDuration * 2 из-за Yoyo-лупа; " +
                 "в Ghost.prefab это 0.2 * 2 = 0.4)")]
        [SerializeField] private float _damageSuspendDuration = 0.45f;

        private IDisposable _damageEventDisposable;
        private Sequence _glitchSequence;
        private Tween _pendingCall;

        // Базовая альфа СНИМАЕТСЯ С РЕНДЕРЕРА, а не берётся из сериализованной
        // константы. Раньше здесь жило поле _maxAlpha = 0.49, и глитч навязывал его
        // в двух местах — из-за чего любая per-instance базовая прозрачность
        // проживала до первого мерцания и затиралась.
        // GhostVisualVariationView стоит на View ВЫШЕ по списку компонентов, значит
        // его OnEntityStartedWork уже выставил персональную альфу к этому моменту.
        private float _baseAlpha;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _damageEventDisposable = entity.TakeDamageEvent.Subscribe(OnDamaged);

            _baseAlpha = _spriteRenderer.color.a;

            // Первая серия — через случайную задержку, чтобы стая не мигала в такт.
            _pendingCall = DOVirtual.DelayedCall(
                UnityEngine.Random.Range(_minStartDelay, _maxStartDelay),
                ScheduleNextGlitch,
                false);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _damageEventDisposable?.Dispose();

            KillOwnTweens();

            // Здесь DOKill по рендереру уместен: сущность уничтожается целиком,
            // и твин обязан не пережить Destroy объекта.
            _spriteRenderer.DOKill();
        }

        private void OnDamaged(DamageData data)
        {
            // Уступаем вспышке: свои твины снимаем, цвет не трогаем — его сейчас
            // выставит и восстановит сама ApplyDamageView.
            KillOwnTweens();

            _pendingCall = DOVirtual.DelayedCall(_damageSuspendDuration, RestartGlitch, false);
        }

        private void RestartGlitch()
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            // Вспышка уже закончилась и вернула свой _originalColor — приводим альфу
            // к базовой явно, чтобы глитч всегда стартовал от известного состояния.
            Color color = _spriteRenderer.color;
            color.a = _baseAlpha;
            _spriteRenderer.color = color;

            ScheduleNextGlitch();
        }

        private void ScheduleNextGlitch()
        {
            Sequence sequence = DOTween.Sequence();

            int flickers = UnityEngine.Random.Range(1, _maxBurstFlickers + 1);

            for (int i = 0; i < flickers; i++)
            {
                sequence.Append(_spriteRenderer.DOFade(_minAlpha, _snapDuration).SetEase(Ease.Linear));
                sequence.AppendInterval(UnityEngine.Random.Range(_flickerDuration * 0.5f, _flickerDuration));
                sequence.Append(_spriteRenderer.DOFade(_baseAlpha, _snapDuration).SetEase(Ease.Linear));

                if (i < flickers - 1)
                {
                    // Рваная пауза внутри серии — читается как помеха, а не как ритм
                    sequence.AppendInterval(UnityEngine.Random.Range(0.02f, 0.08f));
                }
            }

            sequence.AppendInterval(UnityEngine.Random.Range(_minInterval, _maxInterval));

            // Пересобираем серию заново вместо SetLoops: интервалы должны
            // перевыбираться каждый раз, иначе рисунок мерцания зациклится.
            sequence.OnComplete(ScheduleNextGlitch);

            _glitchSequence = sequence;
        }

        private void KillOwnTweens()
        {
            // Только свои хендлы. defaultRecyclable выключен в DOTweenSettings,
            // поэтому мёртвый хендл не может указывать на чужой твин.
            if (_glitchSequence != null && _glitchSequence.IsActive())
            {
                _glitchSequence.Kill();
            }

            if (_pendingCall != null && _pendingCall.IsActive())
            {
                _pendingCall.Kill();
            }

            _glitchSequence = null;
            _pendingCall = null;
        }
    }
}
