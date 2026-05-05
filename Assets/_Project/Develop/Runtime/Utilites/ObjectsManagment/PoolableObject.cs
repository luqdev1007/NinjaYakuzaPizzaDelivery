using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.ObjectsManagment
{
    public class PoolableObject : MonoBehaviour
    {
        private GameObjectPool _originPool;

        public void Init(GameObjectPool pool) => _originPool = pool;

        public void ReturnToPool()
        {
            if (_originPool != null)
                _originPool.Return(gameObject);
            else
                Destroy(gameObject);
        }
    }
}