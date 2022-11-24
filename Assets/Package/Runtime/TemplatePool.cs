using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    [Serializable]
    public class TemplatePool<T> : ICollection where T : Component
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "false positive")]
        public Transform Parent;
        public T Template;
        public bool WorldPositionStaysWhenInstantiating = false;

        private HighSpeedArray<T> _array = new HighSpeedArray<T>(16);
        private Queue<T> _pool = new Queue<T>();

        public T this[int key]
        {
            get => _array[key];
            set => _array[key] = value;
        }

        public int Count => _array.Count;
        public bool IsSynchronized => false;
        public object SyncRoot { get; } = new object();

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
            return Instantiate(out _);
        }

        public T Instantiate(out int index)
        {
            var item = _pool.Dequeue();
            if (item == null)
            {
                item = UnityEngine.Object.Instantiate(Template, Parent, WorldPositionStaysWhenInstantiating);
            }
            Use(item);
            index = _array.Count;
            _array.Add(item);
            return item;
        }

        public void Use(T item)
        {
            if (item.gameObject.activeSelf) return;
            item.gameObject.SetActive(true);
        }

        public void Dispose(T item)
        {
            if (!_array.Remove(item))
            {
                Debug.LogError("Item not created by this pool.");
                return;
            }
            if (item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(false);
            }
        }

        public static T1 AddRequiredComponent<T1>(GameObject target) where T1 : Component
        {
            return target.GetComponent<T1>() ?? target.AddComponent<T1>();
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("The array cannot be null.");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            }
            if (Count > array.Length - index + 1)
            {
                throw new ArgumentException("The destination array has fewer elements than the collection.");
            }
            for (var i = 0; i < Count; i++)
            {
                array[i + index] = _array[i];
            }
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        public IEnumerator GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return _array[i];
            }
        }
    }

    [Serializable]
    public class UnityComponentPool : TemplatePool<Component> { }
}