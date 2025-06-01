public class ScoreModel : ISettableScore
{
    private int _totalScore;
    public int TotalScore => _totalScore;

    public void AddScore(int score)
    {
        if (score <= 0) return;

        _totalScore += score;
    }

    public void ResetScore() => _totalScore = 0;
}