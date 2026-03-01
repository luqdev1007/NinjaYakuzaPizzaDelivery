using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class PreperationTimerView : MonoBehaviour, IShowableView
    {
        [SerializeField] private Animator _animator;

        public Tween Hide()
        {
            _animator.SetTrigger("Hide");

            return null;
        }

        public Tween Show()
        {
            _animator.SetTrigger("Show");

            return null;
        }
    }
}