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
        public int Available => _pool.Count;
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

        /// <summary>
        /// Instantiate and Adds object to reference list.
        /// </summary>
        /// <returns></returns>
        public T Instantiate()
        {
            var item = FastInstantiate();
            _array.Add(item);
            return item;
        }

        /// <summary>
        /// Instantiate but don't add it reference list.
        /// Fast, but reference will be out of sync.
        /// Use FastDispose to return it to pool.
        /// </summary>
        /// <returns></returns>
        public T FastInstantiate()
        {
            if (!_pool.TryDequeue(out var item))
            {
                item = UnityEngine.Object.Instantiate(Template, Parent, WorldPositionStaysWhenInstantiating);
            }
            Use(item);
            return item;
        }

        public void Use(T item)
        {
            if (item.gameObject.activeSelf) return;
            item.gameObject.SetActive(true);
        }

        /// <summary>
        /// Return item to pool. Item can be used on future. Slow, but safe.
        /// </summary>
        public void Dispose(T item)
        {
            if (!_array.Remove(item))
            {
                Debug.LogError("Item not created by this pool.");
                return;
            }
            FastDispose(item);
        }

        /// <summary>
        /// Return item to pool. Item can be used on future. Fast, but reference will be out of sync.
        /// Fast, but reference will be out of sync.
        /// Use FastDispose to return it to pool.
        /// </summary>
        /// <param name="item">Returned instance at Instantiate();</param>
        /// <param name="index">Index returned at Instantiate()</param>
        public void FastDispose(T item)
        {
            _pool.Enqueue(item);
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