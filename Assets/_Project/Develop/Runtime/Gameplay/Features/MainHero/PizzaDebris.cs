using Assets._Project.Develop.Runtime.Utilites.ObjectsManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class PizzaDebris : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 3f;
        private PoolableObject _poolable;

        private void Awake() => _poolable = GetComponent<PoolableObject>();

        private void OnEnable()
        {
            CancelInvoke();
            Invoke(nameof(ReturnToPool), _lifeTime);
        }

        private void ReturnToPool() => _poolable.ReturnToPool();
    }
}

