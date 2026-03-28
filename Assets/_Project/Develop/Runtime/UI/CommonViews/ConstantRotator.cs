using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    public class ConstantRotator : MonoBehaviour
    {
        [SerializeField] private float _duration = 2f; // Время одного полного оборота

        private void Start()
        {
            // Вращаем по Z на 360 градусов относительно текущего угла
            transform.DOLocalRotate(new Vector3(0, 0, 360), _duration, RotateMode.FastBeyond360)
                .SetRelative(true)            // Вращение относительно текущего состояния
                .SetEase(Ease.Linear)         // Линейно, без замедлений/ускорений
                .SetLoops(-1, LoopType.Incremental); // Бесконечно в одном направлении
        }

        private void OnDestroy()
        {
            // Хорошая практика — убивать твин при уничтожении объекта
            transform.DOKill();
        }
    }
}