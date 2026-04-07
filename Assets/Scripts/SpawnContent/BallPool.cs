using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SpawnContent
{
    public class BallPool : MonoBehaviour
    {
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private int _initialCount = 50;
        [SerializeField] private SOContent.BallColorConfig _ballColorConfig;
    
        private List<Ball> _pool;
        private float _ballSpriteSize;
        private float _radius;
        private string[] _ballTypes = { "R", "G", "B","Y"};

        public async UniTask Init(float radius)
        {
            _radius = radius;
        
            if (_ballPrefab == null)
            {
                Debug.LogError("BallPrefab is not assigned!");
                return;
            }
        
            _ballSpriteSize = _ballPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        
            _pool = new List<Ball>();
        
            for (int i = 0; i < _initialCount; i++)
            {
                CreateBall();
            }
        }

        public void SetBallColorConfig(SOContent.BallColorConfig config)
        {
            _ballColorConfig = config;
        }

        private Ball CreateBall()
        {
            Ball ball = Instantiate(_ballPrefab, _container);

            float scale = (_radius * 2f) / _ballSpriteSize;
            ball.transform.localScale = Vector3.one * scale;
            ball.InitRadius(_radius);
        
            if (_ballColorConfig != null)
            {
                ball.SetBallColorConfig(_ballColorConfig);
            }
        
            string randomType = _ballTypes[Random.Range(0, _ballTypes.Length)];
            ball.Init(randomType);
        
            ball.gameObject.SetActive(false);
        
            for (int i = _pool.Count - 1; i >= 0; i--)
            {
                if (_pool[i] == null) _pool.RemoveAt(i);
            }
        
            _pool.Add(ball);
            return ball;
        }

        public Ball Get()
        {
            for (int i = _pool.Count - 1; i >= 0; i--)
            {
                if (_pool[i] == null)
                {
                    _pool.RemoveAt(i);
                    continue;
                }
            
                if (!_pool[i].gameObject.activeSelf)
                {
                    _pool[i].gameObject.SetActive(true);
                    string randomType = _ballTypes[Random.Range(0, _ballTypes.Length)];
                    _pool[i].Init(randomType);
                    return _pool[i];
                }
            }
        
            return CreateBall();
        }

        public void Release(Ball ball)
        {
            ball.gameObject.SetActive(false);
        }
    }
}