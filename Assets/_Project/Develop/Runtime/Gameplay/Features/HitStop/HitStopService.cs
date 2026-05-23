using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HitStop
{
    public class HitStopService
    {
        private Tween _hitStopTween;
        private Coroutine _activeRoutine;

        public void PlayHitStop(float duration, float targetScale)
        {
            _hitStopTween?.Kill();
            Time.timeScale = 1f;

            _hitStopTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, targetScale, 0.02f)
                .SetUpdate(true) 
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(duration, () =>
                    {
                        _hitStopTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.05f)
                            .SetUpdate(true);
                    }).SetUpdate(true);
                });
        }
    }
}