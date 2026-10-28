
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;

    public int Score
    {
        get { return score; }
    }

    private void Start()
    {
        score = 0;
        Refresh();
    }

    public void AddScore(int points)
    {
        score += points;
        Refresh();
    }

    private void Refresh()
    {
        if (scoreText != null)
            scoreText.text = score.ToString("D6");
    }
}
