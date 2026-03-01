using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Core.Buttons;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Endgame
{
    public class WinMenuPopupView : PopupViewBase
    {
        public event Action PillageButtonClicked;
        public event Action RestartButtonClicked;

        [SerializeField] private HoldButtonHandler _pillageHoldHandler;
        [SerializeField] private HoldButtonHandler _restartHoldHandler;

        private void OnEnable()
        {
            _pillageHoldHandler.OnHoldComplete += OnPillageHoldComplete;
            _restartHoldHandler.OnHoldComplete += OnRestartHoldComplete;
        }

        private void OnDisable()
        {
            _pillageHoldHandler.OnHoldComplete -= OnPillageHoldComplete;
            _restartHoldHandler.OnHoldComplete -= OnRestartHoldComplete;
        }

        private void OnPillageHoldComplete() => PillageButtonClicked?.Invoke();
        private void OnRestartHoldComplete() => RestartButtonClicked?.Invoke();
    }
}