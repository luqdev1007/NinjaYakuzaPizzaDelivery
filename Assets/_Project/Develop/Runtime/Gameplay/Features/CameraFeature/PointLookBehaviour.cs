using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class PointLookBehaviour : ICameraBehaviour
    {
        private readonly Vector3 _targetPoint;
        private readonly float _smoothTime;
        private Vector3 _currentVelocity;

        public PointLookBehaviour(Vector3 targetPoint, float smoothTime = 0.5f)
        {
            // Сохраняем z = -10, чтобы камера не улетела внутрь спрайтов
            _targetPoint = new Vector3(targetPoint.x, targetPoint.y, -10f);
            _smoothTime = smoothTime;
        }

        public Vector3 Update(Vector3 currentPosition, float deltaTime)
        {
            return Vector3.SmoothDamp(currentPosition, _targetPoint, ref _currentVelocity, _smoothTime);
        }
    }
}