using System;
using System.Collections.Generic;
using BallContent;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InitializationContent;
using UnityEngine;

public enum ShotType
{
    Normal,
    PowerShot
}

public class BallMover : MonoBehaviour
{
    [SerializeField] private Ball _ball;
    
    private GridManager _grid;
    private Vector3 velocity;
    private float gravity = 9.8f;
    private float left = -5f;
    private float right = 5f;
    private float top = 5f;
    private ScreenData _screenData;
    private ShotType _shotType = ShotType.Normal;
    private bool _hasPenetrated = false;

    public event Action<Ball, Vector2Int> OnBallPlaced;
    public event Action<Ball, Vector2Int> OnBallShotThrough;
    
    public bool isFlying { get; private set; } = false;

    private void Awake()
    {
        _ball = GetComponent<Ball>();
    }

    public void Init(ScreenData screenData, GridManager grid)
    {
        _screenData = screenData;
        _grid = grid;
        left = _screenData.Left;
        right = _screenData.Right;
        top = _screenData.Top;
    }

    public void Launch(Vector3 initialVelocity, ShotType shotType)
    {
        velocity = initialVelocity;
        isFlying = true;
        _shotType = shotType;
        _hasPenetrated = false;
    }

    private void Update()
    {
        if (!isFlying) return;

        Move();
    }

    private void Move()
    {
        Vector3 pos = transform.position + velocity * Time.deltaTime;

        if (pos.x - _ball.Radius <= left || pos.x + _ball.Radius >= right)
        {
            velocity.x = -velocity.x;
            pos.x = Mathf.Clamp(pos.x, left + _ball.Radius, right - _ball.Radius);
        }

        if (pos.y + _ball.Radius >= top)
        {
            isFlying = false;
            SnapToGrid(pos);
            return;
        }

        transform.position = pos;

        CheckCollision();
    }

    private void CheckCollision()
    {
        if (_grid == null) return;
        
        foreach (var kvp in _grid.GetAllBalls())
        {
            GameObject otherObj = kvp.Value;
            
            if (otherObj == null) 
                continue;
            
            Ball other = otherObj.GetComponent<Ball>();
            
            if (other == null || other == _ball) 
                continue;

            float dist = Vector2.Distance(transform.position, other.transform.position);

            if (dist <= _ball.Radius + other.Radius)
            {
                if (_shotType == ShotType.PowerShot && !_hasPenetrated)
                {
                    _hasPenetrated = true;
                    Vector2Int hitCell = _grid.GetCellFromWorldPosition(other.transform.position);
                    TriggerBounceEffect(otherObj);
                    OnBallShotThrough?.Invoke(_ball, hitCell);
                    isFlying = false;
                    return;
                }
                else
                {
                    TriggerBounceEffect(this.gameObject);
                    SnapToGrid(transform.position);
                    return;
                }
            }
        }
    }

    private async UniTask TriggerBounceEffect(GameObject centerBall)
    {
        if (_grid == null) 
            return;
        
        Vector3 centerPos = centerBall.transform.position;
        Vector2Int centerCell = _grid.GetCellFromWorldPosition(centerPos);
        List<Vector2Int> firstCircle = _grid.GetNeighbors(centerCell);
        
        foreach (var cell in firstCircle)
        {
            GameObject ball = _grid.GetBall(cell);
            
            if (ball != null)
                AnimateBounce(ball, 0.25f, 0.08f, centerPos);
        }
    
        await UniTask.Delay(80);
        List<Vector2Int> secondCircle = new List<Vector2Int>();
        
        foreach (var cell in firstCircle)
        {
            List<Vector2Int> neighbors = _grid.GetNeighbors(cell);
            
            foreach (var n in neighbors)
            {
                if (n != centerCell && !firstCircle.Contains(n) && _grid.IsCellOccupied(n))
                    secondCircle.Add(n);
            }
        }
    
        foreach (var cell in secondCircle)
        {
            GameObject ball = _grid.GetBall(cell);
            
            if (ball != null)
                AnimateBounce(ball, 0.13f, 0.06f, centerPos);
        }
    }

    private void AnimateBounce(GameObject ball, float distance, float time, Vector3 centerPos)
    {
        Vector3 originalPos = ball.transform.position;
        Vector3 direction = (ball.transform.position - centerPos).normalized;
        Vector3 bouncePos = originalPos + direction * distance;
    
        ball.transform.DOMove(bouncePos, time).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            ball.transform.DOMove(originalPos, time * 1.5f).SetEase(Ease.OutElastic);
        });
    }

    private void SnapToGrid(Vector2 hitPos)
    {
        isFlying = false;
        Vector2Int bestCell = _grid.GetClosestFreeCell(hitPos);
        Vector2 worldPos = _grid.GetWorldPosition(bestCell.x, bestCell.y);

        transform.DOMove(worldPos, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            OnBallPlaced?.Invoke(_ball, bestCell);
        });
    }

    public void Stop()
    {
        isFlying = false;
    }
}