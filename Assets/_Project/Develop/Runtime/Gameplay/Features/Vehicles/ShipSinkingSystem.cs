using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles
{
    public class ShipSinkingSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody _rigidbody;
        private Transform _transform;

        private ReactiveVariable<bool> _inDeathProcess;
        private ReactiveVariable<float> _rotationSinkSpeed;
        private ReactiveVariable<float> _moveSinkSpeed;

        private Vector3 _sinkRotationAxis;
        private bool _isInitializedSink;

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;
            _inDeathProcess = entity.InDeathProcess;
            _rotationSinkSpeed = entity.RotationSinkSpeed;
            _moveSinkSpeed = entity.MoveSinkSpeed;

            _sinkRotationAxis = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-0.5f, 0.5f)).normalized;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inDeathProcess.Value == false)
                return;

            if (_isInitializedSink == false)
                PrepareForSinking();

            _transform.position += Vector3.down * (_moveSinkSpeed.Value * deltaTime);

            float rotationStep = _rotationSinkSpeed.Value * deltaTime;
            _transform.Rotate(_sinkRotationAxis, rotationStep, Space.World);
        }

        private void PrepareForSinking()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.detectCollisions = false; // Чтобы обломки не мешали живым

            _isInitializedSink = true;
            Debug.Log("Корабль перешел в режим погружения (Kinematic).");
        }
    }
}