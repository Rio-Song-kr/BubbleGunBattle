using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GlobalUIManager : MonoBehaviour, ISceneLoadable
{
    private GameObject _loadingCanvas;
    private GameObject _settingsCanvas;
    private GameObject _quitCanvas;

    private GameObject _audioSettingsCanvas;
    private GameObject _graphicSettingsCanvas;
    private GameObject _controlSettingsCanvas;

    private TMP_Text _loadingText;
    private TMP_Text _loadingProgressText;
    private Slider _loadingProgressBar;

    private Button _quitYesButton;
    private Button _quitNoButton;

    //# UI Stack
    private Stack<GameObject> _uiStack = new Stack<GameObject>();
    private SceneUIController _currentSceneUI;

    public Action<int> OnBrightnessChanged;

    private void Start()
    {
        ConnectLoading();
        ConnectSetting();
        ConnectQuit();
    }

    private void ConnectLoading()
    {
        var loadingObject = Resources.Load<GameObject>("LoadingCanvas");
        _loadingCanvas = Instantiate(loadingObject);
        _loadingCanvas.transform.SetParent(transform);
        _loadingCanvas.SetActive(false);

        var children = loadingObject.GetComponentsInChildren<RectTransform>(true);

        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Loading Text")) _loadingText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Loading Progress Bar")) _loadingProgressBar = child.GetComponent<Slider>();
            else if (child.gameObject.name.Equals("Loading Progress Text")) _loadingProgressText = child.GetComponent<TMP_Text>();
        }
    }

    private void ConnectSetting()
    {
        // var settingsObject = Resources.Load<GameObject>("SettingsObject");
        var settingsObject = Resources.Load<GameObject>("SettingsCanvas");
        _settingsCanvas = Instantiate(settingsObject, transform);
        _settingsCanvas.SetActive(false);

        //# Audio
        var audioSettingObject = Resources.Load<GameObject>("AudioSettingCanvas");
        _audioSettingsCanvas = Instantiate(audioSettingObject, transform);

        var audioSettingsView = _audioSettingsCanvas.GetComponent<AudioSettingPresenter>();
        audioSettingsView.Init();
        _audioSettingsCanvas.SetActive(false);

        //# Control
        var controlSettingObject = Resources.Load<GameObject>("ControlSettingCanvas");
        _controlSettingsCanvas = Instantiate(controlSettingObject, transform);

        var controlSettingView = _controlSettingsCanvas.GetComponent<ControlSettingPresenter>();
        controlSettingView.Init();
        _controlSettingsCanvas.SetActive(false);

        //# Graphic
        var graphicSettingObject = Resources.Load<GameObject>("GraphicSettingCanvas");
        _graphicSettingsCanvas = Instantiate(graphicSettingObject, transform);

        var graphicSettingView = _graphicSettingsCanvas.GetComponent<GraphicSettingPresenter>();
        graphicSettingView.Init();

        _graphicSettingsCanvas.SetActive(false);
    }

    private void ConnectQuit()
    {
        var quitObject = Resources.Load<GameObject>("QuitConfirmCanvas");
        _quitCanvas = Instantiate(quitObject);
        _quitCanvas.transform.SetParent(transform);
        _quitCanvas.SetActive(false);

        var children = _quitCanvas.GetComponentsInChildren<Button>(true);
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Yes Button")) _quitYesButton = child;
            else if (child.gameObject.name.Equals("No Button")) _quitNoButton = child;
        }

        _quitYesButton.onClick.AddListener(OnYesClicked);
        _quitNoButton.onClick.AddListener(OnNoClicked);
    }

    private void OnDestroy()
    {
        _quitYesButton.onClick.RemoveListener(OnYesClicked);
        _quitNoButton.onClick.RemoveListener(OnNoClicked);
    }

    public void SetCurrentSceneUI(SceneUIController sceneUI) => _currentSceneUI = sceneUI;
    public void PopUpSettings(GameObject currentPanel) => PushToStack(_settingsCanvas, currentPanel);
    public void PopUpGraphicSettings(GameObject currentPanel) => PushToStack(_graphicSettingsCanvas, currentPanel);
    public void PopUpControlSettings(GameObject currentPanel) => PushToStack(_controlSettingsCanvas, currentPanel);
    public void PopUpAudioSettings(GameObject currentPanel) => PushToStack(_audioSettingsCanvas, currentPanel);
    public void PopUpQuitConfirm(GameObject currentPanel) => PushToStack(_quitCanvas, currentPanel);

    //# Stack 관리
    public void PushToStack(GameObject panel, GameObject currentPanel)
    {
        currentPanel.SetActive(false);
        _uiStack.Push(currentPanel);

        ShowPanel(panel);
    }

    public void PopFromStack(GameObject currentPanel)
    {
        //# 현재 패널 비활성화
        currentPanel.SetActive(false);

        //# 스택에서 이전 패널 복원
        if (_uiStack.Count > 0)
        {
            var previousPanel = _uiStack.Pop();
            ShowPanel(previousPanel);
        }
        else
        {
            //# 스택이 비어있으면 씬 UI로 돌아가기
            _currentSceneUI?.ShowDefaultUI();
        }
    }

    public void ClearStack()
    {
        _uiStack.Clear();
        HideAllGlobalUI();
    }

    private void ShowPanel(GameObject panel)
    {
        HideAllGlobalUI();
        panel.SetActive(true);
    }

    private void HideAllGlobalUI()
    {
        _settingsCanvas.SetActive(false);
        _quitCanvas.SetActive(false);
        _audioSettingsCanvas.SetActive(false);
        _controlSettingsCanvas.SetActive(false);
        _graphicSettingsCanvas.SetActive(false);

        if (!GameManager.Instance.Scene.IsLoading)
        {
            _loadingCanvas.SetActive(false);
        }
    }

    public void InitSceneLoadingUI()
    {
        ClearStack();

        _loadingCanvas.SetActive(true);

        //# 로딩 UI 초기화
        if (_loadingProgressBar != null) _loadingProgressBar.value = 0f;
        if (_loadingProgressText != null) _loadingProgressText.text = "0%";
        if (_loadingText != null) _loadingText.text = "Loading...";
    }

    public void UpdateLoadingUI(float progress)
    {
        if (_loadingProgressBar != null) _loadingProgressBar.value = progress;

        if (_loadingProgressText != null) _loadingProgressText.text = $"{Mathf.FloorToInt(progress * 100)}%";

        if (_loadingText != null)
        {
            int dotCount = Mathf.FloorToInt(Time.unscaledTime * 2) % 4;
            _loadingText.text = "Loading" + new string('.', dotCount);
        }
    }

    public void CompleteSceneLoading()
    {
        if (_loadingText != null) _loadingText.text = "Complete!";
        _loadingCanvas.SetActive(false);
    }

    private void OnYesClicked() => Application.Quit();
    public void OnNoClicked() => PopFromStack(_quitCanvas);
}