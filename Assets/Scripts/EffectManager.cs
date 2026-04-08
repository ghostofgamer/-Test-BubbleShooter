using System.Collections.Generic;
using BallContent;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private BallExplosionEffect _effectPrefab;
    [SerializeField] private int _poolSize = 10;

    private List<BallExplosionEffect> _pool;

    private void Awake()
    {
        Instance = this;
        CreatePool();
    }

    private void CreatePool()
    {
        _pool = new List<BallExplosionEffect>();

        for (int i = 0; i < _poolSize; i++)
        {
            BallExplosionEffect effect = Instantiate(_effectPrefab, transform);
            effect.gameObject.SetActive(false);
            _pool.Add(effect);
        }
    }

    public void PlayEffect(Vector3 position, Color ballColor)
    {
        BallExplosionEffect effect = GetFreeEffect();
        
        if (effect == null)
            return;

        effect.Play(position, ballColor);
    }

    private BallExplosionEffect GetFreeEffect()
    {
        foreach (var effect in _pool)
        {
            if (!effect.gameObject.activeSelf)
                return effect;
        }

        return null;
    }
}