using UnityEngine;

public class AudioSettingPresenter : MonoBehaviour
{
    [SerializeField] private AudioSettingModel _model;
    private AudioSettingView _view;

    private AudioSettingModel _tempModel; // 임시 설정값 (Apply 전까지)

    private bool _isFirstTime = true;

    private void Awake()
    {
        _view = GetComponent<AudioSettingView>();
        _tempModel = new AudioSettingModel();
    }

    private void OnEnable()
    {
        _view.OnBGMSliderValueChanged += OnBGMVolumeChanged;
        _view.OnSFXSliderValueChanged += OnSFXVolumeChanged;
        _view.OnResetButtonClicked += OnResetClicked;
        _view.OnBackButtonClicked += OnBackClicked;
        _view.OnApplyButtonClicked += OnApplyClicked;
    }

    private void OnDisable()
    {
        _view.OnBGMSliderValueChanged -= OnBGMVolumeChanged;
        _view.OnSFXSliderValueChanged -= OnSFXVolumeChanged;
        _view.OnResetButtonClicked -= OnResetClicked;
        _view.OnBackButtonClicked -= OnBackClicked;
        _view.OnApplyButtonClicked -= OnApplyClicked;

        OnBackClicked();
    }

    public void Init()
    {
        _model.LoadSettings();
        CopyModelToTemp();

        SetViewFromTemp();
    }

    private void CopyModelToTemp()
    {
        _tempModel.BGMVolume = _model.BGMVolume;
        _tempModel.SFXVolume = _model.SFXVolume;
    }

    private void SetViewFromTemp()
    {
        _view.SetBGMSlider(_tempModel.BGMVolume);
        _view.SetSFXSlider(_tempModel.SFXVolume);
    }

    private void ApplyBGMVolumeToMixer(float volume)
    {
        //# 0-100 범위를 -80 to 0 dB로 변환
        float dbValue = volume > 0 ? Mathf.Log10(volume / 100f) * 20f : -80f;
        GameManager.Instance.Audio.AudioData.BGMAudioMixer.audioMixer.SetFloat("BGMVolume", dbValue);
    }

    private void ApplySFXVolumeToMixer(float volume)
    {
        //# 0-100 범위를 -80 to 0 dB로 변환
        float dbValue = volume > 0 ? Mathf.Log10(volume / 100f) * 20f : -80f;

        GameManager.Instance.Audio.AudioData.SFXAudioMixer.audioMixer.SetFloat("SFXVolume", dbValue);
    }

    private void OnBGMVolumeChanged(float value)
    {
        _tempModel.BGMVolume = value;
        ApplyBGMVolumeToMixer(_tempModel.BGMVolume);
    }

    private void OnSFXVolumeChanged(float value)
    {
        _tempModel.SFXVolume = value;
        ApplySFXVolumeToMixer(_tempModel.SFXVolume);

        if (_isFirstTime)
        {
            _isFirstTime = !_isFirstTime;
            return;
        }
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.ButtonClick, _tempModel.SFXVolume / 100f);
    }

    private void OnResetClicked()
    {
        _tempModel.ResetToDefault();
        SetViewFromTemp();

        ApplyBGMVolumeToMixer(_tempModel.BGMVolume);
        ApplySFXVolumeToMixer(_tempModel.SFXVolume);
    }

    private void OnApplyClicked()
    {
        _model.BGMVolume = _tempModel.BGMVolume;
        _model.SFXVolume = _tempModel.SFXVolume;

        _model.SaveSettings();

        ApplyBGMVolumeToMixer(_model.BGMVolume);
        ApplySFXVolumeToMixer(_model.SFXVolume);
    }

    private void OnBackClicked()
    {
        CopyModelToTemp();
        SetViewFromTemp();

        ApplyBGMVolumeToMixer(_model.BGMVolume);
        ApplySFXVolumeToMixer(_model.SFXVolume);

        GameManager.Instance.UI.PopFromStack(gameObject);
    }
}