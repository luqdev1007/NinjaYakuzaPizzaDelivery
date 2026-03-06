using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.Stages;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;

        private EntitiesHealthDisplayPresenter _entitiesHealthDisplayPresenter;

        private readonly List<IPresenter> _childPresenters = new();

        public GameplayScreenPresenter(
            GameplayScreenView view,
            GameplayPresentersFactory gmeplayPresentersFactory)
        {
            _view = view;
            _gameplayPresentersFactory = gmeplayPresentersFactory;
        }

        public void Initialize()
        {
            CreateLevelProgress(); 
            CreateEntitiesHealthDisplay();

            foreach (IPresenter presenter in _childPresenters)
                presenter.Initialize();
        }

        public void Dispose()
        {
            foreach (IPresenter presenter in _childPresenters)
                presenter.Dispose();

            _childPresenters.Clear();
        }

        public void LateUpdate()
        {
            _entitiesHealthDisplayPresenter.LateUpdate();
        }

        private void CreateLevelProgress()
        {
            LevelProgressPresenter presenter = _gameplayPresentersFactory
                .CreateLevelProgressPresenter(_view.LevelProgressView);

            _childPresenters.Add(presenter);
        }

        private void CreateEntitiesHealthDisplay()
        {
            _entitiesHealthDisplayPresenter = _gameplayPresentersFactory
                .CreateEntitiesHealthDisplayPresenter(_view.EntitiesHealthDisplay);

            _childPresenters.Add(_entitiesHealthDisplayPresenter);
        }
    }
    
}
