using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InitializationContent;
using SpawnContent;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{
    public GameObject ballPrefab;

    private Dictionary<Vector2Int, GameObject> grid = new();
    public int cols = 8;
    public int rows;
    public int Cols => cols;
    public int Rows => rows;
    [SerializeField] private float _offset;

    public TextAsset levelFile;

    private float width;
    private float height;
    private float _top;
    public event Action GridCompleted;
    private BallPool _ballPool;

    public float Radius { get; private set; } = 0.5f;

    public async UniTask Init(ScreenData screen, BallPool pool)
    {
        _ballPool = pool;
        // CalculateGridSize(screen.Width, screen.Height);
        _top = screen.Top;

        LoadLevel();
    }

    public void CalculateRadius(float screenWidth, float screenHeight)
    {
        float padding = 0.9f;
        Radius = (screenWidth * padding) / (cols * 2f);

        width = Radius * 2;
        height = Mathf.Sqrt(3) * Radius;

        rows = Mathf.FloorToInt(screenHeight / height);
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        float offsetX = (y % 2 == 0) ? 0 : Radius;

        float totalWidth = cols * width + Radius;
        float worldX = x * width + offsetX - totalWidth / 2f + width / 2f;

        // верхняя граница камеры
        float top = _top;

        // первый ряд ниже верхней границы на радиус + дополнительный оффсет
        float firstRowY = top - Radius - _offset;

        float worldY = firstRowY - y * height;

        return new Vector2(worldX, worldY);
    }

    void LoadLevel()
    {
        if (levelFile == null || string.IsNullOrEmpty(levelFile.text))
        {
            Debug.LogWarning("Level file not found or empty!");
            return;
        }

        string[] lines = levelFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        rows = Mathf.Max(rows, lines.Length);
        cols = Mathf.Max(cols, lines.Length > 0 ? lines[0].Trim().Length : 0);

        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y].Trim();

            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];

                if (c == '-' || c == '.' || c == ' ')
                {
                    continue;
                }

                SpawnBall(x, y, c.ToString());
            }
        }
    }

    void SpawnBall(int x, int y, string type)
    {
        Vector2 pos = GetWorldPosition(x, y);

        Ball ballObj = _ballPool.Get();
        ballObj.transform.position = pos;

        /*// масштабируем по radius
        float spriteSize = ballObj.GetSpriteSize();
        ballObj.transform.localScale = Vector3.one * (Radius * 2f / spriteSize);*/

        ballObj.Init(type);

        grid[new Vector2Int(x, y)] = ballObj.gameObject;
    }

    // ❓ проверка занятости
    public bool IsCellOccupied(Vector2Int pos)
    {
        return grid.ContainsKey(pos);
    }

    // ➕ добавление шара
    public void AddBall(Vector2Int pos, GameObject ball)
    {
        grid[pos] = ball;
    }

    // ❌ удаление
    public void RemoveBall(Vector2Int pos, bool isDestroy = true)
    {
        if (grid.ContainsKey(pos))
        {
            GameObject ball = grid[pos];

            if (ball != null && isDestroy)
            {
                Destroy(ball);
            }

            grid.Remove(pos);
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2 pos = GetWorldPosition(x, y);

                Vector2Int key = new Vector2Int(x, y);

                if (grid.ContainsKey(key))
                    Gizmos.color = Color.green; // занято
                else
                    Gizmos.color = Color.red; // пусто

                Gizmos.DrawWireSphere(pos, Radius * 0.9f);
            }
        }
    }

    public Vector2Int GetClosestFreeCell(Vector2 hitPos)
    {
        float minDist = float.MaxValue;
        Vector2Int best = Vector2Int.zero;

        foreach (var cell in GetAllCells())
        {
            if (grid.ContainsKey(cell)) continue;

            Vector2 world = GetWorldPosition(cell.x, cell.y);

            float dist = Vector2.Distance(hitPos, world);

            if (dist < minDist)
            {
                minDist = dist;
                best = cell;
            }
        }

        return best;
    }

    public List<Vector2Int> GetAllCells()
    {
        List<Vector2Int> cells = new();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                cells.Add(new Vector2Int(x, y));
            }
        }

        return cells;
    }

    public Dictionary<Vector2Int, GameObject> GetAllBalls()
    {
        return grid;
    }

    public List<Vector2Int> GetAllOccupiedCells()
    {
        return new List<Vector2Int>(grid.Keys);
    }

    public GameObject GetBall(Vector2Int cell)
    {
        return grid.ContainsKey(cell) ? grid[cell] : null;
    }

    public List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new();
        int y = cell.y;

        int[] evenRowOffsets =
        {
            new Vector2Int(-1, 0).x, new Vector2Int(1, 0).x,
            new Vector2Int(0, -1).x, new Vector2Int(0, 1).x,
            new Vector2Int(-1, -1).x, new Vector2Int(-1, 1).x
        };
        int[] evenRowOffsetsY = { 0, 0, -1, 1, -1, 1 };

        if (y % 2 == 0)
        {
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y - 1));
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y + 1));
        }
        else
        {
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y - 1));
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y + 1));
        }

        neighbors.RemoveAll(n => n.x < 0 || n.x >= cols || n.y < 0 || n.y >= rows);
        return neighbors;
    }

    public Vector2Int GetCellFromWorldPosition(Vector2 worldPos)
    {
        float minDist = float.MaxValue;
        Vector2Int bestCell = Vector2Int.zero;

        foreach (var cell in GetAllCells())
        {
            Vector2 cellWorldPos = GetWorldPosition(cell.x, cell.y);
            float dist = Vector2.Distance(worldPos, cellWorldPos);
            if (dist < minDist)
            {
                minDist = dist;
                bestCell = cell;
            }
        }

        return bestCell;
    }

    public void ClearAll()
    {
        foreach (var kvp in grid)
        {
            if (kvp.Value != null) Destroy(kvp.Value);
        }

        grid.Clear();
    }

    public int GetMaxRowWithBalls()
    {
        int maxRow = -1;

        foreach (var cell in grid.Keys)
        {
            if (cell.y > maxRow)
                maxRow = cell.y;
        }

        return maxRow;
    }
}