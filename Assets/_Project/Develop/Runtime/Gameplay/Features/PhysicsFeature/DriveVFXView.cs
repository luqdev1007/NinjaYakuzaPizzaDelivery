using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DriveVFXView : EntityView
    {
        [Header("Drive Effects")]
        [SerializeField] private ParticleSystem _driveFireStreamPS;
        [SerializeField] private ParticleSystem _driveSparksPS;

        [Header("Shake (Optional)")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _shakeIntensity = 0.1f;

        private IReadOnlyVariable<bool> _isDriveActive;
        private IDisposable _driveDisposable;
        private bool _active;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDriveActive = entity.IsDriveActive;
            _driveDisposable = _isDriveActive.Subscribe(OnDriveStatusChanged);
        }

        private void Update()
        {
            if (!_active || _viewContainer == null) return;

            // Используем insideUnitCircle для 2D тряски
            Vector2 shakeOffset = UnityEngine.Random.insideUnitCircle * _shakeIntensity;
            _viewContainer.localPosition = new Vector3(shakeOffset.x, shakeOffset.y, 0f);
        }

        private void OnDriveStatusChanged(bool oldValue, bool value)
        {
            _active = value;

            if (value)
            {
                if (_driveFireStreamPS != null) _driveFireStreamPS.Play();
                if (_driveSparksPS != null) _driveSparksPS.Play();
            }
            else
            {
                if (_driveFireStreamPS != null) _driveFireStreamPS.Stop();
                if (_driveSparksPS != null) _driveSparksPS.Stop();

                if (_viewContainer != null)
                    _viewContainer.localPosition = Vector3.zero;
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _driveDisposable?.Dispose();

            if (_viewContainer != null)
                _viewContainer.localPosition = Vector3.zero;
        }
    }
}

