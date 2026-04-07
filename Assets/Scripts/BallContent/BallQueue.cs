using System;
using SpawnContent;
using UnityEngine;

public class BallQueue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private BallPool _ballPool;
    
    [Header("Parameters")]
    [SerializeField] private float _radius = 0.5f;
    
    private Ball _currentBall;

    public void SpawnNextBall()
    {/*
        if (_ballPool == null || _spawnPoint == null)
        {
            Debug.LogWarning("BallPool or SpawnPoint not assigned!");
            return;
        }
        
        _currentBall = _ballPool.Get();
        _currentBall.transform.position = _spawnPoint.position;
        _currentBall.gameObject.SetActive(true);*/
    }
    
    public Ball GetCurrentBall()
    {
        return _currentBall;
    }
}