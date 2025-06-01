using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _TimeText;

    public void SetScoreText(string playerName, int score)
    {
        _scoreText.text = $"{playerName} : {score}";
    }
}