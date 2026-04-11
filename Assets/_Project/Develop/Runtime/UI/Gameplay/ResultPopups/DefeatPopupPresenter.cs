using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultPopups
{
    public class DefeatPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "DEFEAT";

        private readonly DefeatPopupView _view;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayInputArgs _inputArgs;
        private readonly WalletService _walletService;

        public DefeatPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            DefeatPopupView view,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs inputArgs,
            WalletService walletService) : base(coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _view = view;
            _sceneSwitcherService = sceneSwitcherService;
            _inputArgs = inputArgs;
            _walletService = walletService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _walletService.RollbackSessionLoot();

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
            // Помечаем, что следующий вход в сцену — это рестарт
            _inputArgs.IsRestart = true;

            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, _inputArgs));
            OnCloseRequest();
        }

        private void OnContinueClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
            OnCloseRequest();
        }
    }
}
