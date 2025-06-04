using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSettingPresenter : MonoBehaviour
{
    [SerializeField] private ControlSettingModel _model;
    private ControlSettingView _view;

    private ControlSettingModel _tempModel;

    private void Awake()
    {
        _view = GetComponent<ControlSettingView>();
        _tempModel = new ControlSettingModel();
    }

    private void OnEnable()
    {
        _view.OnSensitivityValueChanged += OnSensitivityChanged;
        _view.OnResetButtonClicked += OnResetClicked;
        _view.OnBackButtonClicked += OnBackClicked;
        _view.OnApplyButtonClicked += OnApplyClicked;
    }

    private void OnDisable()
    {
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
        _tempModel.Sensitivity = _model.Sensitivity;
    }

    private void SetViewFromTemp()
    {
        _view.SetSensitivitySlider(_tempModel.Sensitivity);
    }

    private void ApplySensitivity(float value) => GameManager.Instance.Input.SetMouseSensitivity(value);

    private void OnSensitivityChanged(float value)
    {
        _tempModel.Sensitivity = value;
        ApplySensitivity(_tempModel.Sensitivity);
    }

    private void OnResetClicked()
    {
        _tempModel.ResetToDefault();
        SetViewFromTemp();

        ApplySensitivity(_tempModel.Sensitivity);
    }

    private void OnApplyClicked()
    {
        _model.Sensitivity = _tempModel.Sensitivity;

        _model.SaveSettings();

        ApplySensitivity(_model.Sensitivity);
    }

    private void OnBackClicked()
    {
        CopyModelToTemp();
        SetViewFromTemp();

        ApplySensitivity(_model.Sensitivity);

        GameManager.Instance.UI.PopFromStack(gameObject);
    }
}