using UnityEngine;

public class Tweener : MonoBehaviour
{
    private Tween activeTween;

    public bool IsTweening
    {
        get { return activeTween != null; }
    }

    public void AddTween(Transform target, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (activeTween == null)
        {
            activeTween = new Tween(target, startPos, endPos, Time.time, duration);
        }
    }

    void Update()
    {
        if (activeTween == null) return;

        float t = (Time.time - activeTween.StartTime) / activeTween.Duration;

        if (t >= 1f)
        {
            activeTween.Target.position = activeTween.EndPos;
            activeTween = null;
        }
        else
        {
            activeTween.Target.position = Vector3.Lerp(activeTween.StartPos, activeTween.EndPos, t);
        }
    }
}