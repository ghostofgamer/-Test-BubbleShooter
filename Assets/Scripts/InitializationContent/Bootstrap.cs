using BallContent;
using Cysharp.Threading.Tasks;
using ScoreContent;
using SpawnContent;
using UI.Screens;
using UnityEngine;

namespace InitializationContent
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayFieldSpawner _playFieldSpawner;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private BallPool _ballPool;
        [SerializeField] private BallQueue _ballQueue;
        [SerializeField] private LauncherVisualizer _launcherVisualizer;
        [SerializeField] private LauncherPoint _launcherPoint;
        [SerializeField] private GameManager _gameManager;
        [SerializeField]private BallLauncher _ballLauncher;
        [SerializeField]private ScoreViewer _scoreViewer;
        [SerializeField]private EndGameScreen _endGameScreen;

        private ScreenData _screenData;

        private void Start()
        {
            Initialization();
        }

        private async UniTask Initialization()
        {
            _screenData = new ScreenData(_camera);
            _scoreViewer.Init();
            await _playFieldSpawner.Init(_screenData.Height, _screenData.Width);
            _gridManager.CalculateRadius(_screenData.Width, _screenData.Height);
            await _ballPool.Init(_gridManager.Radius);
            await _gridManager.Init(_screenData, _ballPool);
            _launcherVisualizer.Init(_screenData);
            _launcherPoint.Init(_launcherVisualizer, _screenData);
            _ballQueue.SpawnNextBall();
            
            await _gameManager.Init(_screenData, _gridManager, _ballPool);
            _gameManager.ConnectLauncher(_launcherPoint);
            _ballLauncher.Initialize(15);
            _ballLauncher.SpawnCurrentBall();
            
            _gameManager.Init(_ballLauncher,_screenData);
            _ballLauncher.AllBallUsed += OnAllBallsUsed;
            _endGameScreen.Init(_ballLauncher);
        }
        
        private void OnAllBallsUsed()
        {
            Debug.Log("Game Over - All balls used");
        }
    }
}