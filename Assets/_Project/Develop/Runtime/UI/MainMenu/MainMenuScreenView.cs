using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.MainMenu;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreenView : MonoBehaviour, IView
{
    [field: SerializeField] public DojoView DojoView { get; private set; }
    [field: SerializeField] public LeaderboardView LeaderBoardView { get; private set; }
    [field: SerializeField] public ShopView ShopView { get; private set; }
    [field: SerializeField] public ExtrasView ExtrasView { get; private set; }

    [Header("Meta")]
    [field: SerializeField] public IconTextListView WalletView { get; private set; }
    [field: SerializeField] public Button ResetStatsButton { get; private set; }


    [Header("Side Utilities Buttons")]
    [field: SerializeField] public Button OpenGameSettingsButton { get; private set; }
    [field: SerializeField] public Button OpenShopButton { get; private set; }
    [field: SerializeField] public Button OpenExtrasButton { get; private set; }


    [Header("Gameplay Buttons")]
    [field: SerializeField] public Button OpenOrdersButton { get; private set; }
    [field: SerializeField] public Button OpenDojoButton { get; private set; }
    [field: SerializeField] public Button OpenLeaderboardButton { get; private set; }
}
