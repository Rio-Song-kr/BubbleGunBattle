using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeController : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private ColorAdjustments colorAdjustments;
    private const string BRIGHTNESS_KEY = "Brightness";

    // private void Awake()
    // {
    //     if (volume.profile.TryGet(out colorAdjustments))
    //     {
    //         // 설정된 밝기 반영
    //         float savedExposure = PlayerPrefs.GetInt(BRIGHTNESS_KEY, 50);
    //         float exposure = (savedExposure - 50f) / 25f; // -2 ~ +2 범위
    //         colorAdjustments.postExposure.value = exposure;
    //     }
    // }

    private void OnEnable()
    {
        GameManager.Instance.UI.OnBrightnessChanged += ApplyExposure;
    }

    private void OnDisable()
    {
        GameManager.Instance.UI.OnBrightnessChanged += ApplyExposure;
    }

    private void ApplyExposure(int exposureValue)
    {
        if (colorAdjustments != null)
        {
            float savedExposure = exposureValue;
            float exposure = (savedExposure - 50f) / 25f; // -2 ~ +2 범위
            colorAdjustments.postExposure.value = exposure;
        }
    }
}