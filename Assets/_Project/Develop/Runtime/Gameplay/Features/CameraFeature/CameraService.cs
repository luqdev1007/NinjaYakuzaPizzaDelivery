using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private readonly Camera _camera;
        private ICameraBehaviour _currentBehaviour;
        private ICameraBehaviour _backupBehaviour; // Для возврата к преследованию героя
        private Bounds? _levelBounds;

        private Vector3 _currentVelocity;
        private float? _zoomOverride; // Принудительный зум (например, для показа цели)

        // Поля для эффектов
        private float _shakeTimer;
        private float _shakeAmount;
        private float _zoomImpulse;

        // --- НАСТРОЙКИ ЗУМА ---
        private const float MinSize = 5f;
        private const float MaxSize = 20f;
        private const float ZoomSmoothness = 3f;
        private const float OverrideZoomSmoothness = 5f; // Чтобы к цели зумилось быстрее

        // --- ПОРОГИ (DEADZONES) ---
        private const float MinVelocityThreshold = 4f;
        private const float MaxVelocityForZoom = 22f;

        public ICameraBehaviour CurrentBehaviour => _currentBehaviour;

        public CameraService(Camera camera)
        {
            _camera = camera;
        }

        public void SetBehaviour(ICameraBehaviour behaviour) => _currentBehaviour = behaviour;
        public void SetConstraints(Bounds bounds) => _levelBounds = bounds;

        /// <summary>
        /// Временно переключает камеру на точку (например, финиш) с заданным зумом.
        /// </summary>
        public void ShowTargetTemporarily(Vector3 targetPos, float zoomSize = 12f)
        {
            if (_currentBehaviour is PointLookBehaviour) 
                return;

            _backupBehaviour = _currentBehaviour;
            _zoomOverride = zoomSize;
            SetBehaviour(new PointLookBehaviour(targetPos, 0.6f));
        }

        /// <summary>
        /// Возвращает камеру к предыдущему поведению (обычно к герою).
        /// </summary>
        public void StopShowingTarget()
        {
            if (_backupBehaviour != null)
            {
                SetBehaviour(_backupBehaviour);
                _backupBehaviour = null;
                _zoomOverride = null;
            }
        }

        public void Update(float deltaTime)
        {
            if (_currentBehaviour == null || deltaTime <= 0) 
                return;

            Vector3 previousPos = _camera.transform.position;

            // 1. Движение камеры к цели (через текущий Behaviour)
            Vector3 targetPos = _currentBehaviour.Update(previousPos, deltaTime);

            // 2. Ограничение позиции границами уровня (только если мы не в свободном полете интро, 
            // но обычно Clamp нужен всегда)
            if (_levelBounds.HasValue)
                targetPos = ClampPosition(targetPos);

            // 3. Наложение эффекта тряски
            targetPos = ApplyShake(targetPos, deltaTime);

            // 4. Применение позиции
            _camera.transform.position = targetPos;

            // 5. Расчет чистой скорости для эффектов зума
            _currentVelocity = (targetPos - previousPos) / deltaTime;

            // 6. Эффект динамического или принудительного зума
            HandleDynamicZoom(deltaTime);
        }

        public void Shake(float intensity)
        {
            _shakeAmount = intensity * 0.7f;
            _shakeTimer = 0.2f;
        }

        public void ZoomImpulse(float intensity)
        {
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

        // В начало класса добавь переменную
        private Rigidbody2D _heroRigidbody;

        // Добавь метод для установки цели (или передавай в конструктор)
        public void SetHeroRigidbody(Rigidbody2D rb) => _heroRigidbody = rb;

        private void HandleDynamicZoom(float deltaTime)
        {
            float targetSize;

            if (_zoomOverride.HasValue)
            {
                targetSize = _zoomOverride.Value;
            }
            else if (_heroRigidbody != null) // Считаем зум от героя
            {
                // Берем абсолютные значения скорости героя
                float speedX = Mathf.Abs(_heroRigidbody.linearVelocity.x);
                float speedY = Mathf.Abs(_heroRigidbody.linearVelocity.y);

                // Для Y при пикировании даем чуть больше веса, чтобы камера отдалялась сильнее
                float effectiveSpeed = Mathf.Max(speedX, speedY * 1.1f);

                if (effectiveSpeed < MinVelocityThreshold)
                {
                    targetSize = MinSize;
                }
                else
                {
                    // Используем MaxVelocityForZoom, чтобы ограничить отдаление
                    float normalizedSpeed = Mathf.Clamp01((effectiveSpeed - MinVelocityThreshold) / (MaxVelocityForZoom - MinVelocityThreshold));
                    targetSize = Mathf.Lerp(MinSize, MaxSize, normalizedSpeed);
                }
            }
            else
            {
                targetSize = MinSize;
            }

            // Твой старый код импульса и плавности...
            targetSize -= _zoomImpulse;
            _zoomImpulse = Mathf.Lerp(_zoomImpulse, 0, deltaTime * 10f);

            float smoothness = _zoomOverride.HasValue ? OverrideZoomSmoothness : ZoomSmoothness;
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetSize, deltaTime * smoothness);
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

            // Если камера шире уровня — центрируем по X
            if (minX > maxX) pos.x = b.center.x;
            else pos.x = Mathf.Clamp(pos.x, minX, maxX);

            // Если камера выше уровня — центрируем по Y
            if (minY > maxY) pos.y = b.center.y;
            else pos.y = Mathf.Clamp(pos.y, minY, maxY);

            return pos;
        }

        public void SetZoom(float size)
        {
            // Если камера ортографическая
            // _camera.orthographicSize = size;
            _camera.orthographicSize = MinSize;
            // Если перспективная, то меняем fieldOfView
            // _camera.fieldOfView = size; 
        }
    }
}