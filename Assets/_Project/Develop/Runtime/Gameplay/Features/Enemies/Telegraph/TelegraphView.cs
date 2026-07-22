using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Telegraph
{
    /// <summary>
    /// Телеграф перед атакой: узел визуала сжимается и разжимается, читаемо
    /// сообщая игроку об окне для реакции. Ведётся флагом <see cref="IsTelegraphing"/>.
    ///
    /// ОБЩАЯ ВЬЮХА. Изначально писалась под плевок слайма (SlimeView), вынесена
    /// в нейтральную локацию, когда телеграф понадобился фонарю. Слайм-специфики
    /// в анимации нет — только scale по _visualTarget.
    ///
    /// АНИМИРУЕТСЯ ТОЛЬКО SCALE, И ЭТО ЖЁСТКОЕ ОГРАНИЧЕНИЕ. На слайме цвет
    /// основного SpriteRenderer узла уже занят ApplyDamageView (DOColor + DOKill),
    /// четвёртого писателя цвета туда добавлять нельзя — тот же урок, что
    /// зафиксирован в шапке AngryGhostStateView. Держим вьюху на одном канале
    /// (scale), чтобы она оставалась переносимой между врагами.
    ///
    /// Твины хранятся в поле и убиваются АДРЕСНО (KillTelegraphSequence).
    /// Глобального DOKill по цели здесь нет и быть не должно: на том же
    /// трансформе может жить punch-scale от EnemyBounceSurface, а на его
    /// рендерере — твины ApplyDamageView. DOKill по цели снёс бы и их.
    /// </summary>
    public class TelegraphView : EntityView
    {
        [Header("Refs")]
        [SerializeField] private Transform _visualTarget;

        [Header("Settings")]
        [SerializeField] private Vector3 _squashScale = new Vector3(1.25f, 0.75f, 1f);
        [SerializeField] private float _squashDuration = 0.15f;
        [SerializeField] private float _releaseDuration = 0.1f;

        private IReadOnlyVariable<bool> _isTelegraphing;
        private IDisposable _isTelegraphingDisposable;

        private Sequence _telegraphSequence;
        private Vector3 _baseScale;

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (_visualTarget == null)
            {
                return;
            }

            _baseScale = _visualTarget.localScale;

            _isTelegraphing = entity.IsTelegraphing;
            _isTelegraphingDisposable = _isTelegraphing.Subscribe(OnIsTelegraphingChanged);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isTelegraphingDisposable?.Dispose();
            _isTelegraphingDisposable = null;

            KillTelegraphSequence();
        }

        private void OnIsTelegraphingChanged(bool oldValue, bool isTelegraphing)
        {
            if (isTelegraphing)
            {
                PlaySquash();
            }
            else
            {
                PlayRelease();
            }
        }

        private void PlaySquash()
        {
            if (_visualTarget == null)
            {
                return;
            }

            KillTelegraphSequence();

            _telegraphSequence = DOTween.Sequence();
            _telegraphSequence.Append(_visualTarget
                .DOScale(Vector3.Scale(_baseScale, _squashScale), _squashDuration)
                .SetEase(Ease.OutQuad));
        }

        private void PlayRelease()
        {
            if (_visualTarget == null)
            {
                return;
            }

            KillTelegraphSequence();

            _telegraphSequence = DOTween.Sequence();
            _telegraphSequence.Append(_visualTarget
                .DOScale(_baseScale, _releaseDuration)
                .SetEase(Ease.OutBack));
        }

        // Адресное убийство: только собственная последовательность, чужие твины
        // на этой же цели не трогаем.
        private void KillTelegraphSequence()
        {
            if (_telegraphSequence == null)
            {
                return;
            }

            _telegraphSequence.Kill();
            _telegraphSequence = null;
        }
    }
}
