using Assets._Project.Develop.Runtime.Configs.Audio;
using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.ConfigsManagment
{
    public class ResourcesConfigsLoader : IConfigLoader
    {
        private readonly ResourcesAssetsLoader _resources;

        private readonly Dictionary<Type, string> _configsResourcesPath = new()
        {
            { typeof(StartWalletConfig), "Configs/Meta/Wallet/StartWalletConfig" },
            { typeof(CurrencyIconsConfig), "Configs/Meta/Wallet/CurrencyIconsConfig" },
            { typeof(LevelsListConfig), "Configs/Gameplay/Levels/LevelsListConfig" },
            { typeof(MainHeroConfig), "Configs/Entities/MainHero/MainHeroConfig" },
            { typeof(GhostConfig), "Configs/Entities/Enemies/GhostConfig" }, // enemies config
            { typeof(SlimeConfig), "Configs/Entities/Enemies/SlimeConfig" }, // enemies config
            { typeof(DialogConfig), "Configs/Dialogs/TutorialDialog" },
            { typeof(CharactersConfig), "Configs/Dialogs/CharactersConfig" },
            { typeof(MasterLootProviderConfig), "Configs/Gameplay/Loot/MasterLootProvider" },
            { typeof(StyleRankConfig), "Configs/Gameplay/Style/StyleRankConfig" },
            { typeof(StyleActionsConfig), "Configs/Gameplay/Style/StyleActionsConfig" },

            { typeof(AudioLibrary), "Configs/Audio/AudioLibrary" },

            { typeof(PlayerInventoryConfig), "Configs/PlayerData/PlayerInventoryConfig" },
        };

        public ResourcesConfigsLoader(ResourcesAssetsLoader resources)
        {
            _resources = resources;
        }

        public IEnumerator LoadAsync(Action<Dictionary<Type, object>> onConfigsLoaded)
        {
            Dictionary<Type, object> loadedConfigs = new();

            foreach (KeyValuePair<Type, string> configsResourcesPath in _configsResourcesPath)
            {
                ScriptableObject config = _resources.Load<ScriptableObject>(configsResourcesPath.Value);
                loadedConfigs.Add(configsResourcesPath.Key, config);

                yield return null;
            }

            onConfigsLoaded?.Invoke(loadedConfigs);
        }
    }
}