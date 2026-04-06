using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreenView : MonoBehaviour, IView
{
    public event Action StartGameButtonClicked;
    public event Action ResetStatsButtonClicked;

    [field: SerializeField] public IconTextListView WalletView { get; private set; }
    [field: SerializeField] public Button OpenAudioSettingsButton { get; private set; }

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _resetStatsButton;

    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        _resetStatsButton.onClick.AddListener(OnResetStatsButtonClicked);
    }

    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        _resetStatsButton.onClick.RemoveListener(OnResetStatsButtonClicked);
    }

    private void OnResetStatsButtonClicked()
    {
        ResetStatsButtonClicked?.Invoke();
    }

    private void OnStartGameButtonClicked()
    {
        StartGameButtonClicked?.Invoke();
    }
}
