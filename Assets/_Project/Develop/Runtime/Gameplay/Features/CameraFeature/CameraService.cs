using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private readonly Camera _camera;
        private ICameraBehaviour _currentBehaviour;
        private Bounds? _levelBounds;

        private Vector3 _currentVelocity;

        // --- НАСТРОЙКИ ЗУМА ---
        private const float MinSize = 7f;
        private const float MaxSize = 12.5f;
        private const float ZoomSmoothness = 1.5f;

        // --- ПОРОГИ (DEADZONES) ---
        private const float MinVelocityThreshold = 3f;
        private const float MaxVelocityForZoom = 22f;

        public ICameraBehaviour CurrentBehaviour => _currentBehaviour;

        public CameraService(Camera camera)
        {
            _camera = camera;
        }

        public void SetBehaviour(ICameraBehaviour behaviour) => _currentBehaviour = behaviour;
        public void SetConstraints(Bounds bounds) => _levelBounds = bounds;

        public void Update(float deltaTime)
        {
            if (_currentBehaviour == null || deltaTime <= 0) return;

            Vector3 previousPos = _camera.transform.position;

            // 1. Движение камеры
            Vector3 targetPos = _currentBehaviour.Update(previousPos, deltaTime);

            if (_levelBounds.HasValue)
                targetPos = ClampPosition(targetPos);

            _camera.transform.position = targetPos;

            // 2. Расчет чистой скорости
            _currentVelocity = (targetPos - previousPos) / deltaTime;

            // 3. Эффекты (только динамический зум)
            HandleDynamicZoom(deltaTime);
        }

        private void HandleDynamicZoom(float deltaTime)
        {
            float speed = _currentVelocity.magnitude;
            float targetSize;

            if (speed < MinVelocityThreshold)
            {
                targetSize = MinSize;
            }
            else
            {
                float normalizedSpeed = Mathf.Clamp01((speed - MinVelocityThreshold) / (MaxVelocityForZoom - MinVelocityThreshold));
                targetSize = Mathf.Lerp(MinSize, MaxSize, normalizedSpeed);
            }

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetSize, deltaTime * ZoomSmoothness);
        }

        private Vector3 ClampPosition(Vector3 pos)
        {
            float camHeight = _camera.orthographicSize;
            float camWidth = camHeight * _camera.aspect;
            Bounds b = _levelBounds.Value;

            float minX = b.min.x + camWidth;
            float maxX = b.max.x - camWidth;
            float minY = b.min.y + camHeight;
            float maxY = b.max.y - camHeight;

            if (minX > maxX) pos.x = b.center.x;
            else pos.x = Mathf.Clamp(pos.x, minX, maxX);

            if (minY > maxY) pos.y = b.center.y;
            else pos.y = Mathf.Clamp(pos.y, minY, maxY);

            return pos;
        }
    }
}