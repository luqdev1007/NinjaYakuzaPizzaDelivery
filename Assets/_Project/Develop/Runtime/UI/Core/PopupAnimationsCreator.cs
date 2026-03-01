using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core
{
    public class PopupAnimationsCreator
    {
        public static Sequence CreateShowAnimation(
            CanvasGroup body, 
            Image anticlicker, 
            PopupAnimationTypes animationType, 
            float anticlickerMaxALpha)
        {
            switch (animationType)
            {
                case PopupAnimationTypes.None:
                    return DOTween.Sequence();

                case PopupAnimationTypes.Expand:
                    return DOTween.Sequence()
                          .Append(anticlicker
                            .DOFade(endValue: anticlickerMaxALpha, duration: 0.2f)
                            .From(0))
                .          Join(body.transform
                            .DOScale(endValue: 1, duration: 0.5f)
                            .From(0)
                            .SetEase(Ease.OutBack)
                        );

                default:
                    throw new ArgumentException(nameof(animationType));
            }
        }

        public static Sequence CreateHideAnimation(
            CanvasGroup body,
            Image anticlicker,
            PopupAnimationTypes animationType,
            float anticlickerMaxALpha)
        {
            return DOTween.Sequence();
        }
    }
}
