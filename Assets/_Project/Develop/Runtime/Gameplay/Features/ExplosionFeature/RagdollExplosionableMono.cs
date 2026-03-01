using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ExplosionFeature
{
    public class RagdollExplosionableMono : SimpleExplosionableMono
    {
        [Header("Ragdoll Settings")]
        [SerializeField] private Rigidbody[] _ragdollRigidbodies;
        [SerializeField] private Animator _animator;

        [Header("Explosion Tuning")]
        [Range(0f, 1f)][SerializeField] private float _randomnessFactor = 0.3f;
        [SerializeField] private float _explosionRadius = 5f;

        private void Awake()
        {
            if (_animator == null) 
                _animator = GetComponentInChildren<Animator>();

            if (_ragdollRigidbodies == null || _ragdollRigidbodies.Length == 0)
                _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

            ToggleRagdoll(false);
        }

        public override void Explode(float power, Vector3 sourcePosition, IExplosion effect)
        {
            // Находим самый верхний объект (Root), чтобы не оставлять "пустышек" на корабле
            Transform rootTransform = transform.root;

            // 1. ПОЛНОСТЬЮ отвязываем всё дерево объектов от корабля
            if (rootTransform.parent != null) 
                rootTransform.SetParent(null);

            // 2. Активируем регдолл (кости оживают)
            ToggleRagdoll(true);

            // 3. Работаем с главным Rigidbody персонажа
            if (AttachedRigidbody != null)
            {
                // ВАЖНО: Вместо isKinematic = true, мы просто отключаем главный коллайдер.
                // Если оставить isKinematic = false, корень будет падать по гравитации вместе с регдоллом.
                if (TryGetComponent(out Collider mainCollider)) mainCollider.enabled = false;

                // Чтобы корень не "улетал" отдельно от регдолла, делаем его легким 
                // или действительно кинематичным, НО тогда он будет висеть. 
                // Лучший вариант для воды — оставить его падать, но без коллизий.
                AttachedRigidbody.useGravity = true;
                AttachedRigidbody.isKinematic = false;
                AttachedRigidbody.linearVelocity = AttachedRigidbody.linearVelocity; // Сохраняем инерцию корабля
            }

            // 4. Разбрасываем кости
            foreach (var rb in _ragdollRigidbodies)
            {
                float randomModifier = 1f + Random.Range(-_randomnessFactor, _randomnessFactor);

                rb.AddExplosionForce(
                    power * randomModifier,
                    sourcePosition,
                    _explosionRadius,
                    UpwardForceMultiplier,
                    ForceMode.Impulse
                );

                rb.AddTorque(Random.insideUnitSphere * power * 0.1f, ForceMode.Impulse);
            }
        }

        private void ToggleRagdoll(bool isActive)
        {
            if (_animator != null) 
                _animator.enabled = !isActive;

            foreach (var rb in _ragdollRigidbodies)
            {
                rb.isKinematic = !isActive;
                rb.useGravity = isActive;
                if (isActive) rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }
}