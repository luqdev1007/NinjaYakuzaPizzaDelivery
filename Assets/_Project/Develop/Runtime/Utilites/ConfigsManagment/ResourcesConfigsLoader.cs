using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Configs.Meta.Stats;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement; // Добавил пространство имен
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.ConfigsManagment
{
    public class ResourcesConfigsLoader : IConfigLoader
    {
        private readonly ResourcesAssetsLoader _resources;

        private readonly Dictionary<Type, string> _configsResourcesPath = new()
        {
            { typeof(StartWalletConfig), "Configs/Meta/Wallet/StartWalletConfig" },
            { typeof(CurrencyIconsConfig), "Configs/Meta/Wallet/CurrencyIconsConfig" },
            { typeof(GameRewardsConfig), "Configs/Meta/Stats/GameRewardsConfig" },
            { typeof(LevelsListConfig), "Configs/Gameplay/Levels/LevelsListConfig" },
            { typeof(MainHeroConfig), "Configs/Entities/MainHero/MainHeroConfig" },
            { typeof(GhostConfig), "Configs/Entities/GhostConfig" },
            { typeof(DialogConfig), "Configs/Dialogs/TutorialDialog" },
            { typeof(CharactersConfig), "Configs/Dialogs/CharactersConfig" },
            { typeof(AudioConfig), "Configs/Audio/AudioConfig" },
            { typeof(LootTableConfig), "Configs/Gameplay/Loot/LootTable" }
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