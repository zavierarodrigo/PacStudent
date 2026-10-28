using UnityEngine;

public class PacStudentCollisions : MonoBehaviour
{
    [SerializeField] private LevelGrid grid;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip cherryClip;
    [SerializeField] private int pelletPoints = 10;
    [SerializeField] private int cherryPoints = 100;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            grid.ClearPellet(grid.WorldToCell(other.transform.position));
            scoreManager.AddScore(pelletPoints);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Cherry"))
        {
            scoreManager.AddScore(cherryPoints);
            if (sfxSource != null && cherryClip != null)
            {
                sfxSource.PlayOneShot(cherryClip);
            }
            Destroy(other.gameObject);
        }
    }
}