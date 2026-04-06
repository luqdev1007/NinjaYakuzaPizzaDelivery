using UnityEngine;
using DG.Tweening; // Не забудь добавить неймспейс

public class TargetMarkerAnimator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveDistance = 0.5f; // На сколько двигаться вверх-вниз
    [SerializeField] private float _moveDuration = 1f;   // Скорость движения
    [SerializeField] private float _pulseScale = 1.2f;   // На сколько увеличиваться при пульсации
    [SerializeField] private float _pulseDuration = 0.5f; // Скорость пульсации

    private void Start()
    {
        StartAnimations();
    }

    private void StartAnimations()
    {
        // 1. Движение вверх-вниз (Local Move)
        // SetRelative(true) позволяет двигаться ОТ текущей позиции, а не в глобальные координаты
        transform.DOLocalMoveY(transform.localPosition.y + _moveDistance, _moveDuration)
            .SetEase(Ease.InOutSine) // Плавное замедление в крайних точках
            .SetLoops(-1, LoopType.Yoyo); // Бесконечно (-1) туда-обратно (Yoyo)

        // 2. Пульсация масштаба
        transform.DOScale(Vector3.one * _pulseScale, _pulseDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        // Хорошим тоном считается убивать твины при уничтожении объекта, 
        // чтобы не было утечек в памяти, если объект удалится в середине игры
        transform.DOKill();
    }
}