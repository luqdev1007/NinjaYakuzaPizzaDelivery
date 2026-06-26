using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay
{
    public class RankStyleView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rankLetterText;
        [SerializeField] private TextMeshProUGUI _prefixText;
        [SerializeField] private Slider _styleProgressSlider;
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private CanvasGroup _pointsCanvasGroup;

        [SerializeField] private Image _rankGlowImage;
        [SerializeField] private Image _sliderFillImage;

        [SerializeField] private TextMeshProUGUI _pointsPopupPrefab;

        [SerializeField] private Image _decayRingImage;
        [SerializeField] private Color _decayRingIdleColor = new Color(1f, 1f, 1f, 0.15f);
        [SerializeField] private Color _decayRingActiveColor = new Color(1f, 0.15f, 0.15f, 1f);

        private const float PopupSpawnOffsetY = -28f;
        private const float PopupFloatDistance = 36f;

        private Sequence _rankChangeSequence;
        private Tween _sliderTween;
        private Tween _pointsTween;
        private Tween _decayPulseTween;

        private float _displayedPoints;
        private float _lastRawPoints;
        private bool _isPointsVisible;

        private void Awake()
        {
            _pointsCanvasGroup.alpha = 0f;
            _pointsText.text = "0";
            _displayedPoints = 0f;
            _lastRawPoints = 0f;

            if (_decayRingImage != null)
            {
                _decayRingImage.fillAmount = 0f;
                _decayRingImage.color = _decayRingIdleColor;
            }
        }

        public void SetPoints(float points)
        {
            bool shouldBeVisible = points > 0.5f;

            if (shouldBeVisible != _isPointsVisible)
            {
                _isPointsVisible = shouldBeVisible;
                _pointsCanvasGroup.DOKill();
                _pointsCanvasGroup.DOFade(shouldBeVisible ? 1f : 0f, shouldBeVisible ? 0.3f : 0.5f);
            }

            if (points > _lastRawPoints + 0.01f)
            {
                _pointsText.transform.DOKill();
                _pointsText.transform.localScale = Vector3.one;
                _pointsText.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.18f, 4, 0.5f);
            }
            _lastRawPoints = points;

            _pointsTween?.Kill();
            _pointsTween = DOTween.To(() => _displayedPoints, x => _displayedPoints = x, points, 0.25f)
                .OnUpdate(() => _pointsText.text = Mathf.FloorToInt(_displayedPoints).ToString());
        }

        public void SetRank(string letter, string prefix, Color accentColor)
        {
            if (_rankLetterText.text != letter)
            {
                AnimateRankChange(letter, prefix, accentColor);
            }
            else
            {
                _prefixText.text = prefix;
            }

            if (_sliderFillImage != null)
            {
                _sliderFillImage.DOKill();
                _sliderFillImage.DOColor(accentColor, 0.3f);
            }
        }

        private void AnimateRankChange(string letter, string prefix, Color accentColor)
        {
            _rankChangeSequence?.Kill();
            _rankChangeSequence = DOTween.Sequence();

            _rankLetterText.transform.localScale = Vector3.one;
            _rankLetterText.transform.localRotation = Quaternion.identity;

            _rankChangeSequence
                .Append(_rankLetterText.transform.DOScale(1.5f, 0.12f).SetEase(Ease.OutQuad))
                .Join(_rankLetterText.transform.DORotate(new Vector3(0f, 0f, -12f), 0.12f).SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    _rankLetterText.text = letter;
                    _prefixText.text = prefix;
                    _rankLetterText.color = accentColor;
                })
                .Append(_rankLetterText.transform.DOScale(1f, 0.22f).SetEase(Ease.OutBack))
                .Join(_rankLetterText.transform.DORotate(Vector3.zero, 0.22f).SetEase(Ease.OutBack));

            if (_rankGlowImage != null)
            {
                _rankGlowImage.transform.localScale = Vector3.one;
                _rankGlowImage.DOKill();
                _rankGlowImage.transform.DOKill();
                _rankGlowImage.color = accentColor;
                _rankGlowImage.transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0.4f), 0.3f, 8, 0.6f);
                _rankGlowImage.DOFade(1f, 0.08f).OnComplete(() => _rankGlowImage.DOFade(0.35f, 0.5f));
            }
        }

        public void SetProgress(float current, float floor, float ceiling)
        {
            float normalized;

            if (ceiling <= floor)
            {
                normalized = 1f;
            }
            else
            {
                normalized = Mathf.Clamp01((current - floor) / (ceiling - floor));
            }

            _styleProgressSlider.minValue = 0f;
            _styleProgressSlider.maxValue = 1f;

            _sliderTween?.Kill();
            _sliderTween = _styleProgressSlider.DOValue(normalized, 0.2f).SetEase(Ease.OutQuad);
        }

        public void SetDecayWarning(float normalized01)
        {
            if (_decayRingImage == null)
            {
                return;
            }

            DOTween.To(() => _decayRingImage.fillAmount, x => _decayRingImage.fillAmount = x, normalized01, 0.2f)
                .SetEase(Ease.Linear);
        }

        public void SetDecayActive(bool isDecaying)
        {
            _decayPulseTween?.Kill();

            if (_decayRingImage == null)
            {
                return;
            }

            if (!isDecaying)
            {
                _decayRingImage.DOColor(_decayRingIdleColor, 0.2f);
                return;
            }

            _decayRingImage.color = _decayRingActiveColor;
            _decayPulseTween = _decayRingImage
                .DOFade(0.4f, 0.35f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        public void PlayPointsGained(float amount)
        {
            if (_pointsPopupPrefab == null)
            {
                return;
            }

            RectTransform pointsRect = _pointsText.rectTransform;
            var popup = Instantiate(_pointsPopupPrefab, pointsRect.parent);
            popup.text = "+" + Mathf.RoundToInt(amount);
            popup.alpha = 1f;

            RectTransform rect = popup.rectTransform;
            Vector2 spawnPos = pointsRect.anchoredPosition + new Vector2(0f, PopupSpawnOffsetY);
            rect.anchoredPosition = spawnPos;

            float randomXOffset = Random.Range(-12f, 12f);
            Vector2 targetPos = spawnPos + new Vector2(randomXOffset, PopupFloatDistance);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutCubic));
            sequence.Join(popup.DOFade(0f, 0.35f).SetDelay(0.15f));
            sequence.OnComplete(() => Destroy(popup.gameObject));
        }

        private void OnDestroy()
        {
            _rankChangeSequence?.Kill();
            _sliderTween?.Kill();
            _pointsTween?.Kill();
            _decayPulseTween?.Kill();
            _rankGlowImage?.DOKill();
            _decayRingImage?.DOKill();
        }
    }
}