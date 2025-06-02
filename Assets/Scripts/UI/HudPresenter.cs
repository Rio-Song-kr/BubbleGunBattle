using UnityEngine;

public class HudPresenter : MonoBehaviour, ISettableScore, ISettableTime
{
    private readonly ScoreModel _scoreModel = new ScoreModel();
    private HudView _hudView;

    private void OnEnable()
    {
        _hudView = GetComponent<HudView>();
    }

    public void AddScore(string playerName, int score)
    {
        _scoreModel.AddScore(score);
        SetScoreText(playerName, _scoreModel.GetScore());
    }

    public void SetScore(string playerName, int score)
    {
        _scoreModel.SetScore(score);
        SetScoreText(playerName, _scoreModel.GetScore());
    }

    private void SetScoreText(string playerName, int score) => _hudView.SetScoreText(playerName, score);
    public void SetTime(float time) => _hudView.SetTimeText(time);
}