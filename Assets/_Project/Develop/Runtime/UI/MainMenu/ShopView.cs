using Assets._Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    /// <summary>
    /// Экран магазина. Карточки в разметке не лежат — их создаёт ShopPresenter
    /// в ShopItemsListView, чьим _parent назначен пустой Content грида.
    /// </summary>
    public class ShopView : MonoBehaviour, IView
    {
        [field: SerializeField] public ShopItemsListView ItemsListView { get; private set; }

        [field: SerializeField] public Button BackButton { get; private set; }

        [field: SerializeField] public Button SwitchCurrencyButton { get; private set; }

        [SerializeField] private Image _switchCurrencyIcon;
        [SerializeField] private TMP_Text _switchCurrencyLabel;

        public void SetCurrencyIcon(Sprite icon)
        {
            _switchCurrencyIcon.sprite = icon;
            _switchCurrencyIcon.enabled = icon != null;
        }

        public void SetCurrencyLabel(string text)
        {
            _switchCurrencyLabel.text = text;
        }
    }
}
