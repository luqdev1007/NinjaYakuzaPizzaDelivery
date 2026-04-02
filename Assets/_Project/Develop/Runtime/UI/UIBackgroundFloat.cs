using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIBackgroundFloat : MonoBehaviour
{
    [Header("Настройки амплитуды (сила сдвига)")]
    [SerializeField] private float _rangeX = 0.02f;
    [SerializeField] private float _rangeY = 0.02f;

    [Header("Настройки скорости")]
    [SerializeField] private float _speedX = 0.5f;
    [SerializeField] private float _speedY = 0.7f;

    private RawImage _rawImage;
    private Rect _initialRect;

    void Start()
    {
        _rawImage = GetComponent<RawImage>();
        _initialRect = _rawImage.uvRect;
    }

    void Update()
    {
        // Используем синусоиды для плавного циклического движения
        float offsetX = Mathf.Sin(Time.time * _speedX) * _rangeX;
        float offsetY = Mathf.Cos(Time.time * _speedY) * _rangeY;

        // Применяем смещение к UV координатам
        _rawImage.uvRect = new Rect(
            _initialRect.x + offsetX,
            _initialRect.y + offsetY,
            _initialRect.width,
            _initialRect.height
        );
    }
}