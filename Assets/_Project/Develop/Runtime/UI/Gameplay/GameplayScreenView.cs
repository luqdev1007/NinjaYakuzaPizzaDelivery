using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.StyleDisplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.Timers;
using Assets._Project.Develop.Runtime.UI.Wallet;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public Button OpenGameSettingsButton { get; private set; }
        [field: SerializeField] public Button RestartButton { get; private set; }
        [field: SerializeField] public InGameTimerView TimerView { get; private set; }
        [field: SerializeField] public WalletHUDView WalletView { get; private set; }
        // ... в GameplayScreenView.cs
        [field: SerializeField] public RankStyleView StyleView { get; private set; }
    }
}

