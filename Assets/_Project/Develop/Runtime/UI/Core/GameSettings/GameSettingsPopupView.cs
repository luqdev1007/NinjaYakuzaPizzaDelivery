using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class GameSettingsPopupView : PopupViewBase
    {
        [field: SerializeField] public Button OpenAudioSettings { get; private set; }
        [field: SerializeField] public Button ExitGameButton { get; private set; }
    }
}
