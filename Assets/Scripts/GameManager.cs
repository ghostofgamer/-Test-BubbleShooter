using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InitializationContent;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private BallPool _ballPool;
    [SerializeField] private LauncherPoint _launcherPoint;

    [Header("Game Settings")]
    [SerializeField] private int _totalBalls = 30;
    [SerializeField] private int _minBallsForWin = 3;

    private int _ballsUsed = 0;
    private bool _isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public async UniTask Init(ScreenData screenData, GridManager grid, BallPool pool)
    {
        _gridManager = grid;
        _ballPool = pool;

        await UniTask.Delay(500);
    }

    public void SpawnNewBall()
    {
        if (_isGameOver) return;

        if (_ballsUsed >= _totalBalls)
        {
            Debug.Log("LOSE - No balls left");
            return;
        }

        Ball ball = _ballPool.Get();
        ball.gameObject.SetActive(true);
        ball.transform.position = _launcherPoint.transform.position;

        _launcherPoint.SetBall(ball);
        
        _ballsUsed++;
    }

    public void OnBallPlaced(Ball ball)
    {
        Vector2Int cell = _gridManager.GetCellFromWorldPosition(ball.transform.position);
        _gridManager.AddBall(cell, ball.gameObject);

        CheckMatches(ball, cell);
    }

    public void OnBallShotThrough(Ball ball, Vector2Int targetCell)
    {
        _gridManager.RemoveBall(targetCell);
        DestroyBall(ball.gameObject);

        CheckWinCondition();

        Invoke("SpawnNewBallDelayed", 0.3f);
    }

    private void CheckMatches(Ball ball, Vector2Int cell)
    {
        string ballType = ball.GetBallType();
        Debug.Log($"CheckMatches: ballType={ballType}, cell={cell}");
        
        List<Vector2Int> matchingCells = FindConnectedBalls(cell, ballType);
        Debug.Log($"Found {matchingCells.Count} matching balls");

        if (matchingCells.Count >= 3)
        {
            foreach (var matchCell in matchingCells)
            {
                GameObject ballObj = _gridManager.GetBall(matchCell);
                if (ballObj != null)
                {
                    DestroyBall(ballObj);
                    _gridManager.RemoveBall(matchCell);
                }
            }

            RemoveFloatingBalls();
        }

        CheckWinCondition();
        
        Invoke("SpawnNewBallDelayed", 0.3f);
    }

    private void SpawnNewBallDelayed()
    {
        SpawnNewBall();
    }

    private List<Vector2Int> FindConnectedBalls(Vector2Int startCell, string ballType)
    {
        List<Vector2Int> visited = new();
        Queue<Vector2Int> queue = new();
        queue.Enqueue(startCell);
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            List<Vector2Int> neighbors = _gridManager.GetNeighbors(current);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && _gridManager.IsCellOccupied(neighbor))
                {
                    GameObject neighborBall = _gridManager.GetBall(neighbor);
                    if (neighborBall != null)
                    {
                        Ball ballComponent = neighborBall.GetComponent<Ball>();
                        if (ballComponent != null && ballComponent.GetBallType() == ballType)
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        return visited;
    }

    private void RemoveFloatingBalls()
    {
        List<Vector2Int> connectedToCeiling = GetBallsConnectedToCeiling();
        List<Vector2Int> allBalls = _gridManager.GetAllOccupiedCells();

        foreach (var cell in allBalls)
        {
            if (!connectedToCeiling.Contains(cell))
            {
                GameObject ballObj = _gridManager.GetBall(cell);
                if (ballObj != null)
                {
                    _gridManager.RemoveBall(cell);
                    DestroyBall(ballObj);
                }
            }
        }
    }

    private List<Vector2Int> GetBallsConnectedToCeiling()
    {
        List<Vector2Int> connected = new();

        for (int x = 0; x < _gridManager.Cols; x++)
        {
            Vector2Int cell = new Vector2Int(x, 0);
            if (_gridManager.IsCellOccupied(cell))
            {
                BFSConnectivity(cell, connected);
            }
        }

        return connected;
    }

    private void BFSConnectivity(Vector2Int start, List<Vector2Int> connected)
    {
        Queue<Vector2Int> queue = new();
        HashSet<Vector2Int> visited = new();

        queue.Enqueue(start);
        visited.Add(start);
        connected.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            List<Vector2Int> neighbors = _gridManager.GetNeighbors(current);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && _gridManager.IsCellOccupied(neighbor))
                {
                    visited.Add(neighbor);
                    connected.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private void DestroyBall(GameObject ball)
    {
        if (ball != null)
        {
            Destroy(ball);
        }
    }

    private void CheckWinCondition()
    {
        int totalBalls = _gridManager.GetAllOccupiedCells().Count;
        if (totalBalls <= _minBallsForWin && totalBalls > 0)
        {
            List<Vector2Int> remaining = _gridManager.GetAllOccupiedCells();
            foreach (var cell in remaining)
            {
                GameObject ballObj = _gridManager.GetBall(cell);
                if (ballObj != null)
                {
                    _gridManager.RemoveBall(cell);
                    DestroyBall(ballObj);
                }
            }
            _gridManager.ClearAll();
            
            _isGameOver = true;
            Debug.Log("WIN!");
        }
    }

    public void ConnectLauncher(LauncherPoint launcher)
    {
        launcher.OnBallPlaced += OnBallPlacedHandler;
        launcher.OnBallShotThrough += OnBallShotThroughHandler;
    }

    private void OnBallPlacedHandler(Ball ball, Vector2Int cell)
    {
        OnBallPlaced(ball);
    }

    private void OnBallShotThroughHandler(Ball ball, Vector2Int cell)
    {
        OnBallShotThrough(ball, cell);
    }
}