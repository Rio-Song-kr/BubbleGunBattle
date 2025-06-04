using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicSettingView : MonoBehaviour
{
    private Slider _brightnessSlider;
    private TMP_Text _brightnessText;
    private Toggle _fullscreenToggle;

    private Button _resetButton;
    private Button _backButton;
    private Button _applyButton;

    public Action<int> OnBrightnessValueChanged;
    public Action<bool> OnFullscreenToggleChanged;
    public Action OnResetButtonClicked;
    public Action OnBackButtonClicked;
    public Action OnApplyButtonClicked;

    private void OnEnable()
    {
        ConnectCanvas();
        _brightnessSlider.onValueChanged.AddListener(BrightnessValueChanged);
        _brightnessSlider.onValueChanged.AddListener(SetBrightnessText);
        _fullscreenToggle.onValueChanged.AddListener(FullscreenToggleChanged);
        _resetButton.onClick.AddListener(ResetButtonClicked);
        _backButton.onClick.AddListener(BackButtonClicked);
        _applyButton.onClick.AddListener(ApplyButtonClicked);
    }

    private void OnDisable()
    {
        _brightnessSlider.onValueChanged.RemoveListener(BrightnessValueChanged);
        _brightnessSlider.onValueChanged.RemoveListener(SetBrightnessText);
        _fullscreenToggle.onValueChanged.RemoveListener(FullscreenToggleChanged);
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        _backButton.onClick.RemoveListener(BackButtonClicked);
        _applyButton.onClick.RemoveListener(ApplyButtonClicked);
    }

    private void ConnectCanvas()
    {
        var children = gameObject.GetComponentsInChildren<RectTransform>(true);
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Brightness Slider")) _brightnessSlider = child.GetComponent<Slider>();
            else if (child.gameObject.name.Equals("Brightness Value Text"))
                _brightnessText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Fullscreen Toggle")) _fullscreenToggle = child.GetComponent<Toggle>();
            else if (child.gameObject.name.Equals("Reset Button")) _resetButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Back Button")) _backButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Apply Button")) _applyButton = child.GetComponent<Button>();
        }
    }

    public void SetBrightnessSlider(int value)
    {
        _brightnessSlider.value = value;
        SetBrightnessText(value);
    }

    public void SetFullscreenToggle(bool value)
    {
        _fullscreenToggle.isOn = value;
    }

    private void SetBrightnessText(float value)
    {
        _brightnessText.text = ((int)value).ToString();
    }

    private void BrightnessValueChanged(float value) => OnBrightnessValueChanged?.Invoke((int)value);
    private void FullscreenToggleChanged(bool value) => OnFullscreenToggleChanged?.Invoke(value);
    private void ResetButtonClicked() => OnResetButtonClicked?.Invoke();
    private void BackButtonClicked() => OnBackButtonClicked?.Invoke();
    private void ApplyButtonClicked() => OnApplyButtonClicked?.Invoke();
}