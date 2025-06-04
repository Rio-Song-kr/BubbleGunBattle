using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private TMP_Text _titleText;
    private Button _mainMenuButton;
    private Button _quitButton;
    //todo 추후 멀티플레이어 적용되면 Back to Room으로 변경
    private Button _restartButton;

    public Action OnMainMenuButtonClicked;
    public Action OnQuitButtonClicked;
    public Action OnRestartButtonClicked;

    private void Awake() => ConnectCanvas();

    private void OnEnable()
    {
        _mainMenuButton.onClick.AddListener(MainMenuButtonClicked);
        _quitButton.onClick.AddListener(QuitButtonClicked);
        _restartButton.onClick.AddListener(RestartButtonClicked);
    }

    private void OnDisable()
    {
        _mainMenuButton.onClick.RemoveListener(MainMenuButtonClicked);
        _quitButton.onClick.RemoveListener(QuitButtonClicked);
        _restartButton.onClick.RemoveListener(RestartButtonClicked);
    }

    private void ConnectCanvas()
    {
        var children = gameObject.GetComponentsInChildren<RectTransform>(true);
        foreach (var child in children)
        {
            if (child.gameObject.name.Equals("Title Text")) _titleText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Score Text")) _scoreText = child.GetComponent<TMP_Text>();
            else if (child.gameObject.name.Equals("Main Menu Button")) _mainMenuButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Quit Button")) _quitButton = child.GetComponent<Button>();
            else if (child.gameObject.name.Equals("Restart Button")) _restartButton = child.GetComponent<Button>();
        }
    }

    //# Buttons
    private void MainMenuButtonClicked() => OnMainMenuButtonClicked?.Invoke();
    private void QuitButtonClicked() => OnQuitButtonClicked?.Invoke();
    private void RestartButtonClicked() => OnRestartButtonClicked?.Invoke();

    public void SetTitleText(string text) => _titleText.text = text;

    public void AddScoreTextObject(string message, int index, bool isWinner = false)
    {
        var scoreText = Instantiate(_scoreText, _titleText.transform.parent);
        scoreText.transform.SetSiblingIndex(_titleText.transform.GetSiblingIndex() + index + 1);
        scoreText.text = message;

        if (isWinner) scoreText.color = Color.green;
    }
}