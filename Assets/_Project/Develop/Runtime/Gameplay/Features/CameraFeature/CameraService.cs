using Unity.Cinemachine;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private CinemachineCamera _activeCamera;

        private readonly CinemachineCamera _introCamera;
        private readonly CinemachineCamera _scoutingCamera;
        private readonly CinemachineCamera _heroCamera;

        public CameraService(
            CinemachineCamera introCamera,
            CinemachineCamera scoutingCamera,
            CinemachineCamera heroCamera)
        {
            _introCamera = introCamera;
            _scoutingCamera = scoutingCamera;
            _heroCamera = heroCamera;
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
                    {
                        _activeCamera = _introCamera;
                        break;
                    }
                case CameraState.Scouting:
                    {
                        _activeCamera = _scoutingCamera;
                        break;
                    }
                case CameraState.HeroFollow:
                    {
                        _activeCamera = _heroCamera;
                        break;
                    }
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