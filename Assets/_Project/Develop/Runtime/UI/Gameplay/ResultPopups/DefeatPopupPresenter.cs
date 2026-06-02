using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEditor.U2D.Aseprite;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultPopups
{
    public class DefeatPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "DEFEAT";

        private readonly DefeatPopupView _view;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayInputArgs _inputArgs;

        public DefeatPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            DefeatPopupView view,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs inputArgs) : base(coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _view = view;
            _sceneSwitcherService = sceneSwitcherService;
            _inputArgs = inputArgs;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.SetTitle(TitleName);

            _view.ContinueClicked += OnContinueClicked;
            _view.RestartClicked += OnRestartClicked;
        }


        protected override void OnPreHide()
        {
            base.OnPreHide();

            _view.ContinueClicked -= OnContinueClicked;
            _view.RestartClicked -= OnRestartClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.ContinueClicked -= OnContinueClicked;
            _view.RestartClicked -= OnRestartClicked;
        }

        private void OnRestartClicked()
        {
            _inputArgs.IsRestart = true;

            OnCloseRequest();
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, _inputArgs));
        }

        private void OnContinueClicked()
        {
            OnCloseRequest();

            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
        }
    }
}
