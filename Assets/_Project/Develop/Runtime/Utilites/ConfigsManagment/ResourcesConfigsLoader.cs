using Assets._Project.Develop.Runtime.Configs;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.ConfigsManagment
{
    public class ResourcesConfigsLoader : IConfigLoader
    {
        private const string MainConfigPath = "Configs/AllConfigs";

        private readonly ResourcesAssetsLoader _resources;

        public ResourcesConfigsLoader(ResourcesAssetsLoader resources)
        {
            _resources = resources;
        }

        public IEnumerator LoadAsync(Action<Dictionary<Type, object>> onConfigsLoaded)
        {
            AllConfigs allConfigs = _resources.Load<AllConfigs>(MainConfigPath);

            if (allConfigs == null)
            {
                Debug.LogError($"[ConfigsLoader] Не удалось загрузить AllConfigs по пути: {MainConfigPath}");
                onConfigsLoaded?.Invoke(new Dictionary<Type, object>());
                yield break;
            }

            Dictionary<Type, object> loadedConfigs = new();

            PropertyInfo[] properties = typeof(AllConfigs).GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(allConfigs);

                if (value != null)
                {
                    loadedConfigs.Add(property.PropertyType, value);
                }
                else
                {
                    Debug.LogWarning($"[ConfigsLoader] Свойство {property.Name} в AllConfigs не заполнено (null)!");
                }
            }

            onConfigsLoaded?.Invoke(loadedConfigs);

            yield break;
        }
    }
}