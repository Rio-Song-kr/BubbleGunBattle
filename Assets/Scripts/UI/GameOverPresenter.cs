using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPresenter : MonoBehaviour
{
    private GameOverView _gameOverView;
    private InGameUIController _uiController;

    private void Awake() => _gameOverView = GetComponent<GameOverView>();

    private void OnEnable()
    {
        _gameOverView.OnMainMenuButtonClicked += OnMainMenuButtonClicked;
        _gameOverView.OnQuitButtonClicked += OnQuitButtonClicked;
        _gameOverView.OnRestartButtonClicked += OnRestartButtonClicked;
    }

    private void OnDisable()
    {
        _gameOverView.OnMainMenuButtonClicked -= OnMainMenuButtonClicked;
        _gameOverView.OnQuitButtonClicked -= OnQuitButtonClicked;
        _gameOverView.OnRestartButtonClicked -= OnRestartButtonClicked;
    }

    public void SetUIController(InGameUIController uiController) => _uiController = uiController;

    private void OnMainMenuButtonClicked()
    {
        PopUIRequested();
        GameManager.Instance.Scene.LoadSceneAsync(GameManager.Instance.TitleSceneName, true);
    }

    private void OnQuitButtonClicked() => GameManager.Instance.UI.PopUpQuitConfirm(gameObject);

    private void OnRestartButtonClicked()
    {
        PopUIRequested();
        GameManager.Instance.SetPlayerName(GameManager.Instance.PlayerName);
        GameManager.Instance.Scene.LoadSceneAsync(GameManager.Instance.Scene.GetActiveScene());
    }

    private void PopUIRequested() => _uiController?.HideUI();

    public void HandleGameOver(string[] messages, string titleText)
    {
        _gameOverView.SetTitleText(titleText);

        for (int i = 0; i < messages.Length; i++)
        {
            bool isWinner = i == 0;

            _gameOverView.AddScoreTextObject(messages[i], i, isWinner);
        }
    }
}