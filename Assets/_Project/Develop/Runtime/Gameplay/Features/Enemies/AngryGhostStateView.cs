using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Explosion;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Визуал состояний призрака-камикадзе: агро-свечение, индикатор радиуса
    // взрыва, VFX и звук детонации.
    //
    // РАЗДЕЛЕНИЕ РЕНДЕРЕРОВ ПРИНЦИПИАЛЬНО. Этот скрипт НЕ ТРОГАЕТ SpriteRenderer
    // объекта View. Там уже три писателя цвета — GhostVisualVariationView (разовый
    // тинт и базовая альфа), GhostGlitchView (мерцание) и ApplyDamageView
    // (вспышка урона), — и они координируются вручную, попарно, через паузы и
    // выборочный DOKill. Четвёртый писатель в эту схему не влезает без переработки
    // всех трёх. Поэтому _agroGlow — ОТДЕЛЬНЫЙ дочерний рендерер, эксклюзивная
    // собственность этого скрипта, и DOKill по нему здесь безопасен: конкурентов
    // на этой цели нет.
    //
    // По той же причине пульсация взведения гоняет альфу _agroGlow, а не основного
    // спрайта: иначе она дралась бы с глитчем.
    public class AngryGhostStateView : EntityView, IRequireAudioService
    {
        // Индикатор радиуса считает, что спрайт — круг ДИАМЕТРОМ В ОДНУ ЮНИТИ
        // при localScale = 1. Радиус взрыва переводится в диаметр умножением на 2.
        // Спрайт другого размера — правь этот множитель, а не радиус в конфиге.
        private const float RadiusToDiameter = 2f;

        // Пульсаций за окно взведения. Каждая следующая короче предыдущей в
        // PulseAcceleration раз — так «тикание» на слух и на глаз разгоняется
        // к моменту взрыва.
        private const int ArmingPulseCount = 5;
        private const float PulseAcceleration = 0.72f;

        [Header("Refs")]
        [Tooltip("Отдельный рендерер агро-свечения. НЕ основной спрайт View — " +
                 "тот делят между собой три других вьюхи")]
        [SerializeField] private SpriteRenderer _agroGlow;

        [Tooltip("Индикатор радиуса взрыва. Масштабируется под ExplosionRadius. " +
                 "Вешать НЕ под ViewContainer: тому GhostVisualVariationView " +
                 "выдаёт случайный скейл 0.85..1.15, и радиус показывался бы враньём")]
        [SerializeField] private SpriteRenderer _blastRadiusIndicator;

        [Tooltip("Необязателен. Спавнится через Instantiate со stopAction = Destroy — " +
                 "пула для VFX в проекте нет")]
        [SerializeField] private ParticleSystem _explosionVfxPrefab;

        [Header("Agro Glow")]
        [SerializeField] private float _agroGlowAlpha = 0.85f;
        [SerializeField] private float _agroFadeInDuration = 0.25f;

        [Tooltip("Нижняя точка альфы в пульсации взведения")]
        [SerializeField] private float _armingPulseMinAlpha = 0.15f;

        [Header("SFX Keys")]
        [Tooltip("Пустая строка = звук не играется. Ключи заполняет разработчик, " +
                 "SoundData-ассеты под них ещё не заведены")]
        [SerializeField] private string _agroSfxKey = "";
        [SerializeField] private string _armingSfxKey = "";
        [SerializeField] private string _explosionSfxKey = "";

        private IAudioService _audioService;

        private IDisposable _isAgroDisposable;
        private IDisposable _isArmingDisposable;
        private IDisposable _detonationDisposable;

        private Sequence _armingPulseSequence;
        private Tween _agroFadeTween;

        private float _armingDuration;
        private float _explosionRadius;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _armingDuration = entity.ArmingDuration.Value;
            _explosionRadius = entity.ExplosionRadius.Value;

            PrepareAgroGlow();
            PrepareBlastRadiusIndicator();

            _isAgroDisposable = entity.IsAgro.Subscribe(OnIsAgroChanged);
            _isArmingDisposable = entity.IsArming.Subscribe(OnIsArmingChanged);
            _detonationDisposable = entity.DetonationEvent.Subscribe(OnDetonated);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isAgroDisposable?.Dispose();
            _isArmingDisposable?.Dispose();
            _detonationDisposable?.Dispose();

            KillOwnTweens();

            // Здесь DOKill по цели уместен: сущность уничтожается целиком, твин
            // обязан не пережить Destroy объекта. Конкурентов на этом рендерере
            // нет, чужие твины не заденем.
            if (_agroGlow != null)
            {
                _agroGlow.DOKill();
            }
        }

        private void PrepareAgroGlow()
        {
            if (_agroGlow == null)
            {
                return;
            }

            Color color = _agroGlow.color;
            color.a = 0f;
            _agroGlow.color = color;
        }

        private void PrepareBlastRadiusIndicator()
        {
            if (_blastRadiusIndicator == null)
            {
                return;
            }

            _blastRadiusIndicator.transform.localScale =
                Vector3.one * (_explosionRadius * RadiusToDiameter);

            _blastRadiusIndicator.gameObject.SetActive(false);
        }

        private void OnIsAgroChanged(bool oldValue, bool isAgro)
        {
            if (isAgro == false)
            {
                return;
            }

            PlaySfx(_agroSfxKey);

            if (_agroGlow == null)
            {
                return;
            }

            KillAgroFadeTween();

            _agroFadeTween = _agroGlow.DOFade(_agroGlowAlpha, _agroFadeInDuration).SetEase(Ease.OutQuad);
        }

        private void OnIsArmingChanged(bool oldValue, bool isArming)
        {
            if (isArming)
            {
                StartArmingVisuals();
            }
            else
            {
                RollbackArmingVisuals();
            }
        }

        private void StartArmingVisuals()
        {
            PlaySfx(_armingSfxKey);

            if (_blastRadiusIndicator != null)
            {
                _blastRadiusIndicator.gameObject.SetActive(true);
            }

            if (_agroGlow == null)
            {
                return;
            }

            KillArmingPulseSequence();
            KillAgroFadeTween();

            _armingPulseSequence = BuildArmingPulseSequence();
        }

        // Пульсации ускоряются геометрически, а суммарная длительность
        // нормируется под ArmingDuration — визуал заканчивается ровно тогда, когда
        // ArmingTimerSystem досчитает таймер.
        private Sequence BuildArmingPulseSequence()
        {
            float totalWeight = 0f;
            float weight = 1f;

            for (int i = 0; i < ArmingPulseCount; i++)
            {
                totalWeight += weight;
                weight *= PulseAcceleration;
            }

            Sequence sequence = DOTween.Sequence();

            weight = 1f;

            for (int i = 0; i < ArmingPulseCount; i++)
            {
                float pulseDuration = _armingDuration * (weight / totalWeight);
                float halfPulse = pulseDuration * 0.5f;

                sequence.Append(_agroGlow.DOFade(_armingPulseMinAlpha, halfPulse).SetEase(Ease.InQuad));
                sequence.Append(_agroGlow.DOFade(_agroGlowAlpha, halfPulse).SetEase(Ease.OutQuad));

                weight *= PulseAcceleration;
            }

            return sequence;
        }

        // Игрок разорвал дистанцию — откатываемся в «просто агро»: индикатор
        // гаснет, пульсация снимается, свечение возвращается к ровной альфе.
        private void RollbackArmingVisuals()
        {
            if (_blastRadiusIndicator != null)
            {
                _blastRadiusIndicator.gameObject.SetActive(false);
            }

            if (_agroGlow == null)
            {
                return;
            }

            KillArmingPulseSequence();
            KillAgroFadeTween();

            _agroFadeTween = _agroGlow.DOFade(_agroGlowAlpha, _agroFadeInDuration).SetEase(Ease.OutQuad);
        }

        private void OnDetonated(DetonationKind detonationKind)
        {
            KillOwnTweens();

            if (_blastRadiusIndicator != null)
            {
                _blastRadiusIndicator.gameObject.SetActive(false);
            }

            SpawnExplosionVfx();

            PlaySfx(_explosionSfxKey);
        }

        private void SpawnExplosionVfx()
        {
            if (_explosionVfxPrefab == null)
            {
                return;
            }

            // Отдельным инстансом в мире, а не под призраком: сущность
            // самоуничтожается почти сразу после детонации и утащила бы эффект
            // с собой. Образец — ApplyDamageView.SpawnDamageParticles.
            ParticleSystem vfx = Instantiate(_explosionVfxPrefab, transform.position, Quaternion.identity);

            ParticleSystem.MainModule main = vfx.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }

        private void PlaySfx(string key)
        {
            // Незарегистрированный ключ доходит до AudioService.PlaySfx как null от
            // AudioLibrary.GetSound и роняет эмиттер без обработки. Пустые ключи —
            // штатное состояние до заведения SoundData-ассетов, поэтому пропускаем
            // молча. Образец — PropVisualsView.
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _audioService?.PlaySfx(key, transform.position);
        }

        private void KillOwnTweens()
        {
            KillArmingPulseSequence();
            KillAgroFadeTween();
        }

        private void KillArmingPulseSequence()
        {
            if (_armingPulseSequence != null && _armingPulseSequence.IsActive())
            {
                _armingPulseSequence.Kill();
            }

            _armingPulseSequence = null;
        }

        private void KillAgroFadeTween()
        {
            if (_agroFadeTween != null && _agroFadeTween.IsActive())
            {
                _agroFadeTween.Kill();
            }

            _agroFadeTween = null;
        }
    }
}
