using System;
using System.Linq;
using InitializationContent;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage(AttributeTargets.Method)]
public class EditorButtonAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class EditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mono = target as MonoBehaviour;

        var methods = mono.GetType()
            .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(EditorButtonAttribute), true).Length > 0);

        foreach (var method in methods)
        {
            if (GUILayout.Button(method.Name))
            {
                method.Invoke(mono, null);
            }
        }
    }
}
#endif

public class LauncherPoint : MonoBehaviour
{
    [Header("Launcher Settings")]
    [SerializeField] private float maxDragDistance = 2f;
    [SerializeField] private float forceMultiplier = 10f;
    [SerializeField] private float minForceMultiplier = 5f;
    [SerializeField] private float gravity = 9.8f;

    [SerializeField] private BallPool _ballPool;
    [SerializeField] private Transform ballHolder;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private TMP_Text _powerText;

    private Ball _currentBall;
    private BallMover _ballMover;
    private Vector3 _startPosition;
    private Vector3 _dragVector;
    private bool _isDragging = false;

    [SerializeField] private float _minAngle = 45f;
    [SerializeField] private float _maxAngle = 135f;

    private LauncherVisualizer _visualizer;
    private ScreenData _screenData;

    private bool _isFlying = false;

    public event Action<Ball, Vector2Int> OnBallPlaced;
    public event Action<Ball, Vector2Int> OnBallShotThrough;

    public void Init(LauncherVisualizer visualizer, ScreenData screenData)
    {
        _visualizer = visualizer;
        _startPosition = transform.position;
        _screenData = screenData;
    }

    [EditorButton]
    public void GetNewBallTest()
    {
        SetBall(_ballPool.Get());
    }

    public void SetBall(Ball ball)
    {
        _currentBall = ball;
        _currentBall.transform.position = _startPosition;
        _currentBall.transform.SetParent(ballHolder, true);
        
        _ballMover = _currentBall.GetComponent<BallMover>();
        _ballMover.Init(_screenData, _gridManager);
        _ballMover.gravity = gravity;
        _ballMover.Stop();
        
        _ballMover.OnBallPlaced += OnBallPlacedHandler;
        _ballMover.OnBallShotThrough += OnBallShotThroughHandler;
        
        _isFlying = false;
        _isDragging = true;
        _visualizer?.Show();
    }

    private void OnBallPlacedHandler(Ball ball, Vector2Int cell)
    {
        if (_ballMover != null)
        {
            _ballMover.OnBallPlaced -= OnBallPlacedHandler;
            _ballMover.OnBallShotThrough -= OnBallShotThroughHandler;
        }
        _isFlying = false;
        _isDragging = false;
        OnBallPlaced?.Invoke(ball, cell);
    }

    private void OnBallShotThroughHandler(Ball ball, Vector2Int cell)
    {
        if (_ballMover != null)
        {
            _ballMover.OnBallPlaced -= OnBallPlacedHandler;
            _ballMover.OnBallShotThrough -= OnBallShotThroughHandler;
        }
        _isFlying = false;
        _isDragging = false;
        OnBallShotThrough?.Invoke(ball, cell);
    }

    private void Update()
    {
        if (_isDragging && _currentBall != null && !_isFlying)
            DragBall();

        if (_isDragging && Input.GetMouseButtonUp(0))
            LaunchBall();
    }

    private void DragBall()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z))
        );
        mousePos.z = 0f;

        _dragVector = mousePos - _startPosition;

        if (_dragVector.magnitude > maxDragDistance)
            _dragVector = _dragVector.normalized * maxDragDistance;

        Vector3 launchVector = -_dragVector;

        if (launchVector.y <= 0)
        {
            launchVector.y = 0.01f;
            launchVector = launchVector.normalized * _dragVector.magnitude;
            _dragVector = -launchVector;
        }

        float angle = Mathf.Atan2(launchVector.y, launchVector.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, _minAngle, _maxAngle);

        float length = launchVector.magnitude;
        launchVector.x = Mathf.Cos(angle * Mathf.Deg2Rad) * length;
        launchVector.y = Mathf.Sin(angle * Mathf.Deg2Rad) * length;

        _dragVector = -launchVector;
        _currentBall.transform.position = _startPosition + _dragVector;

        float pullAmount = _dragVector.magnitude;
        float pullRatio = pullAmount / maxDragDistance;
        
        if (_powerText != null)
        {
            int percent = Mathf.RoundToInt(pullRatio * 100);
            _powerText.text = $"{percent}%";
        }
        
        float currentForce = Mathf.Lerp(minForceMultiplier * pullAmount, forceMultiplier * pullAmount, pullRatio);

        ShotType shotType = ShotType.Normal;
        float spread = 0f;

        if (pullRatio >= 0.95f)
        {
            shotType = ShotType.PowerShot;
            spread = 15f * pullRatio;
        }

        _visualizer?.UpdateTrajectory(
            _currentBall.transform.position,
            launchVector.normalized,
            currentForce,
            _currentBall.Radius,
            spread,
            shotType == ShotType.PowerShot
        );
    }

    private void LaunchBall()
    {
        _isDragging = false;
        _isFlying = true;
        
        if (_powerText != null)
            _powerText.text = "";
            
        _currentBall.transform.SetParent(null, true);

        float pullAmount = _dragVector.magnitude;
        float pullRatio = pullAmount / maxDragDistance;
        
        float launchForce = Mathf.Max(forceMultiplier * pullAmount, 20f);
        Vector3 launchVelocity = -_dragVector.normalized * launchForce;

        ShotType shotType = (pullRatio >= 0.95f) ? ShotType.PowerShot : ShotType.Normal;

        _ballMover.Launch(launchVelocity, shotType);
        _visualizer?.Hide();
    }

    public void SetFlyingState(bool isFlying)
    {
        _isFlying = isFlying;
    }
}