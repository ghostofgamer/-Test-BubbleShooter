using System;
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
        [SerializeField] private int _maxBalls;

        private int _ballsUsed = 0;
        private Ball _currentBall;
        private Ball _nextBall;

        public event Action AllBallUsed;

        public bool CanShoot => _ballsUsed < _maxBalls;
        public int RemainingBalls => _maxBalls - _ballsUsed;

        public void Initialize()
        {
            _nextBall = _ballPool.Get();
            UpdateNextPreview();
        }

        public void SpawnCurrentBall()
        {
            if (!CanShoot)
            {
                AllBallUsed?.Invoke();
                Debug.Log("AllBallUsed");
                return;
            }

            _currentBall = _nextBall;
            _nextBall = _ballPool.Get();

            _launcherPoint.SetBall(_currentBall);
            _ballsUsed++;

            UpdateNextPreview();
        }

        private void UpdateNextPreview()
        {
            if (_nextBall != null)
            {
                _nextBallImage.color = _nextBall.GetComponent<SpriteRenderer>().color;
            }
            _nextBallText.text = RemainingBalls.ToString();
        }
    }
}