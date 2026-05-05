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

            _fadeSequence?.Kill();
            _fadeSequence = DOTween.Sequence().SetLink(gameObject);

            _fadeSequence.Append(_canvasGroup.DOFade(1, 0.2f))
                         .Join(transform.DOScale(1.2f, 0.2f).OnComplete(() => transform.DOScale(1f, 0.1f)))
                         .AppendInterval(_displayDuration)
                         .Append(_canvasGroup.DOFade(0, 0.5f));
        }

        private void OnDestroy()
        {
            _fadeSequence?.Kill();
        }
    }
}