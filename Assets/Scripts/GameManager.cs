
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] lifeIcons;
    private int lives = 3;

    void Start()
    {
        lives = 3;

        foreach (GameObject icon in lifeIcons)
        {
            if (icon != null)
                icon.SetActive(true);
        }
    }

    public void LoseLife()
    {
        if (lives <= 0) return;

        lives--;

        if (lifeIcons != null && lives < lifeIcons.Length)
        {
            lifeIcons[lives].SetActive(false);
        }

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
        }
    }
}
