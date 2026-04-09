using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.MainMenu;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System;
using UnityEditor;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{

    public class GameSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly GameSettingsPopupView _view;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly PopupService _popupService;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly IInputService _input;

        protected override PopupViewBase PopupView => _view;

        public GameSettingsPopupPresenter
            (GameSettingsPopupView view, 
            ICoroutinesPerformer coroutinesPerformer,
            PopupService popupService,
            SceneSwitcherService sceneSwitcherService,
            IInputService input) : base(coroutinesPerformer)
        {
            _view = view;
            _coroutinesPerformer = coroutinesPerformer;
            _popupService = popupService;
            _sceneSwitcherService = sceneSwitcherService;
            _input = input;
        }

        protected override void OnPreShow()
        {
            _input.IsEnabled = false;
            base.OnPreShow();
        }

        protected override void OnPostHide()
        {
            _input.IsEnabled = true;
            base.OnPostHide();
        }

        public override void Initialize()
        {
            base.Initialize();

            _view.OpenAudioSettings.onClick.AddListener(OnOpenAudioSettingsButtonClicked);
            _view.OpenKeyBindingsSettings.onClick.AddListener(OnOpenKeyBindingsSettingsButtonClicked);

            if (_popupService is MainMenuPopupService)
                _view.ExitGameButton.onClick.AddListener(OnExitGameButtonClicked);
            else if (_popupService is GameplayPopupService)
                _view.ExitGameButton.onClick.AddListener(OnExitToMainMenuGameButtonClicked);
        }

        private void OnOpenKeyBindingsSettingsButtonClicked()
        {
            _popupService.OpenKeyBindingsSettingsPopup();
        }

        private void OnExitToMainMenuGameButtonClicked()
        {
            _popupService.OpenConfirmPopup(HandleExitToMainMenu, "Exit in main menu?\nR u give up your order?");
        }

        private void HandleExitToMainMenu()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.OpenAudioSettings.onClick.RemoveListener(OnOpenAudioSettingsButtonClicked);
            _view.OpenKeyBindingsSettings.onClick.RemoveListener(OnOpenKeyBindingsSettingsButtonClicked);

            if (_popupService is MainMenuPopupService)
                _view.ExitGameButton.onClick.RemoveListener(OnExitGameButtonClicked);
            else if (_popupService is GameplayPopupService)
                _view.ExitGameButton.onClick.RemoveListener(OnExitToMainMenuGameButtonClicked);
        }

        private void OnOpenAudioSettingsButtonClicked()
        {
            _popupService.OpenAudioSettingsPopup();
        }

        private void OnExitGameButtonClicked()
        {
            _popupService.OpenConfirmPopup(HandleExitGame, "Exit game?\nR u pussy?");
        }

        private void HandleExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}