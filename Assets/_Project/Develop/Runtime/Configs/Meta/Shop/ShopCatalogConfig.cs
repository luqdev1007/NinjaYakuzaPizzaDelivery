using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Meta.Shop
{
    /// <summary>
    /// Состав витрины: какие товары и в каком порядке показывает магазин.
    ///
    /// Список, а не автосбор всех ассетов ShopItemConfigBase из Resources:
    /// порядок карточек — дизайнерское решение, а сборка «всего, что нашлось»
    /// молча вытащила бы на витрину любой черновой конфиг. По той же причине
    /// каталог не знает про валюты — фильтрация вкладок целиком на презентере,
    /// который читает Currency у каждого товара.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Configs/Meta/Shop/New Shop Catalog Config",
        fileName = "ShopCatalogConfig",
        order = 57)]
    public class ShopCatalogConfig : ScriptableObject
    {
        [SerializeField] private List<ShopItemConfigBase> _items = new();

        public IReadOnlyList<ShopItemConfigBase> Items => _items;
    }
}
