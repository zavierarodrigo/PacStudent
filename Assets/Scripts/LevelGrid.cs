using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private Vector2 origin;
    [SerializeField] private float tileSize = 1f;

    private readonly int[,] quadrant =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    private int[,] map;
    private bool[,] pellets;

    public int Rows { get { return map.GetLength(0); } }
    public int Cols { get { return map.GetLength(1); } }

    void Awake()
    {
        BuildMap();
    }

    private void BuildMap()
    {
        int qRows = quadrant.GetLength(0);
        int qCols = quadrant.GetLength(1);
        int rows = qRows * 2 - 1;
        int cols = qCols * 2;
        map = new int[rows, cols];
        pellets = new bool[rows, cols];

        for (int r = 0; r < qRows; r++)
        {
            for (int c = 0; c < qCols; c++)
            {
                int v = quadrant[r, c];
                bool hasPellet = (v == 5 || v == 6);
                SetCell(r, c, v, hasPellet);
                SetCell(r, cols - 1 - c, v, hasPellet);
                SetCell(rows - 1 - r, c, v, hasPellet);
                SetCell(rows - 1 - r, cols - 1 - c, v, hasPellet);
            }
        }
    }

    private void SetCell(int row, int col, int value, bool hasPellet)
    {
        map[row, col] = value;
        pellets[row, col] = hasPellet;
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Cols && cell.y >= 0 && cell.y < Rows;
    }

    public bool IsWalkable(Vector2Int cell)
    {
        if (!IsInside(cell)) return false;
        int v = map[cell.y, cell.x];
        return v == 0 || v == 5 || v == 6;
    }

    public bool HasPellet(Vector2Int cell)
    {
        return IsInside(cell) && pellets[cell.y, cell.x];
    }

    public void ClearPellet(Vector2Int cell)
    {
        if (IsInside(cell)) pellets[cell.y, cell.x] = false;
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(origin.x + cell.x * tileSize, origin.y - cell.y * tileSize, 0f);
    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        int col = Mathf.RoundToInt((world.x - origin.x) / tileSize);
        int row = Mathf.RoundToInt((origin.y - world.y) / tileSize);
        return new Vector2Int(col, row);
    }
}