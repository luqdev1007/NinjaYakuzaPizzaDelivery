using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.ObjectsManagment
{
    public class GameObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Queue<GameObject> _pool = new();

        public GameObjectPool(GameObject prefab, Transform parent, int initialSize = 5)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
                _pool.Enqueue(CreateNew());
        }

        public GameObject Get()
        {
            GameObject obj = _pool.Count > 0
                ? _pool.Dequeue()
                : CreateNew();

            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }

        private GameObject CreateNew()
        {
            GameObject obj = UnityEngine.Object.Instantiate(_prefab, _parent);
            obj.SetActive(false);
            return obj;
        }
    }
}