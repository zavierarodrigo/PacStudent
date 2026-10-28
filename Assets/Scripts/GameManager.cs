
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Lives")]
    public GameObject[] lifeIcons;
    [SerializeField] private int startingLives = 3;
    private int lives;

    [Header("Game Timer")]
    public TMP_Text gameTimerText;
    private float elapsedTime = 0f;
    private bool timerRunning = false;

    [Header("Score")]
    public TMP_Text scoreText;
    [SerializeField] private ScoreManager scoreManager;
    private int fallbackScore = 0;

    [Header("Power Pills")]
    public TMP_Text ghostTimerText;
    public Animator[] ghostAnimators;
    private Coroutine powerPillRoutine;

    [Header("Game Over")]
    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private Behaviour[] ghostMovementScripts;
    [SerializeField] private Selectable[] uiButtonsToDisable;
    [SerializeField] private string startSceneName = "StartScene";
    [SerializeField] private float gameOverDisplaySeconds = 3f;

    private bool gameEnded = false;

    public float ElapsedTime
    {
        get { return elapsedTime; }
    }

    public bool GameEnded
    {
        get { return gameEnded; }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        timerRunning = false;
        gameEnded = false;

        lives = startingLives;

        if (lifeIcons != null)
        {
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null)
                    lifeIcons[i].SetActive(i < lives);
            }
        }

        fallbackScore = 0;
        elapsedTime = 0f;

        UpdateScoreDisplay();
        UpdateTimerDisplay();

        if (ghostTimerText != null)
            ghostTimerText.gameObject.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (gameEnded)
            return;

        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }

        // Finish when every pellet has been eaten.
        if (levelGrid != null &&
            levelGrid.RemainingPellets <= 0)
        {
            EndGame();
        }
    }

    public void StartGameTimer()
    {
        if (!gameEnded)
            timerRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt(
            (elapsedTime * 1000f) % 1000f
        );

        if (gameTimerText != null)
        {
            gameTimerText.text = string.Format(
                "{0:00}:{1:00}:{2:000}",
                minutes,
                seconds,
                milliseconds
            );
        }
    }

    private void UpdateScoreDisplay()
    {
        int currentScore = scoreManager != null
            ? scoreManager.Score
            : fallbackScore;

        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    public void AddScore(int points)
    {
        if (gameEnded)
            return;

        if (scoreManager != null)
            scoreManager.AddScore(points);
        else
            fallbackScore += points;

        UpdateScoreDisplay();
    }

    public void ActivatePowerPill()
    {
        if (gameEnded)
            return;

        AddScore(50);

        if (powerPillRoutine != null)
            StopCoroutine(powerPillRoutine);

        powerPillRoutine = StartCoroutine(PowerPillTimer());
    }

    private IEnumerator PowerPillTimer()
    {
        if (ghostTimerText != null)
            ghostTimerText.gameObject.SetActive(true);

        SetGhostState("Scared");

        for (int secondsLeft = 10; secondsLeft > 0; secondsLeft--)
        {
            if (gameEnded)
                yield break;

            if (ghostTimerText != null)
                ghostTimerText.text = secondsLeft.ToString();

            if (secondsLeft == 3)
                SetGhostState("Recovering");

            yield return new WaitForSeconds(1f);
        }

        if (!gameEnded)
            SetGhostState("Normal");

        if (ghostTimerText != null)
            ghostTimerText.gameObject.SetActive(false);

        powerPillRoutine = null;
    }

    private void SetGhostState(string stateName)
    {
        if (ghostAnimators == null)
            return;

        foreach (Animator ghostAnimator in ghostAnimators)
        {
            if (ghostAnimator != null)
                ghostAnimator.Play(stateName);
        }
    }

    public void LoseLife()
    {
        if (gameEnded || lives <= 0)
            return;

        lives--;

        if (lifeIcons != null &&
            lives < lifeIcons.Length &&
            lifeIcons[lives] != null)
        {
            lifeIcons[lives].SetActive(false);
        }

        if (lives <= 0)
            EndGame();
    }

    public void EndGame()
    {
        // Prevent multiple calls from starting multiple coroutines.
        if (gameEnded)
            return;

        gameEnded = true;
        timerRunning = false;

        // Stop player movement and freeze gameplay.
        PacStudentController.SetMovementEnabled(false);
        Time.timeScale = 0f;

        if (powerPillRoutine != null)
        {
            StopCoroutine(powerPillRoutine);
            powerPillRoutine = null;
        }

        if (ghostTimerText != null)
            ghostTimerText.gameObject.SetActive(false);

        // Disable the ghost movement components assigned in the Inspector.
        if (ghostMovementScripts != null)
        {
            foreach (Behaviour movementScript in ghostMovementScripts)
            {
                if (movementScript != null)
                    movementScript.enabled = false;
            }
        }

        // Disable assigned UI controls.
        if (uiButtonsToDisable != null)
        {
            foreach (Selectable control in uiButtonsToDisable)
            {
                if (control != null)
                    control.interactable = false;
            }
        }

        if (gameOverText != null)
            gameOverText.text = "Game Over";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        SaveHighScore();

        StartCoroutine(ReturnToStartScene());
    }

    private void SaveHighScore()
    {
        int currentScore = scoreManager != null
            ? scoreManager.Score
            : fallbackScore;

        int previousHighScore = PlayerPrefs.GetInt("HighScore", 0);
        float previousBestTime = PlayerPrefs.GetFloat(
            "BestTime",
            float.MaxValue
        );

        bool isNewBest =
            currentScore > previousHighScore ||
            (
                currentScore == previousHighScore &&
                elapsedTime < previousBestTime
            );

        // Save this run's result.
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.SetFloat("CurrentTime", elapsedTime);
        PlayerPrefs.SetInt(
            "HighScoreChanged",
            isNewBest ? 1 : 0
        );

        // Update the record if the score is higher,
        // or if the score ties and the time is faster.
        if (isNewBest)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.SetFloat("BestTime", elapsedTime);
        }

        PlayerPrefs.Save();
    }

    private IEnumerator ReturnToStartScene()
    {
        // Use real time because the game is paused.
        yield return new WaitForSecondsRealtime(
            gameOverDisplaySeconds
        );

        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
