using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioSettingModel
{
    [SerializeField] private float _bgmVolume = 50f;
    [SerializeField] private float _sfxVolume = 50f;

    public float BGMVolume { get => _bgmVolume; set => _bgmVolume = Mathf.Clamp(value, 0f, 100f); }

    public float SFXVolume { get => _sfxVolume; set => _sfxVolume = Mathf.Clamp(value, 0f, 100f); }

    // #PlayerPrefs 키
    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    public void LoadSettings()
    {
        _bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 50f);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 50f);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(BGM_KEY, _bgmVolume);
        PlayerPrefs.SetFloat(SFX_KEY, _sfxVolume);
        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        _bgmVolume = 50f;
        _sfxVolume = 50f;
    }
}