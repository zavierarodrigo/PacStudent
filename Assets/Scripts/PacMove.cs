using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2[] waypoints;
    [SerializeField] private AudioSource moveAudioSource;

    private int currentWaypointIndex = 0;

    void Start()
    {
        transform.position = waypoints[0];
    }

    void Update()
    {
        Vector2 target = waypoints[currentWaypointIndex];
        Vector2 currentPos = transform.position;

        Vector2 direction = (target - currentPos).normalized;
        float distanceThisFrame = moveSpeed * Time.deltaTime;

        if (Vector2.Distance(currentPos, target) <= distanceThisFrame)
        {
            transform.position = target;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            transform.position += (Vector3)(direction * distanceThisFrame);
        }

        if (moveAudioSource != null && !moveAudioSource.isPlaying)
        {
            moveAudioSource.Play();
        }
    }
}