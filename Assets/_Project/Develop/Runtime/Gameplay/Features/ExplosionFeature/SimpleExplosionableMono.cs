using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class SimpleExplosionableMono : ExplosionableMono
    {
        [Header("Explosion Tuning")]
        [SerializeField] protected float UpwardForceMultiplier = 0.8f;
        [SerializeField] protected float OutwardForceMultiplier = 1.2f;

        public override void Explode(float power, Vector3 sourcePosition, IExplosion source)
        {
            if (transform.parent != null) transform.SetParent(null);

            AttachedRigidbody.isKinematic = false;
            AttachedRigidbody.useGravity = true;

            // РАСЧЕТ ВЕКТОРА
            Vector3 dir = transform.position - sourcePosition;
            Vector3 lateralDir = new Vector3(dir.x, 0, dir.z).normalized;
            if (dir.magnitude < 0.1f) lateralDir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;

            Vector3 explosionVector = (lateralDir * OutwardForceMultiplier) + (Vector3.up * UpwardForceMultiplier);

            // ГЛАВНЫЙ СЕКРЕТ: Умножаем силу на массу (или используем ForceMode.VelocityChange)
            // Это гарантирует, что объект массой 50 и массой 1 полетят с одинаковой скоростью
            float massFactor = AttachedRigidbody.mass;
            // В SimpleExplosionableMono измени строку AddForce:
            AttachedRigidbody.AddForce(explosionVector.normalized * power, ForceMode.VelocityChange);

            // Закручиваем (тоже с учетом массы)
            AttachedRigidbody.AddTorque(Random.insideUnitSphere * power * massFactor, ForceMode.Impulse);

            Debug.Log($"{gameObject.name} (Mass: {massFactor}) подлетел!");
        }
    }
}