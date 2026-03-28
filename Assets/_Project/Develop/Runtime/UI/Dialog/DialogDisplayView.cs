using Assets._Project.Develop.Runtime.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class DialogDisplayView : MonoBehaviour, IView
    {
        public event Action AppearanceFinished;

        [field: SerializeField] public TMP_Text СontentProgressText { get; private set; }

        [SerializeField] private Image _portraitImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Animator _animator;

        public void Show() => _animator.SetTrigger("Show"); 
        public void Hide() => _animator.SetTrigger("Hide");

        public void OnAppearanceAnimationEnded() => AppearanceFinished?.Invoke();

        public void SetText(string text) => СontentProgressText.text = text;
        public void SetPortrait(Sprite portrait) => _portraitImage.sprite = portrait;
        public void SetBackground(Sprite bg) => _backgroundImage.sprite = bg;
    }
}

