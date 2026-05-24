using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private CinemachineCamera _activeCamera;

        private readonly CinemachineCamera _introCamera;
        private readonly CinemachineCamera _scoutingCamera;
        private readonly CinemachineCamera _heroCamera;

        private Tween _shakeTween;
        private CinemachineBrain _mainCameraBrain;

        private Vector3 _originalCameraPosition;
        private bool _isShaking;

        public CameraService(
            CinemachineCamera introCamera,
            CinemachineCamera scoutingCamera,
            CinemachineCamera heroCamera)
        {
            _introCamera = introCamera;
            _scoutingCamera = scoutingCamera;
            _heroCamera = heroCamera;

            if (Camera.main != null)
            {
                _mainCameraBrain = Camera.main.GetComponent<CinemachineBrain>();
            }
        }

        public CinemachineCamera ScoutingCamera => _scoutingCamera;

        public void GenerateHitShake(float forceMultiplier)
        {
            if (_activeCamera != _heroCamera || Camera.main == null)
                return;

            if (_isShaking)
            {
                _shakeTween?.Kill();
                Camera.main.transform.position = _originalCameraPosition;
            }
            else
            {
                _originalCameraPosition = Camera.main.transform.position;
                if (_mainCameraBrain != null)
                {
                    _mainCameraBrain.enabled = false;
                }
            }

            _isShaking = true;

            float strength = 0.24f * forceMultiplier;

            _shakeTween = Camera.main.transform.DOShakePosition(
                    duration: 0.15f,
                    strength: new Vector3(strength, strength, 0),
                    vibrato: 10,
                    randomness: 0f,
                    snapping: false,
                    fadeOut: true)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _isShaking = false;

                    if (_mainCameraBrain != null)
                    {
                        _mainCameraBrain.enabled = true;
                    }
                });
        }

        public void SetState(CameraState state)
        {
            if (_activeCamera != null)
            {
                _activeCamera.Priority = 0;
            }

            switch (state)
            {
                case CameraState.Intro:
                    _activeCamera = _introCamera;
                    break;
                case CameraState.Scouting:
                    _activeCamera = _scoutingCamera;
                    break;
                case CameraState.HeroFollow:
                    _activeCamera = _heroCamera;
                    break;
            }

            if (_activeCamera != null)
            {
                _activeCamera.Priority = 10;
            }
        }

        public void AttachHero(Transform heroTransform)
        {
            if (_heroCamera != null)
            {
                _heroCamera.Follow = heroTransform;
                _heroCamera.LookAt = heroTransform;
            }
        }
    }
}