using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class KeyBindingsSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly KeyBindingsSettingsPopupView _view;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly PopupService _popupService;

        protected override PopupViewBase PopupView => _view;

        public KeyBindingsSettingsPopupPresenter
            (KeyBindingsSettingsPopupView view,
            ICoroutinesPerformer coroutinesPerformer,
            PopupService popupService) : base(coroutinesPerformer)
        {
            _view = view;
            _coroutinesPerformer = coroutinesPerformer;
            _popupService = popupService;
        }

        public override void Initialize()
        {
            base.Initialize();

            _view.ResetBindsButton.onClick.AddListener(OnResetButtonClicked);
        }

        private void OnResetButtonClicked()
        {
            _popupService.OpenConfirmPopup(() => Debug.Log("Binds reseted!"), "Reset All Binds?");
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.ResetBindsButton.onClick.RemoveListener(OnResetButtonClicked);
        }
    }
}