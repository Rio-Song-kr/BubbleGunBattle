using UnityEngine;
using TMPro;

public class HudView : MonoBehaviour
{
    private GameObject _scorePanel;
    private GameObject _timePanel;

    private TMP_Text _scoreText;
    private TMP_Text _timeText;

    private void Awake()
    {
        var childrenObjects = gameObject.GetComponentsInChildren<RectTransform>(true);

        foreach (var children in childrenObjects)
        {
            if (children.name.Equals("ScorePanel")) _scorePanel = children.gameObject;
            else if (children.name.Equals("TimePanel")) _timePanel = children.gameObject;

            if (_scorePanel != null && _timePanel != null) break;
        }

        Debug.Log($"{_scorePanel.name}, {_timePanel.name}");
        _scoreText = _scorePanel.GetComponentInChildren<TMP_Text>();
        _timeText = _timePanel.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        _scorePanel.SetActive(true);
        _timePanel.SetActive(true);
    }

    private void OnDisable()
    {
        _scorePanel.SetActive(false);
        _timePanel.SetActive(false);
    }

    public void SetScoreText(string playerName, int score)
    {
        _scoreText.text = $"{playerName} : {score}";
    }

    public void SetTimeText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        _timeText.text = $"{minutes:D2}:{seconds:D2}";
    }
}