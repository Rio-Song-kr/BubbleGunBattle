using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIController : SceneUIController
{
    [Header("Main Menu Panels")]
    [SerializeField] private string NextScene;

    private GameObject _mainMenuCanvas;
    private Button _newGameButton;
    private Button _settingsButton;
    private Button _quitButton;

    private GameObject _newGameCanvas;
    private TMP_InputField _newGameText;
    private Button _newGameConfirmButton;

    private void OnEnable()
    {
        var children = GetComponentsInChildren<RectTransform>(true);

        foreach (var child in children)
        {
            if (child.transform.name.Equals("Main Menu Canvas")) _mainMenuCanvas = child.gameObject;
            else if (child.transform.name.Equals("New Game Button")) _newGameButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("Settings Button")) _settingsButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("Quit Button")) _quitButton = child.GetComponent<Button>();
            else if (child.transform.name.Equals("New Game Canvas")) _newGameCanvas = child.gameObject;
            else if (child.gameObject.name.Equals("Nickname Field")) _newGameText = child.GetComponent<TMP_InputField>();
            else if (child.gameObject.name.Equals("Confirm Button")) _newGameConfirmButton = child.GetComponent<Button>();
        }

        _newGameButton.onClick.AddListener(OnNewGameClicked);
        _settingsButton.onClick.AddListener(OnSettingsClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
        _newGameConfirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnDisable()
    {
        _newGameButton.onClick.RemoveListener(OnNewGameClicked);
        _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
        _newGameConfirmButton.onClick.RemoveListener(OnConfirmClicked);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void ShowDefaultUI()
    {
        HideAllSceneUI();
        _mainMenuCanvas.SetActive(true);
    }

    public override void HideAllSceneUI() => _mainMenuCanvas.SetActive(false);

    protected override GameObject GetCurrentActiveScenePanel()
    {
        if (_mainMenuCanvas.activeInHierarchy) return _mainMenuCanvas;
        return null;
    }

    // Button Events
    private void OnNewGameClicked()
    {
        _mainMenuCanvas.SetActive(false);
        _newGameCanvas.SetActive(true);
    }

    private void OnSettingsClicked() => GameManager.Instance.UI.PopUpSettings(_mainMenuCanvas);
    private void OnQuitClicked() => GameManager.Instance.UI.PopUpExitConfirm(_mainMenuCanvas);

    private void OnConfirmClicked()
    {
        if (_newGameText.text == null || _newGameText.text.Equals("")) return;

        GameManager.Instance.SetPlayerName(_newGameText.text);
        GameManager.Instance.Scene.LoadSceneAsync(NextScene);
        gameObject.SetActive(false);
    }
}