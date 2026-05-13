using Assets._Project.Develop.Runtime.UI.Core;
using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogDisplayView : PopupViewBase
    {
        public event Action AppearanceFinished;
        public event Action Hidden;

        [field: SerializeField] public TMP_Text СontentProgressText { get; private set; }

        [SerializeField] private RectTransform _skipLabelVisual;
        [SerializeField] private CanvasGroup _skipLabelGroup;
        [SerializeField] private Image _portraitImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Animator _animator;

        private Tween _shakeTween;
        private Tween _holdTween;
        private Tween _typewriterTween;

        private const float TextSpeed = 0.03f;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            _animator.SetTrigger("Show");
            _skipLabelVisual.localScale = Vector3.one;
            _skipLabelVisual.localRotation = Quaternion.identity;
            _skipLabelGroup.alpha = 0;
            СontentProgressText.text = string.Empty;
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            StopSkipAnims();
            _typewriterTween?.Kill();
            HideSkipWithPizzaEffect();
            _animator.SetTrigger("Hide");
        }

        public void OnAppearanceAnimationEnded()
        {
            AppearanceFinished?.Invoke();
        }

        public void OnHideAnimationEnded()
        {
            Hidden?.Invoke();
        }

        public void SetText(string text)
        {
            _typewriterTween?.Kill();

            СontentProgressText.text = text;
            СontentProgressText.maxVisibleCharacters = 0;

            _typewriterTween = DOTween.To(() => СontentProgressText.maxVisibleCharacters,
                x => СontentProgressText.maxVisibleCharacters = x,
                text.Length,
                text.Length * TextSpeed)
                .SetEase(Ease.Linear);
        }

        public void FinishTypingInstant()
        {
            _typewriterTween?.Kill();
            СontentProgressText.maxVisibleCharacters = СontentProgressText.text.Length;
        }

        public void SetPortrait(Sprite portrait)
        {
            _portraitImage.sprite = portrait;
        }

        public void SetBackground(Sprite bg)
        {
            _backgroundImage.sprite = bg;
        }

        public void ShowSkipHint()
        {
            _skipLabelGroup.DOKill();
            _skipLabelGroup.DOFade(1f, 0.4f);

            _shakeTween?.Kill();
            _shakeTween = _skipLabelVisual.DOShakeAnchorPos(1f, 5, 10)
                .SetLoops(-1)
                .SetDelay(3f)
                .SetLink(_skipLabelVisual.gameObject);
        }

        public void StartHoldAnims(float duration)
        {
            _shakeTween?.Pause();
            _holdTween?.Kill();
            _holdTween = _skipLabelVisual.DOScale(1.4f, duration).SetEase(Ease.OutQuad);
        }

        public void StopHoldAnims()
        {
            _holdTween?.Kill();
            _skipLabelVisual.DOScale(1f, 0.2f).OnComplete(() =>
            {
                _shakeTween?.Play();
            });
        }

        public void ExplodeSkip()
        {
            StopSkipAnims();

            _skipLabelVisual.DOScale(2.5f, 0.25f).SetEase(Ease.OutExpo);
            _skipLabelGroup.DOFade(0f, 0.2f);
        }

        private void HideSkipWithPizzaEffect()
        {
            _skipLabelVisual.DOKill();
            _skipLabelGroup.DOKill();

            Sequence sequence = DOTween.Sequence();

            sequence.Join(_skipLabelVisual.DOAnchorPosY(_skipLabelVisual.anchoredPosition.y + 150f, 0.6f).SetEase(Ease.OutQuad));
            sequence.Join(_skipLabelVisual.DORotate(new Vector3(360, 0, 180), 0.6f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
            sequence.Join(_skipLabelVisual.DOScale(0f, 0.6f).SetEase(Ease.InBack));
            sequence.Join(_skipLabelGroup.DOFade(0f, 0.4f).SetDelay(0.2f));

            sequence.SetLink(_skipLabelVisual.gameObject);
        }

        private void StopSkipAnims()
        {
            _shakeTween?.Kill();
            _holdTween?.Kill();
        }
    }
}