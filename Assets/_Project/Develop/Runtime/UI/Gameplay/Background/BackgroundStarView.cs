using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Background
{
    public class BackgroundStarView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RectTransform _starNormal;
        [SerializeField] private RectTransform _starDiag;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _glow;

        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 20f;
        [SerializeField] private float _pulseDuration = 2f;
        [SerializeField] private float _minGlowAlpha = 0.3f;
        [SerializeField] private float _maxGlowAlpha = 0.8f;

        private void Start()
        {
            // Рандомизируем параметры для каждой конкретной звезды
            RandomizeSettings();
            InitState();
            StartAnimate();
        }

        private void RandomizeSettings()
        {
            // 1. Рандомная скорость вращения и направление (вправо или влево)
            _rotationSpeed *= Random.Range(0.5f, 1.5f) * (Random.value > 0.5f ? 1 : -1);

            // 2. Рандомная длительность пульсации
            _pulseDuration *= Random.Range(0.8f, 1.2f);

            // 3. Рандомный начальный угол поворота, чтобы не все стояли ровно
            transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        private void InitState()
        {
            _canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;
        }

        private void StartAnimate()
        {
            // Добавляем случайную задержку появления (чтобы звезды зажигались не все сразу)
            float appearanceDelay = Random.Range(0f, 2f);

            _canvasGroup.DOFade(1f, 3f).SetEase(Ease.InQuad).SetDelay(appearanceDelay);
            transform.DOScale(1f, 3f).SetEase(Ease.OutBack).SetDelay(appearanceDelay).OnComplete(() =>
            {
                // Запускаем бесконечные циклы только после появления
                StartInfiniteTweens();
            });
        }

        private void StartInfiniteTweens()
        {
            // Вращение
            transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + 360), 360f / Mathf.Abs(_rotationSpeed), RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);

            // Пульсация Glow с рандомной задержкой старта цикла
            _glow.DOFade(_maxGlowAlpha, _pulseDuration)
                .From(_minGlowAlpha)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(Random.Range(0f, _pulseDuration)); // Сдвиг фазы

            ApplyBreathingEffect();
        }

        private void ApplyBreathingEffect()
        {
            float breathOffset = Random.Range(0f, _pulseDuration);

            // Слегка рандомизируем силу "дыхания"
            float normalScale = Random.Range(1.05f, 1.2f);
            float diagScale = Random.Range(0.8f, 0.95f);

            _starNormal.DOScale(normalScale, _pulseDuration * 1.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(breathOffset);

            _starDiag.DOScale(diagScale, _pulseDuration * 1.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(breathOffset);
        }

        private void OnDestroy()
        {
            transform.DOKill();
            _glow.DOKill();
            _starNormal.DOKill();
            _starDiag.DOKill();
            _canvasGroup.DOKill();
        }
    }
}