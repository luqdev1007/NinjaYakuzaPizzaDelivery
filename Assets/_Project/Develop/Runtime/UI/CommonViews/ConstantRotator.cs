using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    public class ConstantRotator : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.3f; // Сюрикен должен крутиться быстро!

        private Tween _rotateTween;

        private void Start()
        {
            _rotateTween = transform.DOLocalRotate(new Vector3(0, 0, 360), _duration, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }

        // Метод для внешней остановки
        public void StopRotation()
        {
            _rotateTween?.Kill();
        }

        private void OnDestroy()
        {
            _rotateTween?.Kill();
        }
    }
}