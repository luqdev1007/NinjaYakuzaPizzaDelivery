using Assets._Project.Develop.Runtime.UI.Core;
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

        [field: SerializeField] public CurrencySwitchView CurrencySwitchView { get; private set; }

        /// <summary>
        /// Кнопка переключения живёт внутри виджета вместе со своей анимацией.
        /// Проксируем её сюда, чтобы презентер подписывался на все кнопки экрана
        /// одинаково и не лез в потроха виджета.
        /// </summary>
        public Button SwitchCurrencyButton => CurrencySwitchView.SwitchButton;
    }
}
