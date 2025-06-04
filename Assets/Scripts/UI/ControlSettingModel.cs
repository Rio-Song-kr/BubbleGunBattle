using UnityEngine;

[System.Serializable]
public class ControlSettingModel
{
    [SerializeField] private float _sensitivity = 1.0f;

    public float Sensitivity { get => _sensitivity; set => _sensitivity = Mathf.Clamp(value, 0.5f, 2f); }

    private const string SENSITIVITY_KEY = "Sensitivity";

    public void LoadSettings()
    {
        _sensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 1.0f);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, _sensitivity);
        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        _sensitivity = 1.0f;
    }
}