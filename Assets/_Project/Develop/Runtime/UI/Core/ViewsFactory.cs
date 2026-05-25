using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.UI.Core
{
    public class ViewsFactory
    {
        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;

        private readonly Dictionary<string, string> _viewIDToResourcesPath = new Dictionary<string, string>()
        {
            // Common
            {ViewIDs.ConfirmPopupView, "UI/Common/ConfirmPopupView" },

            // Meta
            {ViewIDs.CurrencyView, "UI/Meta/Wallet/CurrencyView" },

            // Gameplay
            {ViewIDs.DefeatPopupView, "UI/Gameplay/ResultPopups/DefeatPopupView" },
            {ViewIDs.WinPopupView, "UI/Gameplay/ResultPopups/WinPopupView" },
            {ViewIDs.DialogDisplayView, "UI/Gameplay/Dialog/DialogDisplayView" },
            {ViewIDs.GameplayScreenView, "UI/Gameplay/GameplayScreenView" },
            {ViewIDs.HintView, "UI/Gameplay/Hint/HintView" },

            // Main Menu
            {ViewIDs.MainMenuScreenView, "UI/MainMenu/MainMenuScreenView" },
            {ViewIDs.LevelsMenuPopup, "UI/MainMenu/LevelsMenuPopup" },
            {ViewIDs.LevelTile, "UI/MainMenu/LevelTile" },
            {ViewIDs.LoadMusicPopupView, "UI/MainMenu/LoadMusicPopupView" },

            // Game Settings
            { ViewIDs.AudioSettingsPopupView, "UI/GameSettings/AudioSettingsPopupView" },
            { ViewIDs.GameSettingsPopupView, "UI/GameSettings/GameSettingsPopupView" },
            { ViewIDs.GraphicSettingsPopupView, "UI/GameSettings/GraphicSettingsPopupView" },
            { ViewIDs.LanguageSettingsPopupView, "UI/GameSettings/LanguageSettingsPopupView" },
            { ViewIDs.KeyBindingsSettingsPopupView, "UI/GameSettings/KeyBindingsSettingsPopupView" },
            
            // not used
            {ViewIDs.LootFeedbackView, "UI/Common/LootFeedbackView" },
            {ViewIDs.SimpleHealthBar, "UI/Gameplay/HealthBars/SimpleHealthBar" },
            {ViewIDs.MainHeroHealthBar, "UI/Gameplay/HealthBars/MainHeroHealthBar" },
        };

        public ViewsFactory(ResourcesAssetsLoader resourcesAssetsLoader)
        {
            _resourcesAssetsLoader = resourcesAssetsLoader;
        }

        public TView Create<TView>(string viewID, Transform parent = null) where TView : MonoBehaviour, IView 
        {
            if (_viewIDToResourcesPath.TryGetValue(viewID, out string resourcePath) == false)
                throw new ArgumentException($"You didn't set resource path for {typeof(TView)}, searched id: {viewID}");

            GameObject prefab = _resourcesAssetsLoader.Load<GameObject>(resourcePath);
            GameObject instance = Object.Instantiate(prefab, parent);
            TView view = instance.GetComponent<TView>();

            if (view == null)
                throw new InvalidOperationException($"Not found {typeof(TView)} component of view instance");

            return view;
        }

        public void Release<TView>(TView view) where TView : MonoBehaviour, IView
        {
            Object.Destroy(view.gameObject);
        }

        internal T Create<T>(object hintView, Transform popupLayer)
        {
            throw new NotImplementedException();
        }
    }
}
