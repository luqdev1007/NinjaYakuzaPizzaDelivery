using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private readonly Camera _camera;
        private ICameraBehaviour _currentBehaviour;
        private Bounds? _levelBounds;

        public ICameraBehaviour CurrentBehaviour => _currentBehaviour;

        public CameraService(Camera camera)
        {
            _camera = camera;
        }

        public void SetBehaviour(ICameraBehaviour behaviour)
        {
            _currentBehaviour = behaviour;
        }

        public void SetConstraints(Bounds bounds)
        {
            _levelBounds = bounds;
        }

        public void Update(float deltaTime)
        {
            if (_currentBehaviour == null) return;

            // 1. Получаем желаемую позицию от поведения
            Vector3 targetPos = _currentBehaviour.Update(_camera.transform.position, deltaTime);

            // 2. Если есть границы — зажимаем позицию внутри них
            if (_levelBounds.HasValue)
            {
                targetPos = ClampPosition(targetPos);
            }

            // 3. Применяем
            _camera.transform.position = targetPos;
        }

        private Vector3 ClampPosition(Vector3 pos)
        {
            float camHeight = _camera.orthographicSize;
            float camWidth = camHeight * _camera.aspect;
            Bounds b = _levelBounds.Value;

            // Учитываем размер вьюпорта камеры, чтобы она не видела края текстур
            float minX = b.min.x + camWidth;
            float maxX = b.max.x - camWidth;
            float minY = b.min.y + camHeight;
            float maxY = b.max.y - camHeight;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            return pos;
        }
    }
}