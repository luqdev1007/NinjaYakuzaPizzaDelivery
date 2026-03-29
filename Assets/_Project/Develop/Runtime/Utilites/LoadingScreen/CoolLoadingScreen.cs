using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilites.LoadingScreen
{
    public class CoolLoadingScreen : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _hintText;
        [SerializeField] private TMP_Text _pressAnyKeyText;

        private Tween _blinkTween;
        private Tween _hintFadeTween;

        public bool IsShown => gameObject.activeSelf;

        private void Awake()
        {
            Hide();
            DontDestroyOnLoad(this);
            _pressAnyKeyText.gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _pressAnyKeyText.gameObject.SetActive(false);
            _animator.SetTrigger("Play");
        }

        public void SetHint(string text)
        {
            // Плавная смена текста через Fade
            _hintFadeTween?.Kill();
            _hintFadeTween = _hintText.DOFade(0, 0.3f).OnComplete(() =>
            {
                _hintText.text = text;
                _hintText.DOFade(1, 0.3f);
            });
        }

        public void ShowPressAnyKey()
        {
            _pressAnyKeyText.gameObject.SetActive(true);
            _pressAnyKeyText.alpha = 0;

            // Мигание: за 0.8 сек от 0 до 1 и обратно бесконечно
            _blinkTween = _pressAnyKeyText.DOFade(1f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        public void Hide()
        {
            _blinkTween?.Kill();
            _hintFadeTween?.Kill();
            gameObject.SetActive(false);
        }
    }
}