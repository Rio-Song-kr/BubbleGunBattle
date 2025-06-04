using UnityEngine;

[System.Serializable]
public class GraphicSettingModel
{
    [SerializeField] private int _brightness = 50;
    [SerializeField] private bool _isFullscreen = true;

    public int Brightness { get => _brightness; set => _brightness = Mathf.Clamp(value, 1, 100); }
    public bool IsFullscreen { get => _isFullscreen; set => _isFullscreen = value; }

    private const string BRIGHTNESS_KEY = "Brightness";
    private const string FULLSCREEN_KEY = "Fullscreen";

    public void LoadSettings()
    {
        _brightness = PlayerPrefs.GetInt(BRIGHTNESS_KEY, 50);
        _isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(BRIGHTNESS_KEY, _brightness);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, _isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        _brightness = 50;
        _isFullscreen = true;
    }
}