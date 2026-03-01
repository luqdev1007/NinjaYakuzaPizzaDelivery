using Assets._Project.Develop.Runtime.Gameplay.UI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.Upgrades;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public ProgressFilledImageView HealthView { get; private set; }
        [field: SerializeField] public AnouncementView AnouncementView { get; private set; }
        [field: SerializeField] public PreperationTimerView PrepTimerView { get; private set; }
        [field: SerializeField] public UpgradesPanelView UpgradesPanelView { get; private set; }
        [field: SerializeField] public TMP_Text TimerText { get; private set; }

        public void Init()
        {
            Debug.Log("Gameplay screen view Inited");
        }
    }
}