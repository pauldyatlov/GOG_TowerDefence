using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public interface IPoolObject<T>
    {
        T Group { get; }
        void Create();
        void OnPush();
        void FailedPush();
    }

    public sealed class PoolManager<K, V> where V : IPoolObject<K>
    {
        public int MaxInstances { get; private set; }
        public int InctanceCount { get { return _objects.Count; } }
        public int CacheCount { get { return _cache.Count; } }

        public delegate bool Compare<T>(T value) where T : V;

        private readonly Dictionary<K, List<V>> _objects;
        private readonly Dictionary<Type, List<V>> _cache;

        public PoolManager(int maxInstance)
        {
            MaxInstances = maxInstance;
            _objects = new Dictionary<K, List<V>>();
            _cache = new Dictionary<Type, List<V>>();
        }

        public bool CanPush()
        {
            return InctanceCount + 1 < MaxInstances;
        }

        public bool Push(K groupKey, V value)
        {
            if (CanPush())
            {
                value.OnPush();

                if (!_objects.ContainsKey(groupKey)) {
                    _objects.Add(groupKey, new List<V>());
                }

                _objects[groupKey].Add(value);
                var type = value.GetType();

                if (!_cache.ContainsKey(type)) {
                    _cache.Add(type, new List<V>());
                }

                _cache[type].Add(value);

                return true;
            }

            value.FailedPush();

            return false;
        }

        public T Pop<T>(K groupKey) where T : V
        {
            var result = default(T);
            if (Contains(groupKey) && _objects[groupKey].Count > 0)
            {
                for (var i = 0; i < _objects[groupKey].Count; i++)
                {
                    if (_objects[groupKey][i] is T)
                    {
                        result = (T)_objects[groupKey][i];
                        var type = result.GetType();

                        RemoveObject(groupKey, i);
                        RemoveFromCache(result, type);
                        result.Create();

                        break;
                    }
                }
            }
            return result;
        }

        public T Pop<T>() where T : V
        {
            var result = default(T);
            var type = typeof(T);
            if (ValidateForPop(type))
            {
                for (var i = 0; i < _cache[type].Count; i++)
                {
                    result = (T)_cache[type][i];

                    if (result != null && _objects.ContainsKey(result.Group))
                    {
                        _objects[result.Group].Remove(result);
                        RemoveFromCache(result, type);
                        result.Create();
                        break;
                    }
                }
            }
            return result;
        }

        public T Pop<T>(Compare<T> comparer) where T : V
        {
            var result = default(T);
            var type = typeof(T);
            if (ValidateForPop(type))
            {
                for (var i = 0; i < _cache[type].Count; i++)
                {
                    var value = (T)_cache[type][i];
                    if (comparer(value))
                    {
                        _objects[value.Group].Remove(value);
                        RemoveFromCache(result, type);

                        result = value;
                        result.Create();

                        break;
                    }

                }
            }

            return result;
        }
        
        public bool Contains(K groupKey)
        {
            return _objects.ContainsKey(groupKey);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        private bool ValidateForPop(Type type)
        {
            return _cache.ContainsKey(type) && _cache[type].Count > 0;
        }

        private void RemoveObject(K groupKey, int idx)
        {
            if (idx >= 0 && idx < _objects[groupKey].Count)
            {
                _objects[groupKey].RemoveAt(idx);
                if (_objects[groupKey].Count == 0)
                {
                    _objects.Remove(groupKey);
                }
            }
        }

        private void RemoveFromCache(V value, Type type)
        {
            if (_cache.ContainsKey(type))
            {
                _cache[type].Remove(value);
                if (_cache[type].Count == 0)
                {
                    _cache.Remove(type);
                }
            }
        }
    }
}