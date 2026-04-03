using System;
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
    private GridManager _grid;
    public bool isFlying { get; private set; } = false;
    private Vector3 velocity;
    [SerializeField] private Ball _ball;

    [Header("Настройки")]
    public float gravity = 9.8f;
    public float left = -5f;
    public float right = 5f;
    public float top = 5f;
    public float bottom = -5f;

    private ScreenData _screenData;
    private ShotType _shotType = ShotType.Normal;
    private bool _hasPenetrated = false;

    public event Action<Ball, Vector2Int> OnBallPlaced;
    public event Action<Ball, Vector2Int> OnBallShotThrough;

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
        bottom = _screenData.Bottom;
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
        velocity.y -= gravity * Time.deltaTime;
        
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
            if (otherObj == null) continue;

            Ball other = otherObj.GetComponent<Ball>();
            if (other == null || other == _ball) continue;

            float dist = Vector2.Distance(transform.position, other.transform.position);

            if (dist <= _ball.Radius + other.Radius)
            {
                Debug.Log($"Collision! My type: {_ball.GetBallType()}, Other type: {other.GetBallType()}");
                
                if (_shotType == ShotType.PowerShot && !_hasPenetrated)
                {
                    _hasPenetrated = true;
                    Vector2Int hitCell = _grid.GetCellFromWorldPosition(other.transform.position);
                    TriggerBounceEffect(otherObj);
                    OnBallShotThrough?.Invoke(_ball, hitCell);
                    isFlying = false;
                    gameObject.SetActive(false);
                    return;
                }
                else
                {
                    TriggerBounceEffect(otherObj);
                    SnapToGrid(transform.position);
                    return;
                }
            }
        }
    }

    private void TriggerBounceEffect(GameObject otherBall)
    {
        Vector3 direction = (otherBall.transform.position - transform.position).normalized;
        Vector3 bouncePos = otherBall.transform.position - direction * _ball.Radius * 1.5f;
        Vector3 originalPos = otherBall.transform.position;

        otherBall.transform.DOMove(bouncePos, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            otherBall.transform.DOMove(originalPos, 0.15f).SetEase(Ease.InOutElastic);
        });
    }

    public void SnapToGrid(Vector2 hitPos)
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