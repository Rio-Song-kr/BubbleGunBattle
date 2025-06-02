using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    private GameObject _graphicSettingsPanel;
    private GameObject _controlSettingsPanel;
    private GameObject _audioSettingsPanel;

    private Stack<GameObject> _settingsStack = new Stack<GameObject>();

    private Button _graphicButton;
    private Button _controlButton;
    private Button _audioButton;
    private Button _backButton;

    private void OnEnable()
    {
        var children = GetComponentsInChildren<RectTransform>(true);

        foreach (var child in children)
        {
            if (child.transform.name.Equals("Graphic Button")) _graphicButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("Control Button")) _controlButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("Audio Button")) _audioButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("Back Button")) _backButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("Graphic Settings")) _graphicSettingsPanel = child.gameObject;
            else if (child.transform.name.Equals("Control Settings")) _controlSettingsPanel = child.gameObject;
            else if (child.transform.name.Equals("Audio Settings")) _audioSettingsPanel = child.gameObject;
        }

        _graphicButton.onClick.AddListener(OnGraphicClicked);
        _controlButton.onClick.AddListener(OnControlClicked);
        _audioButton.onClick.AddListener(OnSoundClicked);
        _backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnDisable()
    {
        _graphicButton.onClick.RemoveListener(OnGraphicClicked);
        _controlButton.onClick.RemoveListener(OnControlClicked);
        _audioButton.onClick.RemoveListener(OnSoundClicked);
        _backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void PushSettingsPanel(GameObject panel)
    {
        var currentPanel = GetCurrentActiveSettingsPanel();
        if (currentPanel != null)
        {
            _settingsStack.Push(currentPanel);
            currentPanel.SetActive(false);
        }
        panel.SetActive(true);
    }

    // public void OnBackInSettings()
    // {
    //     if (_settingsStack.Count > 0)
    //     {
    //         GameObject previousPanel = _settingsStack.Pop();
    //         HideAllSettingsPanels();
    //         previousPanel.SetActive(true);
    //     }
    //     else
    //     {
    //         // Settings 메인으로 돌아가거나 Global Stack Pop
    //         GlobalUIManager.Instance.PopFromStack();
    //     }
    // }

    private void HideAllSettingsPanels()
    {
        _audioSettingsPanel.SetActive(false);
        _graphicSettingsPanel.SetActive(false);
        _controlSettingsPanel.SetActive(false);
    }

    private GameObject GetCurrentActiveSettingsPanel()
    {
        if (_audioSettingsPanel.activeInHierarchy) return _audioSettingsPanel;
        if (_graphicSettingsPanel.activeInHierarchy) return _graphicSettingsPanel;
        if (_controlSettingsPanel.activeInHierarchy) return _controlSettingsPanel;
        return null;
    }

    private void OnGraphicClicked() => PushSettingsPanel(_audioSettingsPanel);
    private void OnControlClicked() => PushSettingsPanel(_graphicSettingsPanel);
    private void OnSoundClicked() => PushSettingsPanel(_controlSettingsPanel);
    private void OnBackClicked() => GameManager.Instance.UI.PopFromStack(gameObject);
}