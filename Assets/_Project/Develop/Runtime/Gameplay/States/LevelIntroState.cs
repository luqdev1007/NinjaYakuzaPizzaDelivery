using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.States
{
    public class LevelIntroState : State, IUpdatableState
    {
        private readonly CameraService _cameraService;
        private readonly DialogPresenter _dialogPresenter;
        private readonly GameplayUIRoot _uiRoot;

        private bool _isFinished;

        public bool IsFinished => _isFinished;

        public LevelIntroState(
            CameraService cameraService,
            DialogPresenter dialogPresenter,
            GameplayUIRoot uiRoot)
        {
            _cameraService = cameraService;
            _dialogPresenter = dialogPresenter;
            _uiRoot = uiRoot;
        }

        public override void Enter()
        {
            base.Enter();
            _isFinished = false;

            _uiRoot.HUDLayer.gameObject.SetActive(false);
            _cameraService.SetState(CameraState.Intro);

            _dialogPresenter.DialogEnded += OnDialogEnded;
            _dialogPresenter.Initialize();
        }

        public void Update(float deltaTime)
        {
            _dialogPresenter.Update(deltaTime);
        }

        public override void Exit()
        {
            _dialogPresenter.DialogEnded -= OnDialogEnded;
            base.Exit();
        }

        private void OnDialogEnded()
        {
            _isFinished = true;
        }
    }
}