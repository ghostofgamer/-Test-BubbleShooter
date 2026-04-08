using System;
using SpawnContent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallContent
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField] private BallPool _ballPool;
        [SerializeField] private LauncherPoint _launcherPoint;
        [SerializeField] private Image _nextBallImage;
        [SerializeField] private TMP_Text _nextBallText;
        
         private int _maxBalls;

        private int _ballsUsed = 0;
        private Ball _currentBall;
        private Ball _nextBall;

        public event Action AllBallUsed;

        public bool CanShoot => _ballsUsed < _maxBalls;
        public int RemainingBalls => _maxBalls - _ballsUsed;

        public void Initialize(int maxBalls)
        {
            _maxBalls = maxBalls;
            _ballsUsed = 0;
            _currentBall = _ballPool.Get();
            _nextBall = _ballPool.Get();
            
            UpdateNextPreview();
        }

        public void SpawnCurrentBall()
        {
            if (!CanShoot)
            {
                AllBallUsed?.Invoke();
                return;
            }

            _currentBall = _nextBall;
            _nextBall = null;

            if (_ballsUsed < _maxBalls - 1)
            {
                _nextBall = _ballPool.Get();
            }

            _launcherPoint.SetBall(_currentBall);
            _ballsUsed++;
            UpdateNextPreview();
        }

        private void UpdateNextPreview()
        {
            if (_nextBall != null && _nextBall.gameObject.activeSelf)
                _nextBallImage.color = _nextBall.GetComponent<SpriteRenderer>().color;
            else
                _nextBallImage.color = Color.clear;
            
            _nextBallText.text = RemainingBalls.ToString();
        }
    }
}