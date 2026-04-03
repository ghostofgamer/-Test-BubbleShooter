using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnContent
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private Transform _container;
        private T _prefab;
        private List<T> _poolGeneric;

        public T Prefab => _prefab;

        public ObjectPool(T prefab, int count, Transform container)
        {
            _prefab = prefab;
            _container = container;
            GetInitialization(count, prefab);
        }

        public bool AutoExpand { get; private set; }

        public bool TryGetObject(out T spawned, T prefabs)
        {
            var inactive = _poolGeneric
                .Where(obj => !obj.gameObject.activeSelf)
                .ToList();

            if (inactive.Count == 0)
            {
                if (AutoExpand)
                {
                    spawned = CreateObject(prefabs);
                    return true;
                }

                spawned = null;
                return false;
            }

            int index = Random.Range(0, inactive.Count);
            spawned = inactive[index];
            spawned.gameObject.SetActive(true);

            return true;
        }

        public void SetAutoExpand(bool flag)
        {
            AutoExpand = flag;
        }

        public void Reset()
        {
            foreach (var item in _poolGeneric)
                item.gameObject.SetActive(false);
        }

        private void GetInitialization(int count, T prefabs)
        {
            _poolGeneric = new List<T>();

            for (int i = 0; i < count; i++)
            {
                var spawned = Object.Instantiate(prefabs, _container.transform);
                spawned.gameObject.SetActive(false);
                _poolGeneric.Add(spawned);
            }
        }

        private T CreateObject(T prefabs)
        {
            var spawned = Object.Instantiate(prefabs, _container.transform);
            spawned.gameObject.SetActive(true);
            _poolGeneric.Add(spawned);
            return spawned;
        }
    }
}