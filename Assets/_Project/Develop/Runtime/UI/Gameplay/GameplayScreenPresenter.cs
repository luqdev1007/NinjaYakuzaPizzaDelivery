using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;

        // services 
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly SceneSwitcherService _sceneSwitcherService;

        private List<IDisposable> _disposables = new();

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView view,
            ICoroutinesPerformer coroutinesPerformer,
            SceneSwitcherService sceneSwitcherService)
        {
            _view = view;

            // services 
            _coroutinesPerformer = coroutinesPerformer;
            _sceneSwitcherService = sceneSwitcherService;
        }

        public void Initialize()
        {
            _view.Init();

            _view.BackToMenuButton.onClick.AddListener(BackToMenu);
            
            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {
            _view.BackToMenuButton.onClick.RemoveListener(BackToMenu);

            foreach (var disposable in _disposables)
                disposable.Dispose();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _disposables.Clear();
        }

        private void BackToMenu()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
        }
    }
}