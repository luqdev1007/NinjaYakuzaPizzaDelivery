using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.Static
{
    public static class SpawnerProvider
    {
        private static List<GameObject> _pool = new();
        private const string SpawnerTag = "Spawner";

        public static GameObject GetRandomSpawner()
        {
            if (_pool.Count == 0)
            {
                RefreshPool();
            }

            int randomIndex = Random.Range(0, _pool.Count);
            GameObject selected = _pool[randomIndex];

            _pool.RemoveAt(randomIndex);

            if (selected == null)
                return GetRandomSpawner();

            return selected;
        }

        private static void RefreshPool()
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag(SpawnerTag);

            if (found.Length == 0)
            {
                Debug.LogError($"[SpawnerProvider] На сцене не найдены объекты с тегом {SpawnerTag}");
                return;
            }

            _pool.AddRange(found);
        }

        // Полезно вызвать при смене уровня, чтобы очистить ссылки на старые объекты
        public static void Clear()
        {
            _pool.Clear();
        }
    }
}