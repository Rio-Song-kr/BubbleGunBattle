using System;
using TMPro;
using UnityEngine;

public class HudPresenter : MonoBehaviour
{
    private readonly ScoreModel _scoreModel = new ScoreModel();
    private HudView _hudView;

    private void Start()
    {
        _hudView = GetComponent<HudView>();
    }

    public void IncreaseScore(string playerName, int score)
    {
        _scoreModel.AddScore(score);
        SetScoreText(playerName, _scoreModel.TotalScore);
    }

    private void SetScoreText(string playerName, int score)
    {
        _hudView.SetScoreText(playerName, score);
    }
}