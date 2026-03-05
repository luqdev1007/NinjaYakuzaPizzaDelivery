using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreenView : MonoBehaviour, IView
{
    public event Action StartGameButtonClicked;
    public event Action ResetStatsButtonClicked;
    public event Action OpenChatButtonClicked;

    [SerializeField] private IconTextView _goldView;
    [SerializeField] private IconTextView _diamondView;
    [SerializeField] private IconTextView _winsView;
    [SerializeField] private IconTextView _losesView;

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

    public void EnableResetButton()
    {
        _resetStatsButton.interactable = true;
    }

    public void DisableResetButton()
    {
        _resetStatsButton.interactable = false;
    }

    public void SetWinsText(string value)
    {
        _winsView.SetText(value);
    }

    public void SetLosesText(string value)
    {
        _losesView.SetText(value);
    }

    public void SetGoldText(string value)
    {
        _goldView.SetText(value);
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
