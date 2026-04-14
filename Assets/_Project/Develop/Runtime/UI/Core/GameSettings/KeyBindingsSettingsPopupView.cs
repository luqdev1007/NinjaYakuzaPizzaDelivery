using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class KeyBindingsSettingsPopupView : PopupViewBase
    {
        [field: SerializeField] public Button ResetBindsButton { get; private set; }
    }
}
