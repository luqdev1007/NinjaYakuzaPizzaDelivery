using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Buffs
{
    public class BuffView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _rootGroup;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _radialFillImage;
        [SerializeField] private TextMeshProUGUI _timeText;

        private Tween _fillTween;
        private Sequence _appearSequence;
        private Sequence _disappearSequence;

        public void SetIcon(Sprite icon)
        {
            if (_iconImage != null)
            {
                _iconImage.sprite = icon;
            }
        }

        public void SetProgress(float normalized01)
        {
            if (_radialFillImage == null)
            {
                return;
            }

            _fillTween?.Kill();
            _fillTween = _radialFillImage.DOFillAmount(normalized01, 0.2f).SetEase(Ease.Linear);
        }

        public void SetTimeText(string text)
        {
            if (_timeText != null)
            {
                _timeText.text = text;
            }
        }

        public void PlayExtendedPulse()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.25f, 6, 0.6f);
        }

        public void PlayAppear()
        {
            _rootGroup.alpha = 0f;
            transform.localScale = Vector3.zero;

            _appearSequence?.Kill();
            _appearSequence = DOTween.Sequence()
                .Append(transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack))
                .Join(_rootGroup.DOFade(1f, 0.2f));
        }

        public void PlayDisappearAndDestroy()
        {
            _disappearSequence?.Kill();
            _disappearSequence = DOTween.Sequence()
                .Append(transform.DOScale(0f, 0.2f).SetEase(Ease.InBack))
                .Join(_rootGroup.DOFade(0f, 0.15f))
                .OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            _fillTween?.Kill();
            _appearSequence?.Kill();
            _disappearSequence?.Kill();
        }
    }
}