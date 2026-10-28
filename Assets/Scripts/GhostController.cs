
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Ghost touched: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PacStudent touched a ghost!");
        }
    }
}
