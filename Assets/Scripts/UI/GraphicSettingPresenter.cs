using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicSettingPresenter : MonoBehaviour
{
    [SerializeField] private VolumeProfile _volumeProfile;
    [SerializeField] private GraphicSettingModel _model;
    private GraphicSettingView _view;
    private GraphicSettingModel _tempModel;

    private void Awake()
    {
        _view = GetComponent<GraphicSettingView>();
        _tempModel = new GraphicSettingModel();
    }

    private void OnEnable()
    {
        _view.OnBrightnessValueChanged += OnBrightnessChanged;
        _view.OnFullscreenToggleChanged += OnFullscreenChanged;
        _view.OnResetButtonClicked += OnResetClicked;
        _view.OnBackButtonClicked += OnBackClicked;
        _view.OnApplyButtonClicked += OnApplyClicked;
    }

    private void OnDisable()
    {
        _view.OnBrightnessValueChanged -= OnBrightnessChanged;
        _view.OnFullscreenToggleChanged -= OnFullscreenChanged;
        _view.OnResetButtonClicked -= OnResetClicked;
        _view.OnBackButtonClicked -= OnBackClicked;
        _view.OnApplyButtonClicked -= OnApplyClicked;
    }

    public void Init()
    {
        _model.LoadSettings();
        CopyModelToTemp();
        SetViewFromTemp();
    }

    private void CopyModelToTemp()
    {
        _tempModel.Brightness = _model.Brightness;
        _tempModel.IsFullscreen = _model.IsFullscreen;
    }

    private void SetViewFromTemp()
    {
        _view.SetBrightnessSlider(_tempModel.Brightness);
        _view.SetFullscreenToggle(_tempModel.IsFullscreen);
    }

    //todo 현재 동작하지 않음 수정해야 함
    // private void ApplyBrightness(int value)
    // {
    //     float normalizedBrightness = value / 100f;
    //     Screen.brightness = normalizedBrightness;
    // }

    private void ApplyBrightness(int value)
    {
        float exposure = (value - 50f) / 25f; // -2 ~ +2 범위

        ColorAdjustments colorAdjustments;

        if (_volumeProfile.TryGet(out colorAdjustments))
        {
            colorAdjustments.postExposure.value = exposure;
        }
    }

    private void ApplyFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    private void OnBrightnessChanged(int value)
    {
        _tempModel.Brightness = value;
        ApplyBrightness(_tempModel.Brightness);
        GameManager.Instance.UI.OnBrightnessChanged?.Invoke(_tempModel.Brightness);
    }

    private void OnFullscreenChanged(bool value)
    {
        _tempModel.IsFullscreen = value;
        ApplyFullscreen(_tempModel.IsFullscreen);
    }

    private void OnResetClicked()
    {
        _tempModel.ResetToDefault();
        SetViewFromTemp();

        ApplyBrightness(_tempModel.Brightness);
        ApplyFullscreen(_tempModel.IsFullscreen);
    }

    private void OnApplyClicked()
    {
        _model.Brightness = _tempModel.Brightness;
        _model.IsFullscreen = _tempModel.IsFullscreen;

        _model.SaveSettings();

        ApplyBrightness(_model.Brightness);
        ApplyFullscreen(_model.IsFullscreen);
    }

    private void OnBackClicked()
    {
        CopyModelToTemp();
        SetViewFromTemp();

        ApplyBrightness(_model.Brightness);
        ApplyFullscreen(_model.IsFullscreen);

        GameManager.Instance.UI.PopFromStack(gameObject);
    }
}