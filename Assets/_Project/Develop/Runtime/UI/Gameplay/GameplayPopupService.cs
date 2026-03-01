using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.Endgame;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPopupService : PopupService
    {
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;
        private readonly GameplayUIRoot _uiRoot;

        public GameplayPopupService(
            ViewsFactory viewsFactory,
            ProjectPresentersFactory presentersFactory,
            GameplayUIRoot uiRoot,
            GameplayPresentersFactory gameplayPresentersFactory)
            : base(viewsFactory, presentersFactory)
        {
            _gameplayPresentersFactory = gameplayPresentersFactory;
            _uiRoot = uiRoot;
        }

        protected override Transform PopupLayer => _uiRoot.PopupsLayer;

        public DefeatMenuPopupPresenter OpenDefeatMenuPopup()
        {
            DefeatMenuPopupView view = ViewsFactory.Create<DefeatMenuPopupView>(ViewIDs.DefeatMenuPopupView, PopupLayer);

            DefeatMenuPopupPresenter popup = _gameplayPresentersFactory.CreateDefeatMenuPopupPresenter(view);

            OnPopupCreated(popup, view);

            return popup;
        }

        public WinMenuPopupPresenter OpenWinMenuPopup()
        {
            WinMenuPopupView view = ViewsFactory.Create<WinMenuPopupView>(ViewIDs.WinMenuPopupView, PopupLayer);

            WinMenuPopupPresenter popup = _gameplayPresentersFactory.CreateWinMenuPopupPresenter(view);

            OnPopupCreated(popup, view);

            return popup;
        }
    }
}