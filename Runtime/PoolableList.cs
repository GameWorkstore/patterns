using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    public class PoolableMonoBehaviour : MonoBehaviour
    {
        internal bool IsUsing;
    }

    [Serializable]
    public class PoolableTemplateList<T> : List<T> where T : Component
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "false positive")]
        public Transform Parent;
        public T Template;
        public bool WorldPositionStaysWhenInstantiating = false;

        public void SetActiveCount(int desiredCount)
        {
            if (desiredCount < 0)
            {
                throw new ArgumentException(string.Format("{0} is not a valid value for desiredCount", desiredCount), "desiredCount");
            }

            for (int i = 0; i < desiredCount && i < Count; i++)
            {
                Use(this[i]);
            }
            while (Count < desiredCount)
            {
                Instantiate();
            }
            for (int i = desiredCount; i < Count; i++)
            {
                Dispose(this[i]);
            }
        }

        public T Instantiate()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].GetComponent<PoolableMonoBehaviour>().IsUsing)
                {
                    continue;
                }
                Use(this[i]);
                return this[i];
            }
            var newItem = UnityEngine.Object.Instantiate(Template, Parent, WorldPositionStaysWhenInstantiating);
            newItem.gameObject.SetActive(true);
            AddRequiredComponent<PoolableMonoBehaviour>(newItem.gameObject);
            Use(newItem);
            Add(newItem);
            return newItem;
        }

        public virtual void Use(T item)
        {
            item.GetComponent<PoolableMonoBehaviour>().IsUsing = true;
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
            }
        }

        public virtual void Dispose(T item)
        {
            item.GetComponent<PoolableMonoBehaviour>().IsUsing = false;
            if (item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(false);
            }
        }

        public static T1 AddRequiredComponent<T1>(GameObject target) where T1 : Component
        {
            return target.GetComponent<T1>() ?? target.AddComponent<T1>();
        }
    }

    [Serializable]
    public class PoolableList : PoolableTemplateList<Component> { }
}