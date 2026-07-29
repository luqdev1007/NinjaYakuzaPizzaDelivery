using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    /// <summary>
    /// Переключатель валют: два статичных слота с иконками и круглая кнопка,
    /// которая ездит между ними. Кнопка стоит НАД активной валютой и, съезжая,
    /// открывает соседнюю — то есть видимая иконка это всегда то, куда можно
    /// переключиться.
    ///
    /// Иконки статичные, а не свапаемые: свап спрайта у одной иконки нечем
    /// анимировать — «переключение» читалось бы как мгновенная подмена без
    /// направления. Ездящая кнопка направление показывает.
    ///
    /// Целевые X берутся из RectTransform самих слотов, а не из констант в коде:
    /// раскладку двигают в префабе, и захардкоженные позиции разъехались бы с
    /// ней молча.
    /// </summary>
    public class CurrencySwitchView : MonoBehaviour, IView
    {
        [SerializeField] private RectTransform _slider;
        [SerializeField] private Button _switchButton;
        [SerializeField] private Image _leftIcon;
        [SerializeField] private Image _rightIcon;
        [SerializeField] private TMP_Text _label;

        [SerializeField, Min(0f)] private float _slideDuration = 0.25f;

        private Tween _slideTween;

        public Button SwitchButton => _switchButton;

        /// <summary>
        /// Сколько валют помещается в виджет. Слотов ровно два — это свойство
        /// разметки, и презентер должен знать о потолке, а не молча промахиваться
        /// мимо несуществующего третьего слота.
        /// </summary>
        public int SlotsCount => 2;

        public void SetSlotIcons(IReadOnlyList<Sprite> icons)
        {
            SetIcon(_leftIcon, GetIconAt(icons, 0));
            SetIcon(_rightIcon, GetIconAt(icons, 1));
        }

        public void SetLabel(string text)
        {
            _label.text = text;
        }

        /// <summary>
        /// Подвинуть кнопку к слоту. animated = false для первичной расстановки:
        /// на открытии магазина кнопка обязана уже стоять на месте, а не
        /// приезжать откуда-то у игрока на глазах.
        /// </summary>
        public void SlideTo(int slotIndex, bool animated)
        {
            _slideTween?.Kill();

            float targetX = GetSlotX(slotIndex);

            if (animated == false)
            {
                _slider.anchoredPosition = new Vector2(targetX, _slider.anchoredPosition.y);

                return;
            }

            // SetUpdate(true) — меню живёт и при timeScale = 0, как попапы и
            // тайлы уровней.
            _slideTween = _slider
                .DOAnchorPosX(targetX, _slideDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }

        public void Cleanup()
        {
            _slideTween?.Kill();
            _slider.DOKill();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private float GetSlotX(int slotIndex)
        {
            if (slotIndex <= 0)
            {
                return _leftIcon.rectTransform.anchoredPosition.x;
            }

            return _rightIcon.rectTransform.anchoredPosition.x;
        }

        private Sprite GetIconAt(IReadOnlyList<Sprite> icons, int index)
        {
            if (icons == null)
            {
                return null;
            }

            if (index >= icons.Count)
            {
                return null;
            }

            return icons[index];
        }

        /// <summary>
        /// Иконки валют могут быть не проставлены — гасим Image, а не оставляем
        /// белый квадрат дефолтного спрайта (та же логика, что в ShopItemView).
        /// </summary>
        private void SetIcon(Image image, Sprite icon)
        {
            image.sprite = icon;
            image.enabled = icon != null;
        }
    }
}
