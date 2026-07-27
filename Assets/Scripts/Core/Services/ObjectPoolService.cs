using System;
using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Services
{
    public class ObjectPoolService : IObjectPoolService
    {
        private readonly Dictionary<Type, Queue<Component>> _pools = new();
        private readonly Dictionary<Type, Component> _prefabByType = new();

        public void Register<T>(T prefab, int prewarmCount = 0) where T : Component
        {
            var type = typeof(T);
            _prefabByType[type] = prefab;

            if (!_pools.ContainsKey(type))
                _pools[type] = new Queue<Component>();

            for (var i = 0; i < prewarmCount; i++)
            {
                var instance = Object.Instantiate(prefab);
                instance.gameObject.SetActive(false);
                _pools[type].Enqueue(instance);
            }
        }

        public T Get<T>(Transform parent = null) where T : Component
        {
            var type = typeof(T);

            if (!_prefabByType.TryGetValue(type, out var prefab))
                throw new InvalidOperationException(
                    $"No prefab registered for {type.Name}. Call Register<T>() before Get<T>().");

            var queue = _pools[type];
            T instance;

            if (queue.Count == 0)
            {
                instance = Object.Instantiate((T)prefab, parent);
            }
            else
            {
                instance = (T)queue.Dequeue();
                instance.transform.SetParent(parent, false);
                instance.gameObject.SetActive(true);
            }

            (instance as IPoolable)?.OnSpawned();
            return instance;
        }

        public void Return<T>(T instance) where T : Component
        {
            (instance as IPoolable)?.OnReturned();
            instance.gameObject.SetActive(false);
            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var queue))
            {
                Debug.LogError($"No pool registered for {type.Name}, destroying instance");
                Object.Destroy(instance.gameObject);
                return;
            }
            
            _pools[typeof(T)].Enqueue(instance);
        }

        public void Clear<T>() where T : Component
        {
            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var queue)) return;

            while (queue.Count > 0)
                Object.Destroy(queue.Dequeue().gameObject);

            _pools.Remove(type);
            _prefabByType.Remove(type);
        }

        public void Clear<T>(T prefab) where T : Component
            => Clear<T>();
    }
}