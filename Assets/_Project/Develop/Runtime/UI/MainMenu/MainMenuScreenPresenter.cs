using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenPresenter : IPresenter
    {
        private readonly MainMenuScreenView _view;
        private readonly MainMenuPopupService _popupService;
        private readonly ProjectPresentersFactory _projectPresentersFactory;

        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private List<IDisposable> _disposables = new();

        private readonly List<IPresenter> _childPresenters = new();

        public MainMenuScreenPresenter(
            MainMenuScreenView view,
            MainMenuPopupService popupService,
            SceneSwitcherService sceneSwitcherService,
            ICoroutinesPerformer coroutinesPerformer,
            ProjectPresentersFactory projectPresentersFactory)
        {
            _view = view;
            _popupService = popupService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _projectPresentersFactory = projectPresentersFactory;
        }

        public void Initialize()
        {
            CreateWallet();

            _view.StartGameButton.onClick.AddListener(StartGame);

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {

            _view.StartGameButton.onClick.RemoveListener(StartGame);

            foreach (var disposable in _disposables)
                disposable.Dispose();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _disposables.Clear();
        }

        private void CreateWallet()
        {
            WalletPresenter walletPresenter = _projectPresentersFactory.CreateWalletPresenter(_view.WalletView);

            _childPresenters.Add(walletPresenter);
        }

        private void StartGame()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, new GameplayInputArgs()));
        }
    }
}
