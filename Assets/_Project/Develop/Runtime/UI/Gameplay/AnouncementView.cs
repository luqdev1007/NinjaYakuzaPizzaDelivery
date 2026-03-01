using Assets._Project.Develop.Runtime.UI.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class AnouncementView : MonoBehaviour, IShowableView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _header;
        [SerializeField] private TMP_Text _subHeader;

        public void SetText(string header, string subheader = "")
        {
            _header.text = header;
            _subHeader.text = subheader;
        }

        public Tween Hide()
        {
            gameObject.SetActive(false);

            return null;
        }

        public Tween Show()
        {
            _animator.SetTrigger("Show");

            return null;
        }
    }
}