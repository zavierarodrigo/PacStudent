using UnityEngine;
using System.Collections;

public class PacStudentIdleAnimation : MonoBehaviour
{
    [SerializeField] private Sprite pacOpen;
    [SerializeField] private Sprite pacClosed;
    [SerializeField] private float frameDelay = 0.2f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(IdleAnimation());
    }

    private IEnumerator IdleAnimation()
    {
        while (true)
        {
            spriteRenderer.sprite = pacOpen;
            yield return new WaitForSeconds(frameDelay);

            spriteRenderer.sprite = pacClosed;
            yield return new WaitForSeconds(frameDelay);
        }
    }
}