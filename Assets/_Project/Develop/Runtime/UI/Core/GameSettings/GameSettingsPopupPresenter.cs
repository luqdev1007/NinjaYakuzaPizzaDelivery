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

        protected override PopupViewBase PopupView => _view;

        public GameSettingsPopupPresenter
            (GameSettingsPopupView view, 
            ICoroutinesPerformer coroutinesPerformer,
            PopupService popupService,
            SceneSwitcherService sceneSwitcherService) : base(coroutinesPerformer)
        {
            _view = view;
            _coroutinesPerformer = coroutinesPerformer;
            _popupService = popupService;
            _sceneSwitcherService = sceneSwitcherService;
        }

        public override void Initialize()
        {
            base.Initialize();

            _view.OpenAudioSettings.onClick.AddListener(OnOpenAudioSettingsButtonClicked);

            if (_popupService is MainMenuPopupService)
                _view.ExitGameButton.onClick.AddListener(OnExitGameButtonClicked);
            else if (_popupService is GameplayPopupService)
                _view.ExitGameButton.onClick.AddListener(OnExitToMainMenuGameButtonClicked);
        }

        private void OnExitToMainMenuGameButtonClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.OpenAudioSettings.onClick.RemoveListener(OnOpenAudioSettingsButtonClicked);

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
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}