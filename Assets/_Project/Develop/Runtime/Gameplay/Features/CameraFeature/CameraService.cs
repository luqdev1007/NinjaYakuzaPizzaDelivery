using UnityEngine;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class CameraService
    {
        private readonly Camera _camera;
        private ICameraBehaviour _currentBehaviour;
        private Bounds? _levelBounds;

        private Vector3 _currentVelocity;
        private List<ParticleSystem> _leafParticles = new List<ParticleSystem>();

        // --- НАСТРОЙКИ ЗУМА ---
        private const float MinSize = 7f;        // Чуть поближе в покое (было 10)
        private const float MaxSize = 12.5f;     // Отдаляется поменьше (было 13)
        private const float ZoomSmoothness = 1.5f; // Плавность (чем меньше, тем медленнее)

        // --- ПОРОГИ (DEADZONES) ---
        private const float MinVelocityThreshold = 3f;  // Скорость ниже этой игнорируется
        private const float MaxVelocityForZoom = 22f;    // Порог "максимального" отдаления

        public ICameraBehaviour CurrentBehaviour => _currentBehaviour;

        public CameraService(Camera camera)
        {
            _camera = camera;
            _leafParticles.AddRange(_camera.GetComponentsInChildren<ParticleSystem>());
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

            // 3. Эффекты
            ApplyWindToParticles();
            HandleDynamicZoom(deltaTime);
        }

        private void HandleDynamicZoom(float deltaTime)
        {
            float speed = _currentVelocity.magnitude;
            float targetSize;

            // Если скорость ниже порога — зум всегда минимальный (базовый)
            if (speed < MinVelocityThreshold)
            {
                targetSize = MinSize;
            }
            else
            {
                // Рассчитываем фактор скорости только сверх порога
                float normalizedSpeed = Mathf.Clamp01((speed - MinVelocityThreshold) / (MaxVelocityForZoom - MinVelocityThreshold));
                targetSize = Mathf.Lerp(MinSize, MaxSize, normalizedSpeed);
            }

            // Используем Lerp для максимальной мягкости, чтобы камеру не "штормило"
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetSize, deltaTime * ZoomSmoothness);
        }

        private void ApplyWindToParticles()
        {
            float mag = _currentVelocity.magnitude;

            // Листья тоже не должны суетиться от микро-движений
            if (mag < MinVelocityThreshold)
            {
                foreach (var ps in _leafParticles)
                {
                    var force = ps.forceOverLifetime;
                    force.x = 0;
                    force.y = 0;
                }
                return;
            }

            foreach (var ps in _leafParticles)
            {
                var forceModule = ps.forceOverLifetime;
                forceModule.enabled = true;

                // Инерция
                Vector3 resistance = -_currentVelocity * 0.12f;
                forceModule.x = new ParticleSystem.MinMaxCurve(resistance.x);
                forceModule.y = new ParticleSystem.MinMaxCurve(resistance.y);

                // Турбулентность при рывках
                var noise = ps.noise;
                if (noise.enabled)
                {
                    float noiseFactor = Mathf.Clamp01(mag / MaxVelocityForZoom);
                    noise.strength = Mathf.Lerp(0f, 0.6f, noiseFactor);
                }
            }
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

            // Если зум слишком большой и камера не влезает в границы уровня по X или Y
            // (защита от тряски, когда maxX становится меньше minX)
            if (minX > maxX) pos.x = b.center.x;
            else pos.x = Mathf.Clamp(pos.x, minX, maxX);

            if (minY > maxY) pos.y = b.center.y;
            else pos.y = Mathf.Clamp(pos.y, minY, maxY);

            return pos;
        }
    }
}