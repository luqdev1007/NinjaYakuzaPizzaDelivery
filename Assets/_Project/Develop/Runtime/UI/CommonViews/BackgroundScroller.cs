using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    // Скрипт требует наличия RawImage на этом же объекте
    [RequireComponent(typeof(RawImage))]
    public class BackgroundScroller : MonoBehaviour
    {
        [Header("Настройки скорости")]
        [Tooltip("Скорость скроллинга по горизонтали и вертикали")]
        [SerializeField] private Vector2 _scrollSpeed = new Vector2(0.05f, 0.03f);

        [Header("Опции")]
        [Tooltip("Если включено, будет двигаться и в редакторе (не во время игры)")]
        [SerializeField] private bool _animateInEditor = false;

        private RawImage _rawImage;
        private Rect _currentUvRect;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _currentUvRect = _rawImage.uvRect;
        }

        private void Update()
        {
            // Не анимируем в игре, если RawImage выключен
            if (_rawImage == null || !_rawImage.enabled) return;

            AnimateUV(Time.deltaTime);
        }

#if UNITY_EDITOR
        // Логика для анимации прямо в редакторе Unity (чтобы видеть эффект без запуска)
        private void OnDrawGizmos()
        {
            if (!_animateInEditor) return;
            if (Application.isPlaying) return; // В игре работает Update

            if (_rawImage == null)
                _rawImage = GetComponent<RawImage>();

            if (_rawImage != null)
            {
                // В редакторе используем Time.realtimeSinceStartup для дельты
                // Это грубое приближение, но для предпросмотра сойдет
                AnimateUV(0.001f); // Очень маленькая дельта
            }
        }
#endif

        private void AnimateUV(float deltaTime)
        {
            // Получаем текущий UV Rect
            _currentUvRect = _rawImage.uvRect;

            // Вычисляем новое смещение (Offset)
            float newX = _currentUvRect.x + _scrollSpeed.x * deltaTime;
            float newY = _currentUvRect.y + _scrollSpeed.y * deltaTime;

            // Зацикливаем значения от 0 до 1, чтобы избежать переполнения float
            // (хотя для UV это не критично, но так чище)
            newX = Mathf.Repeat(newX, 1f);
            newY = Mathf.Repeat(newY, 1f);

            // Применяем новые координаты, сохраняя размер (W, H)
            _rawImage.uvRect = new Rect(newX, newY, _currentUvRect.width, _currentUvRect.height);
        }
    }
}