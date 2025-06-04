using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    private Button _resumeButton;
    private Button _mainMenuButton;
    private Button _quitButton;
    private Button _settingsButton;

    public Action OnResumeButtonClicked;
    public Action OnMainMenuButtonClicked;
    public Action OnQuitButtonClicked;
    public Action OnSettingsButtonClicked;

    private void Awake() => ConnectCanvas();

    private void OnEnable()
    {
        _resumeButton.onClick.AddListener(ResumeButtonClicked);
        _mainMenuButton.onClick.AddListener(MainMenuButtonClicked);
        _quitButton.onClick.AddListener(QuitButtonClicked);
        _settingsButton.onClick.AddListener(SettingsButtonClicked);
    }

    private void OnDisable()
    {
        _resumeButton.onClick.RemoveListener(ResumeButtonClicked);
        _mainMenuButton.onClick.RemoveListener(MainMenuButtonClicked);
        _quitButton.onClick.RemoveListener(QuitButtonClicked);
        _settingsButton.onClick.RemoveListener(SettingsButtonClicked);
    }

    private void ConnectCanvas()
    {
        var children = gameObject.GetComponentsInChildren<RectTransform>(true);
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Resume Button")) _resumeButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Main Menu Button")) _mainMenuButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Quit Button")) _quitButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Settings Button")) _settingsButton = child.GetComponent<Button>();
        }
    }

    private void ResumeButtonClicked() => OnResumeButtonClicked?.Invoke();
    private void MainMenuButtonClicked() => OnMainMenuButtonClicked?.Invoke();
    private void QuitButtonClicked() => OnQuitButtonClicked?.Invoke();
    private void SettingsButtonClicked() => OnSettingsButtonClicked?.Invoke();
}