using SOContent;
using UnityEngine;

namespace BallContent
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sr;
        [SerializeField] private BallColorConfig _ballColorConfig;
    
        private string _ballType;
        private float _radius;

        public float Radius => _radius;
        public string GetBallType() => _ballType;

        public void SetBallColorConfig(BallColorConfig config)
        {
            _ballColorConfig = config;
        }
    
        public void Init(string type)
        {
            _ballType = type;
        
            if (_sr != null && _ballColorConfig != null)
                _sr.color = _ballColorConfig.GetColor(type);
        }
    
        public void InitRadius(float radius)
        {
            _radius = radius;
        }
    }
}