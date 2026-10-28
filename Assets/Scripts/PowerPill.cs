
using UnityEngine;

public class PowerPill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager manager = FindFirstObjectByType<GameManager>();

            if (manager != null)
            {
                manager.ActivatePowerPill();
                Destroy(gameObject);
            }
        }
    }
}
