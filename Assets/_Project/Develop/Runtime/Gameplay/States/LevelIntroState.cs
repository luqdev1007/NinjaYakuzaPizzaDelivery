using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.States
{
    public class LevelIntroState : State, IUpdatableState
    {
        private readonly CameraService _cameraService;
        private readonly GameplayPopupService _popupService;
        private readonly GameplayUIRoot _uiRoot;
        private readonly DialogConfig _dialogConfig;

        private readonly bool _isRestart;

        private DialogPresenter _activeDialog;
        private bool _isFinished;

        public bool IsFinished => _isFinished;

        public LevelIntroState(
            CameraService cameraService,
            GameplayPopupService popupService,
            GameplayUIRoot uiRoot,
            DialogConfig dialogConfig,
            bool isRestart)
        {
            _cameraService = cameraService;
            _popupService = popupService;
            _uiRoot = uiRoot;
            _dialogConfig = dialogConfig;
            _isRestart = isRestart;
        }

        public override void Enter()
        {
            base.Enter();
            _isFinished = false;

            if (_isRestart)
            {
                _isFinished = true;
                return;
            }

            _uiRoot.HUDLayer.gameObject.SetActive(false);
            _cameraService.SetState(CameraState.Intro);
            _activeDialog = _popupService.OpenDialog(_dialogConfig, OnDialogEnded);
        }

        public void Update(float deltaTime)
        {
            _activeDialog?.Update(deltaTime);
        }

        public override void Exit()
        {
            _activeDialog?.Hide();
            _activeDialog = null;
            base.Exit();
        }

        private void OnDialogEnded()
        {
            _isFinished = true;
        }
    }
}