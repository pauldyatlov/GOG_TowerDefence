using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Pool
    {
        public const int MaxInstanceCount = 64;

        private static PoolManager<string, PoolObject> _poolManager;
        public static PoolManager<string, PoolObject> PoolManager
        {
            get { return _poolManager ?? (_poolManager = new PoolManager<string, PoolObject>(MaxInstanceCount)); }
        }
    
        public static bool CanPush()
        {
            return PoolManager.CanPush();
        }

        public static bool Push(string groupKey, PoolObject poolObject)
        {
            return PoolManager.Push(groupKey, poolObject);
        }

        public static T PopOrCreate<T>(T prefab) where T : PoolObject
        {
            return PopOrCreate(prefab, Vector3.zero, Quaternion.identity);
        }

        public static T PopOrCreate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : PoolObject
        {
            var result = PoolManager.Pop<T>(prefab.Group);

            if (result == null)
            {
                result = CreateObject(prefab, position, rotation, parent);
            }
            else
            {
                result.SetTransform(position, rotation);
            }

            return result;
        }

        public static PoolObject Pop(string groupKey)
        {
            return PoolManager.Pop<PoolObject>(groupKey);
        }

        public static T Pop<T>() where T : PoolObject
        {
            return PoolManager.Pop<T>();
        }

        public static T Pop<T>(PoolManager<string, PoolObject>.Compare<T> comparer) where T : PoolObject
        {
            return PoolManager.Pop<T>(comparer);
        }

        public static T Pop<T>(string groupKey) where T : PoolObject
        {
            return PoolManager.Pop<T>(groupKey);
        }

        public static bool Contains(string groupKey)
        {
            return PoolManager.Contains(groupKey);
        }

        public static void Clear()
        {
            PoolManager.Clear();
        }

        public static T CreateObject<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : PoolObject
        {
            var go = Object.Instantiate(prefab.gameObject, position, rotation, parent);
            var result = go.GetComponent<T>();

            result.name = prefab.name;

            return result;
        }
    }
}