using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
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
        }

        _graphicButton.onClick.AddListener(OnGraphicClicked);
        _controlButton.onClick.AddListener(OnControlClicked);
        _audioButton.onClick.AddListener(OnAudioClicked);
        _backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnDisable()
    {
        _graphicButton.onClick.RemoveListener(OnGraphicClicked);
        _controlButton.onClick.RemoveListener(OnControlClicked);
        _audioButton.onClick.RemoveListener(OnAudioClicked);
        _backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void OnGraphicClicked() => GameManager.Instance.UI.PopUpGraphicSettings(gameObject);
    private void OnControlClicked() => GameManager.Instance.UI.PopUpControlSettings(gameObject);
    private void OnAudioClicked() => GameManager.Instance.UI.PopUpAudioSettings(gameObject);
    private void OnBackClicked() => GameManager.Instance.UI.PopFromStack(gameObject);
}