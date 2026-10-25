using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScrollingBorder : MonoBehaviour
{
    [SerializeField] private float tileSize = 32f;
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private bool vertical;

    private RawImage raw;
    private RectTransform rt;
    private float offset;

    void Awake()
    {
        raw = GetComponent<RawImage>();
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        offset = (offset + scrollSpeed * Time.unscaledDeltaTime) % 1f;
        float length = vertical ? rt.rect.height : rt.rect.width;
        float tiles = length / tileSize;
        raw.uvRect = vertical ? new Rect(0f, offset, 1f, tiles)
                              : new Rect(offset, 0f, tiles, 1f);
    }
}