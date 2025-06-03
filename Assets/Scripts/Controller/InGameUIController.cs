using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIController : SceneUIController
{
    private GameObject _hudCanvasObject;
    private GameObject _gameOverObject;
    // public GameObject _pauseMenuPanel;

    private ISettableScore _hudScore;
    private ISettableTime _hudTime;

    private GameOverPresenter _gameOver;

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
        }
        _hudScore = _hudCanvasObject.GetComponent<ISettableScore>();
        _hudTime = _hudCanvasObject.GetComponent<ISettableTime>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnScoreAdded += OnScoreAdded;
        GameManager.Instance.OnTimeChanged += OnTimeChanged;
        GameManager.Instance.OnGameOver += HandleGameOver;
        _gameOver.SetUIController(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnScoreAdded -= OnScoreAdded;
        GameManager.Instance.OnTimeChanged -= OnTimeChanged;
        GameManager.Instance.OnGameOver -= HandleGameOver;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void ShowDefaultUI()
    {
        HideAllSceneUI();
        _hudCanvasObject.SetActive(true);
        _gameOverObject.SetActive(false);
    }

    public override void HideAllSceneUI()
    {
        _hudCanvasObject.SetActive(false);
        _gameOverObject.SetActive(false);
    }
    protected override GameObject GetCurrentActiveScenePanel()
    {
        if (_hudCanvasObject.activeInHierarchy) return _hudCanvasObject;
        if (_gameOverObject.activeInHierarchy) return _gameOverObject;
        //todo 다른 panel 추가 시 해당 panel 추가해야 함
        return null;
    }

    //# Game State UI
    public void PopUpGameOverUI() => PushSceneUI(_gameOverObject);
    public void HideGameOverUI() => PopSceneUI();

    private void HandleGameOver(int[] sortedScore, string[] sortedName)
    {
        PopUpGameOverUI();

        string[] messages = new string[sortedScore.Length];
        string titleText;

        if (GameManager.Instance.PlayerName == sortedName[0]) titleText = "Win!";
        else titleText = "Lose...";

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
}