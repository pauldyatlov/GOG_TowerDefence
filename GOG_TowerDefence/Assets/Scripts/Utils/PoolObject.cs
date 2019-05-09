using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class PoolObject : MonoBehaviour, IPoolObject<string>
    {
        public virtual string Group { get { return name; } }
        public Transform MyTransform { get { return myTransform; } }

        protected Transform myTransform;

        protected virtual void Awake()
        {
            myTransform = transform;
        }

        public virtual void SetTransform(Vector3 position, Quaternion rotation)
        {
            myTransform.position = position;
            myTransform.rotation = rotation;
        }

        public virtual void Create()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnPush()
        {
            gameObject.SetActive(false);
        }

        public virtual void Push()
        {
            Pool.Push(Group, this);
        }

        public void FailedPush()
        {
            Debug.Log("Push failed.");

            Destroy(gameObject);
        }
    }
}