using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay
{
    public class LivesCountView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private float _displayDuration = 2f;

        private Sequence _fadeSequence;

        public void Show(int currentLives)
        {
            _livesText.text = currentLives.ToString();

            // Сбрасываем старую анимацию, если она шла
            _fadeSequence?.Kill();

            _fadeSequence = DOTween.Sequence();

            // Появление + легкий рывок вверх (Scale или Jump)
            _fadeSequence.Append(_canvasGroup.DOFade(1, 0.2f));
            _fadeSequence.Join(transform.DOScale(1.2f, 0.2f).OnComplete(() => transform.DOScale(1f, 0.1f)));

            // Задержка и плавное исчезновение
            _fadeSequence.AppendInterval(_displayDuration);
            _fadeSequence.Append(_canvasGroup.DOFade(0, 0.5f));
        }
    }
}