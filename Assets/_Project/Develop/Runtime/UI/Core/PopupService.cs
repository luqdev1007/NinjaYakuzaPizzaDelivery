using Assets._Project.Develop.Runtime.UI.Core.TestPopup;
using Assets._Project.Develop.Runtime.UI.LevelsMenuPopup;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Core
{
    public abstract class PopupService : IDisposable
    {
        protected readonly ViewsFactory ViewsFactory;

        private readonly ProjectPresentersFactory _presentersFactory;

        private readonly Dictionary<PopupPresenterBase, PopupInfo> _presenterToInfo = new();

        protected PopupService(
            ViewsFactory viewsFactory, 
            ProjectPresentersFactory presentersFactory)
        {
            ViewsFactory = viewsFactory;
            _presentersFactory = presentersFactory;
        }

        protected abstract Transform PopupLayer { get; }

        public void ClosePopup(PopupPresenterBase popup)
        {
            popup.CloseRequest -= ClosePopup;

            popup.Hide(() =>
            {
                _presenterToInfo[popup].CloseCallback?.Invoke();

                DisposeFor(popup);
                _presenterToInfo.Remove(popup);
            });
        }


        public void Dispose()
        {
            foreach (PopupPresenterBase popup in _presenterToInfo.Keys)
            {
                popup.CloseRequest -= ClosePopup;
                DisposeFor(popup);
            }

            _presenterToInfo.Clear();
        }

        private void DisposeFor(PopupPresenterBase popup)
        {
            popup.Dispose();
            ViewsFactory.Release(_presenterToInfo[popup].View);
        }


        public LevelsMenuPopupPresenter OpenLevelsMenuPopup()
        {
            LevelsMenuPopupView view = ViewsFactory.Create<LevelsMenuPopupView>(ViewIDs.LevelsMenuPopup, PopupLayer);

            LevelsMenuPopupPresenter popup = _presentersFactory.CreateLevelsMenuPopupPresenter(view);

            OnPopupCreated(popup, view);

            return popup;
        }

        public ConfirmPopupPresenter OpenConfirmPopup(Action onConfirmButtonClicked, string header, Action closeCallback = null)
        {
            ConfirmPopupView view = ViewsFactory.Create<ConfirmPopupView>(ViewIDs.ConfirmPopup, PopupLayer);

            ConfirmPopupPresenter popup = _presentersFactory.CreateConfirmPopupPresenter(view, onConfirmButtonClicked, header);

            OnPopupCreated(popup, view, closeCallback);

            return popup;
        }

        protected void OnPopupCreated(
            PopupPresenterBase popup,
            PopupViewBase view,
            Action closeCallback = null)
        {
            PopupInfo popupInfo = new PopupInfo(view, closeCallback);

            _presenterToInfo.Add(popup, popupInfo);

            popup.Initialize();
            popup.Show();

            popup.CloseRequest += ClosePopup;
        }

        private class PopupInfo
        {
            public PopupInfo(PopupViewBase view, Action closeCallback)
            {
                View = view;
                CloseCallback = closeCallback;
            }

            public PopupViewBase View { get; }
            public Action CloseCallback { get; }
        }
    }
}
