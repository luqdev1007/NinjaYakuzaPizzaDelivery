using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Core.ConfirmPopup;
using Assets._Project.Develop.Runtime.UI.Core.GameSettings;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuPopupService : PopupService
    {
        private readonly MainMenuUIRoot _uiRoot;
        private readonly MainMenuPresentersFactory _mainMenuPresentersFactory;

        public MainMenuPopupService(
            ViewsFactory viewsFactory,
            ProjectPresentersFactory presentersFactory,
            MainMenuUIRoot uiRoot,
            MainMenuPresentersFactory mainMenuPresentersFactory)
            : base(viewsFactory, presentersFactory)
        {
            _uiRoot = uiRoot;
            _mainMenuPresentersFactory = mainMenuPresentersFactory;
        }

        protected override Transform PopupLayer => _uiRoot.PopupsLayer;
        protected override Transform VFXOverPopupsLayer => _uiRoot.VFXOverPopupsLayer;

        public LoadMusicPopupPresenter OpenLoadMusicPopupView()
        {
            LoadMusicPopupView view = ViewsFactory.Create<LoadMusicPopupView>(ViewIDs.LoadMusicPopupView, PopupLayer);

            LoadMusicPopupPresenter popup = _mainMenuPresentersFactory.CreateLoadMusicPopupPresenter(view);

            OnPopupCreated(popup, view);

            return popup;
        }
    }
}
