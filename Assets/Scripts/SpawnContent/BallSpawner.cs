using System;
using UnityEngine;

namespace SpawnContent
{
    public class BallSpawner : MonoBehaviour
    {
        // @formatter:off           
        [Header("References")]
        [SerializeField] private Ball _ballPrefabs;
        [SerializeField] private Transform _container;
        
        [Header("Parameters")]
        [SerializeField] private float _spawnInterval = 3f;
// @formatter:on

        private ObjectPool<Ball> _pool;

        /*private void Start()
        {
            Init();
        }*/

        public void Init()
        {
            InitPools();
        }

        private void InitPools()
        {
            _pool = new ObjectPool<Ball>(_ballPrefabs, 5, _container);
            _pool.SetAutoExpand(true);
        }
    }
}