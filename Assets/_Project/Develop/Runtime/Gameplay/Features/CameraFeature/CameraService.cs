using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private readonly Camera _camera;
        private ICameraBehaviour _currentBehaviour;
        private Bounds? _levelBounds;

        private Vector3 _currentVelocity;

        // Поля для эффектов
        private float _shakeTimer;
        private float _shakeAmount;
        private float _zoomImpulse; // Импульсный зум при ударе

        // --- НАСТРОЙКИ ЗУМА ---
        private const float MinSize = 7f;
        private const float MaxSize = 12.5f;
        private const float ZoomSmoothness = 3f;

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

            // 1. Движение камеры к цели
            Vector3 targetPos = _currentBehaviour.Update(previousPos, deltaTime);

            // 2. Ограничение позиции границами уровня
            if (_levelBounds.HasValue)
                targetPos = ClampPosition(targetPos);

            // 3. Наложение эффекта тряски
            targetPos = ApplyShake(targetPos, deltaTime);

            // 4. Применение позиции
            _camera.transform.position = targetPos;

            // 5. Расчет чистой скорости для эффектов
            _currentVelocity = (targetPos - previousPos) / deltaTime;

            // 6. Эффект динамического зума
            HandleDynamicZoom(deltaTime);
        }

        public void Shake(float intensity)
        {
            _shakeAmount = intensity * 0.7f;
            _shakeTimer = 0.2f;
        }

        public void ZoomImpulse(float intensity)
        {
            // Уменьшаем orthographicSize (приближаем), интенсивность 0.5-1.0 обычно достаточно
            _zoomImpulse = intensity * 1.5f;
        }

        private Vector3 ApplyShake(Vector3 pos, float deltaTime)
        {
            if (_shakeTimer > 0)
            {
                Vector2 randomOffset = Random.insideUnitCircle * _shakeAmount;
                _shakeTimer -= deltaTime;
                _shakeAmount = Mathf.Lerp(_shakeAmount, 0, deltaTime * 5f);

                return new Vector3(pos.x + randomOffset.x, pos.y + randomOffset.y, pos.z);
            }

            return pos;
        }

        private void HandleDynamicZoom(float deltaTime)
        {
            float speedX = Mathf.Abs(_currentVelocity.x);
            float speedY = Mathf.Abs(_currentVelocity.y);

            float verticalEmphasis = 1.2f;
            float effectiveSpeed = Mathf.Max(speedX, speedY * verticalEmphasis);

            float targetSize;

            if (effectiveSpeed < MinVelocityThreshold)
            {
                targetSize = MinSize;
            }
            else
            {
                float normalizedSpeed = Mathf.Clamp01((effectiveSpeed - MinVelocityThreshold) / (MaxVelocityForZoom - MinVelocityThreshold));
                targetSize = Mathf.Lerp(MinSize, MaxSize, normalizedSpeed);
            }

            // Применяем импульсный зум (вычитаем из целевого размера)
            targetSize -= _zoomImpulse;

            // Плавно возвращаем импульс к нулю (быстрее, чем основной зум)
            _zoomImpulse = Mathf.Lerp(_zoomImpulse, 0, deltaTime * 10f);

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