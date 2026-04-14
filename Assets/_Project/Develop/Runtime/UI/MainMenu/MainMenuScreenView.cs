using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreenView : MonoBehaviour, IView
{
    [Header("Meta")]
    [field: SerializeField] public IconTextListView WalletView { get; private set; }
    [field: SerializeField] public Button ResetStatsButton { get; private set; }
    [field: SerializeField] public DojoView DojoView { get; private set; }

    [Header("Side Utilities Buttons")]
    [field: SerializeField] public Button OpenGameSettingsButton { get; private set; }
    [field: SerializeField] public Button OpenShopButton { get; private set; }
    [field: SerializeField] public Button OpenExtrasButton { get; private set; }


    [Header("Gameplay Buttons")]
    [field: SerializeField] public Button OpenOrdersButton { get; private set; }
    [field: SerializeField] public Button OpenDojoButton { get; private set; }
    [field: SerializeField] public Button OpenLeaderboardButton { get; private set; }
}
