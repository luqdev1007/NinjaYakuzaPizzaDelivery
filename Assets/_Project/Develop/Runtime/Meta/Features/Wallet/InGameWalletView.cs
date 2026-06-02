using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Wallet
{
    public class InGameWalletView : MonoBehaviour
    {
        [Serializable]
        public struct CurrencyUIMap
        {
            public CurrencyTypes Type;
            public TextMeshProUGUI Text;
            public RectTransform Container; // Для анимации
        }

        [SerializeField] private List<CurrencyUIMap> _currencyElements;

        private Dictionary<CurrencyTypes, CurrencyUIMap> _map;

        private void Awake()
        {
            _map = new Dictionary<CurrencyTypes, CurrencyUIMap>();

            foreach (var element in _currencyElements)
                _map[element.Type] = element;
        }

        public void UpdateCurrency(CurrencyTypes type, int total)
        {
            if (_map.TryGetValue(type, out var element))
                element.Text.text = total.ToString();
        }

        public void PlayCollectEffect(CurrencyTypes type)
        {
            if (_map.TryGetValue(type, out var element))
            {
                // Здесь будет простая анимация через Animator или DOTween
                // Пока просто затычка для понимания логики
                Debug.Log($"UI Effect for {type}!");
            }
        }
    }
}