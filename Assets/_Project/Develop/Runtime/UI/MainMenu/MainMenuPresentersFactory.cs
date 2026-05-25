using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Core.GameSettings;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using System;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuPresentersFactory
    {
        private readonly DIContainer _container;

        public MainMenuPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public MainMenuScreenPresenter CreateMainMenuScreen(MainMenuScreenView view)
        {
            return new MainMenuScreenPresenter(
                view,
                _container.Resolve<MainMenuPopupService>(),
                _container.Resolve<ConfigsProviderService>(),
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<ProjectPresentersFactory>(),
                _container.Resolve<ICoroutinesPerformer>()
                );
        }

        public LoadMusicPopupPresenter CreateLoadMusicPopupPresenter(LoadMusicPopupView view)
        {
            return new LoadMusicPopupPresenter(view, _container.Resolve<ICoroutinesPerformer>());
        }
    }
}
