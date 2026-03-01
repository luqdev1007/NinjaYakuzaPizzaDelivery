using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Mines;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Explosions;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
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

            { typeof(LevelsListConfig), "Configs/Gameplay/Levels/LevelsListConfig" },

            { typeof(ExplosionsListConfig), "Configs/Gameplay/Explosions/ExplosionsListConfig" },

            { typeof(ArcherConfig), "Configs/Gameplay/Entities/Enemies/ArcherConfig" },
            { typeof(SoldierConfig), "Configs/Gameplay/Entities/Enemies/SoldierConfig" },
            { typeof(DriverConfig), "Configs/Gameplay/Entities/Enemies/DriverConfig" },

            { typeof(CaptainConfig), "Configs/Gameplay/Entities/MainHeroes/CaptainConfig" },
            { typeof(WizardConfig), "Configs/Gameplay/Entities/MainHeroes/WizardConfig" },
            { typeof(EngineerConfig), "Configs/Gameplay/Entities/MainHeroes/EngineerConfig" },

            { typeof(SimpleProjectileConfig), "Configs/Gameplay/Entities/Projectiles/SimpleProjectileConfig" },

            { typeof(MainShipConfig), "Configs/Gameplay/Entities/Vehicles/MainShipConfig" },
            { typeof(SmallShipConfig), "Configs/Gameplay/Entities/Vehicles/SmallShipConfig" },

            { typeof(BallistaConfig), "Configs/Gameplay/Entities/Ballista/BallistaConfig" },

            { typeof(WaterMineConfig), "Configs/Gameplay/Entities/Mines/WaterMineConfig" },
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
