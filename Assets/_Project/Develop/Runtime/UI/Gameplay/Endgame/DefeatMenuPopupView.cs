using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Core.Buttons;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Endgame
{
    public class DefeatMenuPopupView : PopupViewBase
    {
        public event Action ConcedeButtonClicked;
        public event Action RestartButtonClicked;
        public event Action TradeButtonClicked;

        [Header("Hold Handlers")]
        [SerializeField] private HoldButtonHandler _concedeHoldHandler;
        [SerializeField] private HoldButtonHandler _tradeHoldHandler;
        [SerializeField] private HoldButtonHandler _restartHoldHandler;

        private void OnEnable()
        {
            _concedeHoldHandler.OnHoldComplete += OnConcedeHoldComplete;
            _restartHoldHandler.OnHoldComplete += OnRestartHoldComplete;
            _tradeHoldHandler.OnHoldComplete += OnTradeHoldComplete;
        }

        private void OnDisable()
        {
            _concedeHoldHandler.OnHoldComplete -= OnConcedeHoldComplete;
            _restartHoldHandler.OnHoldComplete -= OnRestartHoldComplete;
            _tradeHoldHandler.OnHoldComplete -= OnTradeHoldComplete;
        }

        private void OnConcedeHoldComplete() => ConcedeButtonClicked?.Invoke();
        private void OnRestartHoldComplete() => RestartButtonClicked?.Invoke();
        private void OnTradeHoldComplete() => TradeButtonClicked?.Invoke();
    }
}