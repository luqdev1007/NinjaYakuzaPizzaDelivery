using Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Sensors
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class DetachColliderHandler : MonoBehaviour, IExplosionable
    {
        private Collider _attachedCollider;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _attachedCollider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }

        public void Explode(float power, Vector3 sourcePosition, IExplosion source)
        {
            if (_attachedCollider is MeshCollider meshCollider)
            {
                meshCollider.convex = true;
            }

            transform.SetParent(null);

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            _rigidbody.AddExplosionForce(power, sourcePosition, power);

            Vector3 randomTorque = Random.insideUnitSphere * (power * 0.5f);
            _rigidbody.AddTorque(randomTorque, ForceMode.Impulse);

            Destroy(gameObject, 10f);
        }
    }
}