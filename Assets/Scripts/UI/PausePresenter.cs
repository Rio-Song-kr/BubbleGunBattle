using UnityEngine;

public class PausePresenter : MonoBehaviour
{
    private PauseView _pauseView;
    private InGameUIController _uiController;

    private void Awake() => _pauseView = GetComponent<PauseView>();

    private void OnEnable()
    {
        _pauseView.OnResumeButtonClicked += OnResumeButtonClicked;
        _pauseView.OnMainMenuButtonClicked += OnMainMenuButtonClicked;
        _pauseView.OnQuitButtonClicked += OnQuitButtonClicked;
        _pauseView.OnSettingsButtonClicked += OnSettingsButtonClicked;
    }

    private void OnDisable()
    {
        _pauseView.OnResumeButtonClicked -= OnResumeButtonClicked;
        _pauseView.OnMainMenuButtonClicked -= OnMainMenuButtonClicked;
        _pauseView.OnQuitButtonClicked -= OnQuitButtonClicked;
        _pauseView.OnSettingsButtonClicked -= OnSettingsButtonClicked;
    }

    public void SetUIController(InGameUIController uiController) => _uiController = uiController;

    private void OnResumeButtonClicked()
    {
        PopUIRequested();
        GameManager.Instance.SetPaused(false);
    }

    private void OnMainMenuButtonClicked()
    {
        PopUIRequested();
        GameManager.Instance.Scene.LoadSceneAsync(GameManager.Instance.TitleSceneName, true);
    }

    private void OnQuitButtonClicked() => GameManager.Instance.UI.PopUpQuitConfirm(gameObject);
    private void OnSettingsButtonClicked() => GameManager.Instance.UI.PopUpSettings(gameObject);
    private void PopUIRequested() => _uiController?.HideUI();
}