using UnityEngine;

namespace BallContent
{
    public class BallExplosionEffect : MonoBehaviour
    {
        [SerializeField]private ParticleSystem[] _particleSystems;
        
        private bool _isPlaying = false;

        public void SetColor(Color color)
        {
            foreach (var ps in _particleSystems)
            {
                var colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.enabled = true;
            
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
                );
                colorOverLifetime.color = grad;
            }
        }

        public void Play(Vector3 position, Color color)
        {
            transform.position = position;
            SetColor(color);
            gameObject.SetActive(true);
            _isPlaying = true;
        
            foreach (var ps in _particleSystems)
            {
                ps.Play();
            }
        }

        private void Update()
        {
            if (_isPlaying)
            {
                bool allStopped = true;
                foreach (var ps in _particleSystems)
                {
                    if (ps.isPlaying) allStopped = false;
                }
            
                if (allStopped)
                {
                    _isPlaying = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
