using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.HealthDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.ResultPopups;
using Assets._Project.Develop.Runtime.UI.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
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

        public LevelProgressPresenter CreateLevelProgressPresenter(BarWithText view)
        {
            return new LevelProgressPresenter(
                view,
                _container.Resolve<LevelProgressService>());
        }

        public EntitiesHealthDisplayPresenter CreateEntitiesHealthDisplayPresenter(EntitiesHealthDisplay view)
        {
            return new EntitiesHealthDisplayPresenter(
                _container.Resolve<EntitiesLifeContext>(),
                view,
                _container.Resolve<ViewsFactory>(),
                this
                );
        }

        public EntityHealthPresenter CreateEntityHealthPresenter(Entity entity, BarWithText view)
        {
            return new EntityHealthPresenter(view, entity);
        }

        public WinPopupPresenter CreateWinPopupPresenter(WinPopupView view)
        {
            return new WinPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                view,
                _container.Resolve<SceneSwitcherService>()
                );
        }

        public DefeatPopupPresenter CreateDefeatPopupPresenter(DefeatPopupView view)
        {
            return new DefeatPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                view,
                _container.Resolve<SceneSwitcherService>(),
                _inputArgs
                );
        }

        public GameplayScreenPresenter CreateGameplayScreen(GameplayScreenView view)
        {
            return new GameplayScreenPresenter(
                view, 
                _container.Resolve<GameplayPresentersFactory>()
                );
        }

        public StagePresenter CreateStagePresenter(IconTextView view)
        {
            return new StagePresenter(
                view,
                _container.Resolve<StageProviderService>()
                );
        }
    }
}