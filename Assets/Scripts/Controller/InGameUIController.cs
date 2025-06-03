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

    private TMP_Text _titleText;
    [SerializeField] private TMP_Text _scoreText;
    private Button _mainMenuButton;
    private Button _quitButton;
    //todo 추후 멀티플레이어 적용되면 Back to Room으로 변경
    private Button _restartButton;

    private void Awake()
    {
        var children = GetComponentsInChildren<RectTransform>(true);
        ConnectHudCanvas(children);
        ConnectGameWinCanvas(children);
    }

    private void ConnectHudCanvas(RectTransform[] children)
    {
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("HUD Canvas")) _hudCanvasObject = child.gameObject;
        }
        _hudScore = _hudCanvasObject.GetComponent<ISettableScore>();
        _hudTime = _hudCanvasObject.GetComponent<ISettableTime>();
    }

    private void ConnectGameWinCanvas(RectTransform[] children)
    {
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Game Over Canvas")) _gameOverObject = child.gameObject;
            else if (child.gameObject.name.Equals("Title Text")) _titleText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Score Text")) _scoreText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Main Menu Button")) _mainMenuButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Quit Button")) _quitButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Restart Button")) _restartButton = child.GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnScoreAdded += OnScoreAdded;
        GameManager.Instance.OnTimeChanged += OnTimeChanged;
        GameManager.Instance.OnGameOver += HandleGameOver;
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
    public void ShowGameWinOrLose()
    {
        PushSceneUI(_gameOverObject);
    }

    //# HUD Updates
    private void OnTimeChanged(float time) => _hudTime.SetTime(time);

    private void OnScoreAdded(string player, int score)
    {
        if (score == 0) _hudScore.SetScore(player, score);
        else _hudScore.SetScore(player, score);
    }

    private void HandleGameOver(int[] sortedScore, string[] sortedName)
    {
        if (GameManager.Instance.PlayerName == sortedName[0]) _titleText.text = "Win!";
        else _titleText.text = "Lose...";

        for (int i = 0; i < sortedName.Length; i++)
        {
            var scoreText = Instantiate(_scoreText, _titleText.transform.parent);
            scoreText.transform.SetSiblingIndex(_titleText.transform.GetSiblingIndex() + i + 1);

            scoreText.text = $"{sortedName[i]} - {sortedScore[i]}";
            if (i == 0)
            {
                scoreText.color = Color.green;
            }
        }

        ShowGameWinOrLose();
    }
}