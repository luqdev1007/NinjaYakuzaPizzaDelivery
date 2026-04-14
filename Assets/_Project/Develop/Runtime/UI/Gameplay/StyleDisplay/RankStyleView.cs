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
        [SerializeField] private CanvasGroup _pointsCanvasGroup; // Добавь этот компонент на текст очков

        private Sequence _rankChangeSequence;
        private Tween _sliderTween;
        private Tween _pointsTween;
        private float _displayedPoints;
        private bool _isPointsVisible;

        private void Awake()
        {
            // Скрываем очки на старте
            _pointsCanvasGroup.alpha = 0f;
            _pointsText.text = "0";
            _displayedPoints = 0f;
        }

        public void SetPoints(float points)
        {
            bool shouldBeVisible = points > 0.01f;

            if (shouldBeVisible != _isPointsVisible)
            {
                _isPointsVisible = shouldBeVisible;
                _pointsCanvasGroup.DOKill(); // Убиваем старый фейд
                _pointsCanvasGroup.DOFade(shouldBeVisible ? 1f : 0f, 0.4f);

                // Если скрываем, можно еще и букву чуть притушить (опционально)
                _rankLetterText.transform.parent.gameObject.SetActive(true); // Убедись, что сам объект не выключен
            }

            // Управление видимостью
            if (points > 0.1f && !_isPointsVisible)
            {
                _isPointsVisible = true;
                _pointsCanvasGroup.DOFade(1f, 0.3f);
            }
            else if (points <= 0.1f && _isPointsVisible)
            {
                _isPointsVisible = false;
                _pointsCanvasGroup.DOFade(0f, 0.5f);
            }

            // Плавный счетчик
            _pointsTween?.Kill();
            _pointsTween = DOTween.To(() => _displayedPoints, x => _displayedPoints = x, points, 0.25f)
                .OnUpdate(() => _pointsText.text = Mathf.FloorToInt(_displayedPoints).ToString());

            // Аккуратная анимация при начислении (вместо дикого пунша)
            _pointsText.transform.DOKill(); // Сбрасываем старые анимации трансформа
            _pointsText.transform.localScale = Vector3.one;
            _pointsText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f, 5, 0.5f);
        }

        public void SetRank(string letter, string prefix)
        {
            if (_rankLetterText.text != letter)
            {
                AnimateRankChange(letter, prefix);
            }
            else
            {
                _prefixText.text = prefix;
            }
        }

        private void AnimateRankChange(string letter, string prefix)
        {
            _rankChangeSequence?.Kill();
            _rankChangeSequence = DOTween.Sequence();

            // Сброс состояния перед новой анимацией
            _rankLetterText.transform.localScale = Vector3.one;

            _rankChangeSequence.Append(_rankLetterText.transform.DOScale(1.4f, 0.15f).SetEase(Ease.OutBack))
                .AppendCallback(() =>
                {
                    _rankLetterText.text = letter;
                    _prefixText.text = prefix;
                })
                .Append(_rankLetterText.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack))
                .Join(_rankLetterText.DOColor(Color.yellow, 0.15f).OnComplete(() => _rankLetterText.DOColor(Color.white, 0.4f)));
        }

        public void SetProgress(float current, float max)
        {
            _styleProgressSlider.maxValue = max;
            _sliderTween?.Kill();
            _sliderTween = _styleProgressSlider.DOValue(current, 0.2f).SetEase(Ease.Linear);
        }

        private void OnDestroy()
        {
            _rankChangeSequence?.Kill();
            _sliderTween?.Kill();
            _pointsTween?.Kill();
        }
    }
}