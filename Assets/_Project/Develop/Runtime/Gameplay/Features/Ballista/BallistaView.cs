using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Ballista
{
    public class BallistaView : MonoBehaviour
    {
        [SerializeField] private BallistaController _controller;
        [SerializeField] private Animator _animator;

        [Header("Recoil Settings (Сочные настройки)")]
        [Tooltip("Дистанция отката назад. Для тяжелой баллисты 1.2 — это мощно.")]
        [SerializeField] private float _recoilDistance = 1.2f;

        [Tooltip("Высота подскока. Массивное оружие должно немного 'подпрыгивать'.")]
        [SerializeField] private float _jumpHeight = 0.5f;

        [Tooltip("Угол задирания носа (в градусах).")]
        [SerializeField] private float _maxPitch = 15f;

        [Range(0, 1)][SerializeField] private float _minRecoilPower = 0.4f;

        [Header("Timing (90% времени — это возврат)")]
        [Tooltip("Удар должен быть почти мгновенным (5-8% от цикла).")]
        [Range(0, 1)][SerializeField] private float _backWeight = 0.07f;

        [Tooltip("Возврат должен быть долгим, чтобы чувствовался вес.")]
        [Range(0, 1)][SerializeField] private float _returnWeight = 0.85f;

        [Header("Jerkiness Control")]
        [Tooltip("OutExpo — идеален для тяжелых механизмов: резкий старт и очень долгое дотягивание.")]
        [SerializeField] private Ease _returnEase = Ease.OutExpo;

        private const string ParamName = "ChargeProgress";
        private Vector3 _currentPosOffset;
        private float _currentPitchOffset;

        private void OnEnable() => _controller.OnFired += PlayRelease;
        private void OnDisable() => _controller.OnFired -= PlayRelease;

        private void Update()
        {
            if (_controller.IsCharging)
                _animator.SetFloat(ParamName, _controller.ChargeProgress);
        }

        public void PlayRelease(float progress)
        {
            float totalTime = _controller.FireCycleDuration;

            // Расчет таймингов
            float backTime = totalTime * _backWeight;
            float returnTime = totalTime * _returnWeight;

            DOTween.Kill(this);

            // Сила зависит от натяжения, но даже слабый выстрел имеет вес
            float power = Mathf.Lerp(_minRecoilPower, 1f, progress);

            Vector3 targetPosOffset = new Vector3(0, _jumpHeight * power, -_recoilDistance * power);
            float targetPitch = -_maxPitch * power;

            Sequence recoilSeq = DOTween.Sequence().SetId(this);

            // ФАЗА 1: ВЗРЫВНОЙ ОТКАТ
            // Используем InQuint для позиции, чтобы создать эффект 'вылета', 
            // и OutSine для тетивы, чтобы она исчезла моментально.
            recoilSeq.Append(DOTween.To(() => _currentPosOffset, x => _currentPosOffset = x, targetPosOffset, backTime).SetEase(Ease.OutQuint));
            recoilSeq.Join(DOTween.To(() => _currentPitchOffset, x => _currentPitchOffset = x, targetPitch, backTime).SetEase(Ease.OutQuint));

            // ФАЗА 2: ТЯЖЕЛЫЙ ВОЗВРАТ
            // OutExpo дает ощущение, что баллиста 'вязнет' в воздухе, медленно возвращаясь в строй
            recoilSeq.Append(DOTween.To(() => _currentPosOffset, x => _currentPosOffset = x, Vector3.zero, returnTime).SetEase(_returnEase));
            recoilSeq.Join(DOTween.To(() => _currentPitchOffset, x => _currentPitchOffset = x, 0f, returnTime).SetEase(_returnEase));

            recoilSeq.OnUpdate(() => {
                _controller.SetRecoilOffsets(_currentPosOffset, _currentPitchOffset);
            });

            // Анимация сброса тетивы (визуально должна быть быстрее самого отката)
            _animator.SetFloat(ParamName, 0f);
            // Если аниматор не справляется мгновенно, можно оставить:
            // DOTween.To(() => progress, x => _animator.SetFloat(ParamName, x), 0f, backTime * 0.5f).SetEase(Ease.OutQuint);
        }
    }
}