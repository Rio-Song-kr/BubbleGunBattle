using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;

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