using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class LoadMusicPopupView : PopupViewBase
    {
        [field: SerializeField] public Button AddTrackButton { get; private set; }
    }
}
