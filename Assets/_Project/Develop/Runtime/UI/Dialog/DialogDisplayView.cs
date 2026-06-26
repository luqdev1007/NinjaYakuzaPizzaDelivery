using Assets._Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogDisplayView : PopupViewBase
    {
        [field: SerializeField] public TMP_Text ContentProgressText { get; private set; }

        [SerializeField] private SkipLabelView _skipLabel;
        [SerializeField] private RectTransform _topCinemaLine;
        [SerializeField] private RectTransform _bottomCinemaLine;
        [SerializeField] private RectTransform _backgroundGlow;
        [SerializeField] private RectTransform _portraitRoot;
        [SerializeField] private Image _portraitImage;
        [SerializeField] private Image _backgroundImage;

        private Tween _typewriterTween;

        private const float TextSpeed = 0.03f;
        private const float LineOffset = 2500f;

        public SkipLabelView SkipLabel => _skipLabel;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            KillAllAnimations();

            if (ContentProgressText != null)
            {
                ContentProgressText.text = string.Empty;
                ContentProgressText.alpha = 1f;
            }

            _skipLabel?.Initialize();

            if (_topCinemaLine != null)
                _topCinemaLine.anchoredPosition = new Vector2(-LineOffset, _topCinemaLine.anchoredPosition.y);

            if (_bottomCinemaLine != null)
                _bottomCinemaLine.anchoredPosition = new Vector2(LineOffset, _bottomCinemaLine.anchoredPosition.y);

            if (_backgroundGlow != null)
                _backgroundGlow.localScale = new Vector3(0, 1, 1);

            if (_portraitRoot != null)
                _portraitRoot.localScale = Vector3.zero;
        }

        public Sequence PlayAppearance()
        {
            KillAllAnimations();
            Sequence seq = DOTween.Sequence();

            if (_topCinemaLine != null)
                seq.Append(_topCinemaLine.DOAnchorPosX(0, 0.7f).SetEase(Ease.OutCubic));

            if (_bottomCinemaLine != null)
                seq.Join(_bottomCinemaLine.DOAnchorPosX(0, 0.7f).SetEase(Ease.OutCubic));

            if (_backgroundGlow != null)
                seq.Append(_backgroundGlow.DOScaleX(1f, 0.4f).SetEase(Ease.OutQuad));

            if (_portraitRoot != null)
                seq.Append(_portraitRoot.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

            seq.AppendCallback(() => _skipLabel?.Show());

            return seq.SetUpdate(true);
        }

        public Sequence PlayDisappearance()
        {
            KillAllAnimations();

            if (_skipLabel != null)
                _skipLabel.Hide();

            Sequence seq = DOTween.Sequence();

            if (_portraitRoot != null)
                seq.Append(_portraitRoot.DOScale(0f, 0.25f).SetEase(Ease.InBack));

            if (ContentProgressText != null)
                seq.Join(ContentProgressText.DOFade(0f, 0.2f));

            if (_backgroundGlow != null)
                seq.Append(_backgroundGlow.DOScaleX(0f, 0.3f).SetEase(Ease.InQuad));

            if (_topCinemaLine != null)
                seq.Append(_topCinemaLine.DOAnchorPosX(-LineOffset, 0.4f).SetEase(Ease.InCubic));

            if (_bottomCinemaLine != null)
                seq.Join(_bottomCinemaLine.DOAnchorPosX(LineOffset, 0.4f).SetEase(Ease.InCubic));

            return seq.SetUpdate(true);
        }

        public void SetText(string text, System.Action onComplete)
        {
            if (ContentProgressText == null)
                return;

            _typewriterTween?.Kill();
            ContentProgressText.text = text;
            ContentProgressText.maxVisibleCharacters = 0;

            _typewriterTween = DOTween.To(() => ContentProgressText.maxVisibleCharacters,
                x => ContentProgressText.maxVisibleCharacters = x,
                text.Length,
                text.Length * TextSpeed)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void ClearText()
        {
            _typewriterTween?.Kill();

            if (ContentProgressText != null)
                ContentProgressText.text = string.Empty;
        }

        public void FinishTypingInstant()
        {
            _typewriterTween?.Kill();

            if (ContentProgressText != null)
                ContentProgressText.maxVisibleCharacters = ContentProgressText.text.Length;
        }

        public void SetPortrait(Sprite portrait)
        {
            if (_portraitImage != null)
                _portraitImage.sprite = portrait;
        }

        public void SetBackground(Sprite bg)
        {
            if (_backgroundImage != null)
                _backgroundImage.sprite = bg;
        }

        private void KillAllAnimations()
        {
            if (_topCinemaLine != null)
                _topCinemaLine.DOKill();

            if (_bottomCinemaLine != null)
                _bottomCinemaLine.DOKill();

            if (_backgroundGlow != null)
                _backgroundGlow.DOKill();

            if (_portraitRoot != null)
                _portraitRoot.DOKill();

            if (ContentProgressText != null)
                ContentProgressText.DOKill();

            _typewriterTween?.Kill();
        }

        private void OnDestroy() => KillAllAnimations();
    }
}