using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.ObjectsManagment
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PoolableVfx : MonoBehaviour
    {
        private IVfxPoolService _poolService;
        private ParticleSystem _prefab;

        public void Setup(IVfxPoolService service, ParticleSystem prefab)
        {
            _poolService = service;
            _prefab = prefab;

            // Важно: в настройках ParticleSystem (модуль Main) 
            // должен стоять Stop Action: Callback
            var main = GetComponent<ParticleSystem>().main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            _poolService.ReturnToPool(_prefab, GetComponent<ParticleSystem>());
        }
    }
}