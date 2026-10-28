using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    [SerializeField] private Tweener tweener;
    [SerializeField] private LevelGrid grid;
    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private Transform levelMask;
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private float tilesPerSecond = 4f;
    [SerializeField] private float marginTiles = 2f;
    [SerializeField] private int cherrySortingOrder = 100;

    private Vector3 center;
    private float tile;
    private float halfWidth;
    private float halfHeight;

    void Start()
    {
        MeasureLevel();
        FitMask();
        StartCoroutine(SpawnLoop());
    }

    private void MeasureLevel()
    {
        Vector3 topLeft = grid.CellToWorld(Vector2Int.zero);
        Vector3 bottomRight = grid.CellToWorld(new Vector2Int(grid.Cols - 1, grid.Rows - 1));
        Vector3 nextCell = grid.CellToWorld(new Vector2Int(1, 0));

        tile = Mathf.Abs(nextCell.x - topLeft.x);
        center = (topLeft + bottomRight) * 0.5f;
        halfWidth = grid.Cols * tile * 0.5f;
        halfHeight = grid.Rows * tile * 0.5f;
    }

    private void FitMask()
    {
        if (levelMask == null) return;
        levelMask.position = center;
        levelMask.localScale = new Vector3(halfWidth * 2f, halfHeight * 2f, 1f);
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            GameObject cherry = SpawnCherry();

            while (cherry != null)
            {
                if (!tweener.TweenExists(cherry.transform))
                {
                    Destroy(cherry);
                }
                yield return null;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private GameObject SpawnCherry()
    {
        float margin = marginTiles * tile;
        Vector3 offset = RandomEdgeOffset(halfWidth + margin, halfHeight + margin);
        Vector3 start = center + offset;
        Vector3 end = center - offset;
        float duration = Vector3.Distance(start, end) / (tilesPerSecond * tile);

        GameObject cherry = Instantiate(cherryPrefab, start, Quaternion.identity, transform);

        SpriteRenderer sr = cherry.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            sr.sortingOrder = cherrySortingOrder;
        }

        tweener.AddTween(cherry.transform, start, end, duration);
        return cherry;
    }

    private Vector3 RandomEdgeOffset(float hx, float hy)
    {
        float x = Random.Range(-hx, hx);
        float y = Random.Range(-hy, hy);

        switch (Random.Range(0, 4))
        {
            case 0: return new Vector3(x, hy, 0f);
            case 1: return new Vector3(x, -hy, 0f);
            case 2: return new Vector3(-hx, y, 0f);
            default: return new Vector3(hx, y, 0f);
        }
    }
}