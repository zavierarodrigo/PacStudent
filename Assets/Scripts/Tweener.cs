using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    private readonly List<Tween> activeTweens = new List<Tween>();

    public bool TweenExists(Transform target)
    {
        for (int i = 0; i < activeTweens.Count; i++)
        {
            if (activeTweens[i].Target == target) return true;
        }
        return false;
    }

    public bool AddTween(Transform target, Vector3 startPos, Vector3 endPos, float duration, float startTime = -1f)
    {
        if (TweenExists(target)) return false;
        float start = startTime < 0f ? Time.time : startTime;
        activeTweens.Add(new Tween(target, startPos, endPos, start, duration));
        return true;
    }

    void Update()
    {
        for (int i = activeTweens.Count - 1; i >= 0; i--)
        {
            Tween tween = activeTweens[i];
            if (tween.Target == null)
            {
                activeTweens.RemoveAt(i);
                continue;
            }

            float t = (Time.time - tween.StartTime) / tween.Duration;
            if (t >= 1f)
            {
                tween.Target.position = tween.EndPos;
                activeTweens.RemoveAt(i);
            }
            else
            {
                tween.Target.position = Vector3.Lerp(tween.StartPos, tween.EndPos, t);
            }
        }
    }
}