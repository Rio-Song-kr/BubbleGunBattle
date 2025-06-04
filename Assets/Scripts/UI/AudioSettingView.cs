using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingView : MonoBehaviour
{
    private Slider _bgmSlider;
    private Slider _sfxSlider;
    private TMP_Text _bgmValueText;
    private TMP_Text _sfxValueText;
    private Button _resetButton;
    private Button _backButton;
    private Button _applyButton;

    public Action<float> OnBGMSliderValueChanged;
    public Action<float> OnSFXSliderValueChanged;
    public Action OnResetButtonClicked;
    public Action OnBackButtonClicked;
    public Action OnApplyButtonClicked;

    private void OnEnable()
    {
        ConnectCanvas();
        SetUpSliders();

        _bgmSlider.onValueChanged.AddListener(BGMSliderChanged);
        _sfxSlider.onValueChanged.AddListener(SFXSliderChanged);
        _resetButton.onClick.AddListener(ResetButtonClicked);
        _backButton.onClick.AddListener(BackButtonClicked);
        _applyButton.onClick.AddListener(ApplyButtonClicked);
    }

    private void OnDisable()
    {
        _bgmSlider.onValueChanged.RemoveListener(BGMSliderChanged);
        _sfxSlider.onValueChanged.RemoveListener(SFXSliderChanged);
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        _backButton.onClick.RemoveListener(BackButtonClicked);
        _applyButton.onClick.RemoveListener(ApplyButtonClicked);
    }

    private void ConnectCanvas()
    {
        var children = gameObject.GetComponentsInChildren<RectTransform>(true);
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("BGM Slider")) _bgmSlider = child.GetComponent<Slider>();
            else if (child.gameObject.name.Equals("SFX Slider")) _sfxSlider = child.GetComponent<Slider>();
            else if (child.gameObject.name.Equals("BGM Value Text")) _bgmValueText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("SFX Value Text")) _sfxValueText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("SFX Slider")) _sfxSlider = child.GetComponent<Slider>();
            else if (child.gameObject.name.Equals("Reset Button")) _resetButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Back Button")) _backButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Apply Button")) _applyButton = child.GetComponent<Button>();
        }
    }

    private void SetUpSliders()
    {
        _bgmSlider.minValue = 0f;
        _bgmSlider.maxValue = 100f;
        _bgmSlider.wholeNumbers = true;

        _sfxSlider.minValue = 0f;
        _sfxSlider.maxValue = 100f;
        _sfxSlider.wholeNumbers = true;
    }

    public void SetBGMSlider(float value)
    {
        _bgmSlider.value = value;
        SetBGMValueText(value);
    }

    public void SetSFXSlider(float value)
    {
        _sfxSlider.value = value;
        SetSFXValueText(value);
    }

    private void SetBGMValueText(float value)
    {
        _bgmValueText.text = value.ToString("F0");
    }

    private void SetSFXValueText(float value)
    {
        _sfxValueText.text = value.ToString("F0");
    }

    private void BGMSliderChanged(float value)
    {
        OnBGMSliderValueChanged?.Invoke(value);
        SetBGMValueText(value);
    }

    private void SFXSliderChanged(float value)
    {
        OnSFXSliderValueChanged?.Invoke(value);
        SetSFXValueText(value);
    }

    private void ResetButtonClicked() => OnResetButtonClicked?.Invoke();
    private void BackButtonClicked() => OnBackButtonClicked?.Invoke();
    private void ApplyButtonClicked() => OnApplyButtonClicked?.Invoke();
}