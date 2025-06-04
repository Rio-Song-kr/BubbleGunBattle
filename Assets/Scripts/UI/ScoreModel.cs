public class ScoreModel
{
    private int _totalScore;

    public void AddScore(int score)
    {
        if (score <= 0) return;

        _totalScore += score;
    }

    public void SetScore(int score)
    {
        if (score < 0) return;

        _totalScore = score;
    }

    public int GetScore() => _totalScore;

    public void ResetScore() => _totalScore = 0;
}