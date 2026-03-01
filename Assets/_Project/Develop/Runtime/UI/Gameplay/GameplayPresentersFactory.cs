using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay.Endgame;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPresentersFactory
    {
        private readonly DIContainer _container;
        private readonly GameplayInputArgs _inputArgs;

        public GameplayPresentersFactory(DIContainer container, GameplayInputArgs inputArgs)
        {
            _container = container;
            _inputArgs = inputArgs;
        }

        public GameplayScreenPresenter CreateGameplayScreen(GameplayScreenView view)
        {
            return new GameplayScreenPresenter(
                view,
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<SceneSwitcherService>()
                );
        }

        public DefeatMenuPopupPresenter CreateDefeatMenuPopupPresenter(DefeatMenuPopupView view)
        {
            return new DefeatMenuPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(), 
                view, 
                _inputArgs, 
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<SceneSwitcherService>());
        }

        public WinMenuPopupPresenter CreateWinMenuPopupPresenter(WinMenuPopupView view)
        {
            return new WinMenuPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                view,
                _inputArgs,
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<SceneSwitcherService>());
        }
    }
}