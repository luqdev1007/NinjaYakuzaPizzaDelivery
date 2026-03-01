using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using UnityEngine;
using System.Collections;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CustomPhysics
{
    public class AddDelayedForceSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly bool _isAdditive;
        private WaitForSeconds _delay;
        private Rigidbody _rigidbody;
        private ReactiveVariable<Vector3> _pushDirection;
        private ReactiveVariable<float> _pushForce;

        private Coroutine _coroutine;

        public AddDelayedForceSystem(float delay, ICoroutinesPerformer coroutinesPerformer, bool isAdditive = false)
        {
            Delay = delay;
            _coroutinesPerformer = coroutinesPerformer;
            _isAdditive = isAdditive;
            _delay = new WaitForSeconds(delay);
        }

        public float Delay { get; private set;  }

        public void OnInit(Entity entity)
        {
            // _rigidbody = entity.Rigidbody;

            _pushForce = entity.PushForce;
            _pushDirection = entity.PushDirection;

            _coroutine = _coroutinesPerformer.StartPerform(DelayLaunchProcess());
        }

        private void Launch()
        {
            if (_isAdditive == false)
                _rigidbody.linearVelocity = Vector3.zero;

            _rigidbody.transform.SetParent(null);

            _rigidbody.linearVelocity = _pushDirection.Value * _pushForce.Value;
        }

        private IEnumerator DelayLaunchProcess()
        {
            yield return _delay;

            Launch();
        }

        public void OnDispose()
        {
            _coroutinesPerformer.StopPerform(_coroutine);   
        }
    }
}