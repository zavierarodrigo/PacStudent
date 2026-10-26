using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    [SerializeField] private Vector3[] corners;
    [SerializeField] private float speed = 3f;
    [SerializeField] private AudioSource moveAudio;

    private Tweener tweener;
    private int nextIndex = 1;

    void Start()
    {
        tweener = GetComponent<Tweener>();
        transform.position = corners[0];

        if (moveAudio != null)
        {
            moveAudio.loop = true;
            moveAudio.Play();
        }
    }

    void LateUpdate()
    {
        if (tweener.IsTweening) return;

        Vector3 start = transform.position;
        Vector3 end = corners[nextIndex];
        float duration = Vector3.Distance(start, end) / speed;

        tweener.AddTween(transform, start, end, duration);
        nextIndex = (nextIndex + 1) % corners.Length;
    }
}