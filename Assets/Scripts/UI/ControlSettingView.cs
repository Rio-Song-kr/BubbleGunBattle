using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Timeline.Actions;

public class ControlSettingView : MonoBehaviour
{
    private Slider _mouseSensitivitySlider;
    private TMP_Text _mouseSensitivityText;

    private Button _resetButton;
    private Button _backButton;
    private Button _applyButton;

    public Action<float> OnSensitivityValueChanged;
    public Action OnResetButtonClicked;
    public Action OnBackButtonClicked;
    public Action OnApplyButtonClicked;

    private void OnEnable()
    {
        ConnectCanvas();
        _mouseSensitivitySlider.onValueChanged.AddListener(SensitivityValueChanged);
        _mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivityText);
        _resetButton.onClick.AddListener(ResetButtonClicked);
        _backButton.onClick.AddListener(BackButtonClicked);
        _applyButton.onClick.AddListener(ApplyButtonClicked);
    }

    private void OnDisable()
    {
        _mouseSensitivitySlider.onValueChanged.RemoveListener(GameManager.Instance.Input.SetMouseSensitivity);
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        _backButton.onClick.RemoveListener(BackButtonClicked);
        _applyButton.onClick.RemoveListener(ApplyButtonClicked);
    }

    private void ConnectCanvas()
    {
        var children = gameObject.GetComponentsInChildren<RectTransform>(true);
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Mouse Sensitivity Slider")) _mouseSensitivitySlider = child.GetComponent<Slider>();
            else if (child.gameObject.name.Equals("Sensitivity Value Text"))
                _mouseSensitivityText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Reset Button")) _resetButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Back Button")) _backButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Apply Button")) _applyButton = child.GetComponent<Button>();
        }
    }

    public void SetSensitivitySlider(float value)
    {
        _mouseSensitivitySlider.value = value;
        SetMouseSensitivityText(value);
    }

    private void SetMouseSensitivityText(float value)
    {
        _mouseSensitivityText.text = value.ToString("0.0");
    }

    private void SensitivityValueChanged(float value) => OnSensitivityValueChanged?.Invoke(value);
    private void ResetButtonClicked() => OnResetButtonClicked?.Invoke();
    private void BackButtonClicked() => OnBackButtonClicked?.Invoke();
    private void ApplyButtonClicked() => OnApplyButtonClicked?.Invoke();
}