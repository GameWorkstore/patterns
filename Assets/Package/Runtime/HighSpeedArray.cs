using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    [Serializable]
    public class HighSpeedArray<T> : IList<T>, ICollection
    {
        [SerializeField]
        private T[] _array;
        [SerializeField]
        private int _count;

        public int Count => _count;
        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot { get; } = new object();
        public int Capacity => _array.Length;

        public T this[int key]
        {
            get => _array[key];
            set => _array[key] = value;
        }

        public HighSpeedArray(int capacity)
        {
            _array = new T[capacity];
            _count = 0;
        }

        public HighSpeedArray(IEnumerable<T> list)
        {
            _array = list.ToArray();
            _count = _array.Length;
        }

        /// <summary>
        /// Set capacity, trimming or increasing the internal array. Changes count if capacity is lower.
        /// </summary>
        /// <param name="capacity">new capacity</param>
        public void SetCapacity(int capacity)
        {
            if (_array.Length == capacity) return;
            Array.Resize(ref _array, capacity);
            if (_count > capacity) _count = capacity;
        }

        /// <summary>
        /// trim internal array to have only the current amount of elements.
        /// </summary>
        public void Trim()
        {
            SetCapacity(_count);
        }

        public void Add(T obj)
        {
            if (_count >= Capacity)
            {
                var newCapacity = Mathf.CeilToInt(1.5f * Capacity);
#if UNITY_EDITOR
                Debug.LogWarning("Optimization: HighSpeedArray reached limit for " + typeof(T) + ". Resizing from " + Capacity + " to " + newCapacity + ". Try to allocate a large buffer for avoid this operation.");
#endif
                SetCapacity(newCapacity);
            }
            _array[_count] = obj;
            _count++;
        }

        public void AddRange(HighSpeedArray<T> range)
        {
            for (var i = 0; i < range.Count; i++)
            {
                Add(range[i]);
            }
        }

        public void AddRange(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public int IndexOf(T entry)
        {
            for (var i = 0; i < _count; i++)
            {
                if (Equals(_array[i], entry))
                {
                    return i;
                }
            }
            return -1;
        }

        public int IndexOf(Predicate<T> predicate)
        {
            for (var i = 0; i < _count; i++)
            {
                if (predicate(_array[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public int IndexOf<T1>(Func<T, T1, bool> predicate, T1 param)
        {
            for (var i = 0; i < _count; i++)
            {
                if (predicate(_array[i], param))
                {
                    return i;
                }
            }
            return -1;
        }

        public T Find(Predicate<T> predicate)
        {
            var index = IndexOf(predicate);
            return index < 0 ? default : this[index];
        }

        public T Find<T1>(Func<T, T1, bool> predicate, T1 param)
        {
            var index = IndexOf(predicate, param);
            return index < 0 ? default : this[index];
        }

        public void ForEach(Action<T> doIt)
        {
            for (var i = 0; i < _count; i++)
            {
                doIt(_array[i]);
            }
        }

        public bool Exists(Predicate<T> predicate)
        {
            for (var i = 0; i < _count; i++)
            {
                if (predicate(_array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T data)
        {
            for (var i = 0; i < _count; i++)
            {
                if (_array[i].Equals(data))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            for (var i = 0; i < _count;)
            {
                if (predicate(_array[i]))
                {
                    RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void Remove(Predicate<T> predicate)
        {
            for (var i = 0; i < _count; i++)
            {
                if (!predicate(_array[i])) continue;
                RemoveAt(i);
                return;
            }
        }

        public void Remove<T1>(Func<T, T1, bool> predicate, T1 param)
        {
            for (var i = 0; i < _count; i++)
            {
                if (!predicate(_array[i], param)) continue;
                RemoveAt(i);
                return;
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            for (var i = 0; i < _count; i++)
            {
                for (var j = i + 1; j < _count; j++)
                {
                    if (comparer.Compare(_array[i], _array[j]) >= 0)
                    {
                        continue;
                    }
                    var info = _array[i];
                    _array[i] = _array[j];
                    _array[j] = info;
                }
            }
        }

        public void Clear()
        {
            _count = 0;
        }

        /// <summary>
        /// This function operates as Clean, but also replace array values with defaultValue.
        /// Important to ensure object begin garbage collected. Mostly used with null
        /// with HighSpeedArray<object>. Useless for builtin types or structs.
        /// </summary>
        /// <param name="defaultValue"></param>
        public void Empty(T defaultValue)
        {
            for (var i = 0; i < _count; i++)
            {
                _array[i] = defaultValue;
            }
            _count = 0;
        }

        public void RemoveAt(int index)
        {
            if (index >= _count) return;
            var last = _count - 1;
            _array[index] = _array[last];
            _array[last] = default;
            _count--;
        }

        public bool Remove(T entry)
        {
            for (var i = 0; i < _count; i++)
            {
                if (!Equals(_array[i], entry)) continue;
                RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function experimental, avoid use it in production.
        /// </summary>
        /// <param name="entry">removes all values equals to this value</param>
        public void RemoveAll(T entry)
        {
            for (var i = 0; i < _count; i++)
            {
                if (Equals(_array[i], entry))
                {
                    RemoveAt(i);
                }
            }
        }

        public bool Any(Predicate<T> predicate)
        {
            for (var i = 0; i < _count; i++)
            {
                if (predicate(_array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public void Revert()
        {
            for (var i = 0; i < _count / 2; i++)
            {
                var info = _array[i];
                _array[i] = _array[_count - i - 1];
                _array[_count - i - 1] = info;
            }
        }

        public int Sum(Func<T, int> function)
        {
            var value = 0;
            for (var i = 0; i < _count; i++)
            {
                value += function(_array[i]);
            }
            return value;
        }

        public void Do(Action<T> action)
        {
            for (var i = 0; i < _count; i++)
            {
                action(_array[i]);
            }
        }

        /// <summary>
        /// Function experimental, avoid use it in production.
        /// </summary>
        /// <param name="index">insertion point.</param>
        /// <param name="item">inserting value</param>
        public void Insert(int index, T item)
        {
            if (index > _count)
            {
                return;
            }

            if (_count > 0)
            {
                Add(_array[_count - 1]);
                for (var i = index + 1; i < _count; i++)
                {
                    _array[i] = _array[i - 1];
                }
            }

            _array[index] = item;
        }

        public T Random()
        {
            return _count <= 0 ? default : _array[UnityEngine.Random.Range(0, _array.Length)];
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
            for (var i = 0; i < _count; i++)
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
            for (var i = 0; i < _count; i++)
            {
                yield return _array[i];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return _array[i];
            }
        }
    }
}
