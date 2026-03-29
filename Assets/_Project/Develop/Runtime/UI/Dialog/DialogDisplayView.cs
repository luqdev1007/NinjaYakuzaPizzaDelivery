using Assets._Project.Develop.Runtime.UI.Core;
using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogDisplayView : MonoBehaviour, IView
    {
        public event Action AppearanceFinished;
        public event Action Hidden; // Новое событие завершения скрытия

        [field: SerializeField] public TMP_Text СontentProgressText { get; private set; }

        [SerializeField] private RectTransform _skipLabelVisual;
        [SerializeField] private CanvasGroup _skipLabelGroup;

        [SerializeField] private Image _portraitImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Animator _animator;

        private Tween _shakeTween;
        private Tween _holdTween;

        public void Show()
        {
            _animator.SetTrigger("Show");
            _skipLabelVisual.localScale = Vector3.one;
            _skipLabelVisual.localRotation = Quaternion.identity;
            _skipLabelGroup.alpha = 0;
        }

        public void Hide()
        {
            StopSkipAnims();
            // Сначала запускаем эффект пиццы, а Animator дернем из ивента или параллельно
            HideSkipWithPizzaEffect();
            _animator.SetTrigger("Hide");
        }

        public void OnAppearanceAnimationEnded() => AppearanceFinished?.Invoke();

        // ВАЖНО: Вызывай этот метод из Animation Event в конце клипа DialogEndAnim
        public void OnHideAnimationEnded() => Hidden?.Invoke();

        public void SetText(string text) => СontentProgressText.text = text;
        public void SetPortrait(Sprite portrait) => _portraitImage.sprite = portrait;
        public void SetBackground(Sprite bg) => _backgroundImage.sprite = bg;

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
            _skipLabelVisual.DOScale(1f, 0.2f).OnComplete(() => _shakeTween?.Play());
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

            Sequence pizzaSequence = DOTween.Sequence();
            pizzaSequence.Join(_skipLabelVisual.DOAnchorPosY(_skipLabelVisual.anchoredPosition.y + 150f, 0.6f).SetEase(Ease.OutQuad));
            pizzaSequence.Join(_skipLabelVisual.DORotate(new Vector3(360, 0, 180), 0.6f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
            pizzaSequence.Join(_skipLabelVisual.DOScale(0f, 0.6f).SetEase(Ease.InBack));
            pizzaSequence.Join(_skipLabelGroup.DOFade(0f, 0.4f).SetDelay(0.2f));

            pizzaSequence.SetLink(_skipLabelVisual.gameObject);
        }

        private void StopSkipAnims()
        {
            _shakeTween?.Kill();
            _holdTween?.Kill();
        }
    }
}