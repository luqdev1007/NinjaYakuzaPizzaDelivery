using Assets._Project.Develop.Runtime.UI.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    /// <summary>
    /// Карточка товара. Знает только как себя нарисовать: что показывать и
    /// когда гасить кнопку — решает ShopItemPresenter.
    ///
    /// Публичного геттера валюты здесь нет намеренно: фильтрацией вкладок
    /// рулит ShopPresenter по конфигу, а не по состоянию вьюхи. Иначе валюта
    /// товара имела бы двух хранителей — конфиг и карточку.
    /// </summary>
    public class ShopItemView : MonoBehaviour, IView
    {
        private const string MaxedCostText = "MAX";

        public event Action Clicked;

        [SerializeField] private Button _button;
        [SerializeField] private Image _borderImage;
        [SerializeField] private Image _itemIconImage;
        [SerializeField] private Image _costIcon;
        [SerializeField] private TMP_Text _itemNameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _tierInfoText;
        [SerializeField] private TMP_Text _costText;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Init(string itemName, string description, Sprite icon)
        {
            _itemNameText.text = itemName;
            _descriptionText.text = description;

            SetIcon(_itemIconImage, icon);
        }

        public void SetCurrencyIcon(Sprite icon)
        {
            SetIcon(_costIcon, icon);
        }

        public void SetTierInfo(string text)
        {
            _tierInfoText.text = text;
        }

        public void SetCost(int cost)
        {
            _costText.text = cost.ToString();
        }

        public void SetAvailable()
        {
            _button.interactable = true;
            _borderImage.color = Color.white;
        }

        public void SetBlock()
        {
            _button.interactable = false;
            _borderImage.color = Color.red;
        }

        public void SetMaxed()
        {
            _button.interactable = false;
            _borderImage.color = Color.green;
            _costText.text = MaxedCostText;
        }

        /// <summary>
        /// Иконка может быть не проставлена в конфиге (арт плейсхолдерный) —
        /// тогда гасим сам Image, а не оставляем белый квадрат дефолтного
        /// спрайта.
        /// </summary>
        private void SetIcon(Image image, Sprite icon)
        {
            image.sprite = icon;
            image.enabled = icon != null;
        }

        private void OnButtonClicked() => Clicked?.Invoke();
    }
}
