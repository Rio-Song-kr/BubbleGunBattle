using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIController : SceneUIController
{
    private GameObject _hudCanvasObject;
    private GameObject _gameOverObject;
    private GameObject _pauseMenuObject;

    private ISettableScore _hudScore;
    private ISettableTime _hudTime;

    private GameOverPresenter _gameOver;
    private PausePresenter _pause;

    private void Awake()
    {
        var children = GetComponentsInChildren<RectTransform>(true);
        ConnectCanvas(children);
        HideAllSceneUI();
    }

    private void ConnectCanvas(RectTransform[] children)
    {
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("HUD Canvas")) _hudCanvasObject = child.gameObject;
            else if (child.gameObject.name.Equals("Game Over Canvas"))
            {
                _gameOverObject = child.gameObject;
                _gameOver = _gameOverObject.GetComponent<GameOverPresenter>();
            }
            else if (child.gameObject.name.Equals("Pause Canvas"))
            {
                _pauseMenuObject = child.gameObject;
                _pause = _pauseMenuObject.GetComponent<PausePresenter>();
            }
        }
        _hudScore = _hudCanvasObject.GetComponent<ISettableScore>();
        _hudTime = _hudCanvasObject.GetComponent<ISettableTime>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnScoreAdded += OnScoreAdded;
        GameManager.Instance.OnTimeChanged += OnTimeChanged;
        GameManager.Instance.OnGameOver += HandleGameOver;
        GameManager.Instance.Input.OnPause += GetPaused;
        _gameOver.SetUIController(this);
        _pause.SetUIController(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnScoreAdded -= OnScoreAdded;
        GameManager.Instance.OnTimeChanged -= OnTimeChanged;
        GameManager.Instance.Input.OnPause -= GetPaused;
        GameManager.Instance.OnGameOver -= HandleGameOver;
    }

    protected override void Start() => base.Start();

    public override void ShowDefaultUI()
    {
        HideAllSceneUI();
        _hudCanvasObject.SetActive(true);
        _gameOverObject.SetActive(false);
        _pauseMenuObject.SetActive(false);
    }

    public override void HideAllSceneUI()
    {
        GameManager.Instance.UI.ClearStack();
        _hudCanvasObject.SetActive(false);
        _gameOverObject.SetActive(false);
        _pauseMenuObject.SetActive(false);
    }
    protected override GameObject GetCurrentActiveScenePanel()
    {
        if (_pauseMenuObject.activeInHierarchy) return _pauseMenuObject;
        if (_gameOverObject.activeInHierarchy) return _gameOverObject;
        //# Hud는 상시 true이므로, 마지막에 체크
        if (_hudCanvasObject.activeInHierarchy) return _hudCanvasObject;
        return null;
    }

    public void PopUpGameOverUI()
    {
        HideAllSceneUI();
        PushSceneUI(_gameOverObject);
    }

    public void PopUpPauseUI()
    {
        HideAllSceneUI();
        PushSceneUI(_pauseMenuObject);
    }
    public void HideUI() => PopSceneUI();

    private void HandleGameOver(int[] sortedScore, string[] sortedName, string titleText)
    {
        PopUpGameOverUI();

        string[] messages = new string[sortedScore.Length];


        for (int i = 0; i < sortedScore.Length; i++)
        {
            messages[i] = $"{sortedName[i]} - {sortedScore[i]}";
        }

        _gameOver.HandleGameOver(messages, titleText);
    }

    //# HUD Updates
    private void OnTimeChanged(float time) => _hudTime.SetTime(time);

    private void OnScoreAdded(string player, int score)
    {
        if (score == 0) _hudScore.SetScore(player, score);
        else _hudScore.AddScore(player, score);
    }

    private void GetPaused()
    {
        if (GameManager.Instance.IsGameOver) return;
        GameManager.Instance.SetPaused(!GameManager.Instance.IsPaused);

        if (GameManager.Instance.IsPaused) PopUpPauseUI();
        else HideUI();
    }
}