using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootFeedbackView : MonoBehaviour, IView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private CanvasGroup _canvasGroup;

        public void Show(Sprite icon, int amount)
        {
            _icon.sprite = icon;
            _amountText.text = $"+{amount}";

            // Сочная анимация появления
            transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
            seq.Join(_canvasGroup.DOFade(1, 0.2f));
            seq.Append(transform.DOScale(1f, 0.1f));

            // Улетает вверх и исчезает через 1.5 сек
            seq.AppendInterval(1.2f);
            seq.Append(transform.DOMoveY(transform.position.y + 50f, 0.5f));
            seq.Join(_canvasGroup.DOFade(0, 0.5f));
            seq.OnComplete(() => Destroy(gameObject)); // Или возвращаем в пул
        }
    }
}