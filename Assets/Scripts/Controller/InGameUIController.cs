using System;
using UnityEngine;

public class InGameUIController : SceneUIController
{
    private GameObject _hudCanvasObject;
    // public GameObject gameWinPanel;
    // public GameObject gameLosePanel;
    // public GameObject pauseMenuPanel;

    private ISettableScore _hudScore;
    private ISettableTime _hudTime;

    private void Awake()
    {
        var children = GetComponentsInChildren<RectTransform>(true);

        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("HUD Canvas")) _hudCanvasObject = child.gameObject;
        }
        _hudScore = _hudCanvasObject.GetComponent<ISettableScore>();
        _hudTime = _hudCanvasObject.GetComponent<ISettableTime>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnScoreAdded += OnScoreAdded;
        GameManager.Instance.OnTimeChanged += OnTimeChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnScoreAdded -= OnScoreAdded;
        GameManager.Instance.OnTimeChanged -= OnTimeChanged;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void ShowDefaultUI()
    {
        HideAllSceneUI();
        _hudCanvasObject.SetActive(true);
    }

    public override void HideAllSceneUI()
    {
        _hudCanvasObject.SetActive(false);
        // gameWinPanel.SetActive(false);
        // gameLosePanel.SetActive(false);
        // pauseMenuPanel.SetActive(false);
    }
    protected override GameObject GetCurrentActiveScenePanel()
    {
        if (_hudCanvasObject.activeInHierarchy) return _hudCanvasObject;
        //todo 다른 panel 추가 시 해당 panel 추가해야 함
        return null;
    }

    //# Game State UI
    // public void ShowGameWin()
    // {
    //     PushSceneUI(gameWinPanel);
    //     Time.timeScale = 0f; // 게임 일시정지
    // }
    //
    // public void ShowGameLose()
    // {
    //     PushSceneUI(gameLosePanel);
    //     Time.timeScale = 0f; // 게임 일시정지
    // }
    //
    // public void ShowPauseMenu()
    // {
    //     PushSceneUI(pauseMenuPanel);
    //     Time.timeScale = 0f; // 게임 일시정지
    // }
    //
    // public void OnMainMenuClicked()
    // {
    // }
    //
    //# Button Events
    // public void OnRestartClicked()
    // {
    //     Time.timeScale = 1f;
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    // }
    //
    // public void OnSettingsClicked() => GameManager.Instance.UI.PopUpSettings(gameObject);
    // public void OnExitClicked() => GameManager.Instance.UI.PopUpExitConfirm(gameObject);
    //
    // public void OnResumeClicked()
    // {
    //     PopSceneUI();
    //     Time.timeScale = 1f;
    // }

    //# HUD Updates
    private void OnTimeChanged(float time) => _hudTime.SetTime(time);
    public void OnScoreAdded(string player, int score)
    {
        if (score == 0) _hudScore.SetScore(player, score);
        else _hudScore.SetScore(player, score);
    }
}