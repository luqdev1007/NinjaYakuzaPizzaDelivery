using Assets._Project.Develop.Runtime.UI.Core;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Hints
{
    public class HintView : PopupViewBase
    {
        [SerializeField] private TMP_Text _messageText;

        public void SetText(string text) => _messageText.text = text;
    }
}