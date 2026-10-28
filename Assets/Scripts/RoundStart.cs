
using UnityEngine;
using TMPro;
using System.Collections;

public class RoundStart : MonoBehaviour
{
    [Header("Countdown UI")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TMP_Text countdownText;

    [Header("Countdown Settings")]
    [SerializeField] private float secondsPerNumber = 1f;

    [Header("Background Audio")]
    [SerializeField] private AudioSource introAudio;
    [SerializeField] private AudioSource normalBackgroundMusic;

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        // Pause gameplay during the countdown.
        Time.timeScale = 0f;
        PacStudentController.SetMovementEnabled(false);

        // Show the countdown overlay.
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        // Play intro music.
        if (introAudio != null)
        {
            introAudio.Stop();
            introAudio.loop = false;
            introAudio.Play();
        }

        // Keep normal background music stopped.
        if (normalBackgroundMusic != null)
        {
            normalBackgroundMusic.Stop();
        }

        StartCoroutine(PlayCountdown());
    }

    private IEnumerator PlayCountdown()
    {
        if (countdownPanel == null || countdownText == null)
        {
            Debug.LogError(
                "RoundStart: Assign Countdown Panel and Countdown Text."
            );

            EndCountdown();
            yield break;
        }

        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(secondsPerNumber);

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(secondsPerNumber);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(secondsPerNumber);

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(secondsPerNumber);

        // Hide GO! before starting the timer.
        EndCountdown();
    }

    private void EndCountdown()
    {
        // Hide the countdown overlay.
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        // Stop intro music.
        if (introAudio != null)
        {
            introAudio.Stop();
        }

        // Resume gameplay time.
        Time.timeScale = 1f;

        // Enable player movement.
        PacStudentController.SetMovementEnabled(true);

        // Start the game timer.
        if (gameManager != null)
        {
            gameManager.StartGameTimer();
        }
        else
        {
            Debug.LogError(
                "RoundStart: Game Manager is not assigned in the Inspector."
            );
        }

        // Start normal background music.
        if (normalBackgroundMusic != null)
        {
            normalBackgroundMusic.loop = true;
            normalBackgroundMusic.Play();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
