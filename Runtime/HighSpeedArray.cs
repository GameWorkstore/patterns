using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameWorkstore.Patterns
{
    [Serializable]
    public class HighSpeedArray<T> : IList<T>, IEnumerable<T>
    {
        [SerializeField]
        private T[] _array = Array.Empty<T>();
        [SerializeField]
        private int _count = 0;

        public int Count { get { return _count; } }
        public int Capacity { get { return _array.Length; } }
        
        public T this[int key]
        {
            get
            {
                return _array[key];
            }
            set
            {
                _array[key] = value;
            }
        }

        public HighSpeedArray(int capacity)
        {
            _array = new T[capacity];
            _count = 0;
        }

        public HighSpeedArray(T[] array)
        {
            _array = array.ToArray();
            _count = _array.Length;
        }

        public HighSpeedArray(IList<T> list)
        {
            _array = new T[list.Count];
            _count = list.Count;
            for (int i = 0; i < Count; i++)
                _array[i] = list[i];
        }

        public void SetCapacity(int capacity)
        {
            if (_array.Length >= capacity) return;
            Array.Resize(ref _array, capacity);
        }

        public void Add(T obj)
        {
            if (_count >= Capacity)
            {
                int newCapacity = Mathf.CeilToInt(1.5f * Capacity);
#if UNITY_EDITOR
                Debug.LogWarning("Optimization: HighSpeedArray reached limit for " + typeof(T) + ". Resizing from " + Capacity + " to " + newCapacity + ". Try to allocate a large buffer for avoid this.");
#endif
                SetCapacity(newCapacity);
            }
            _array[_count] = obj;
            _count++;
        }

        public void AddRange(HighSpeedArray<T> range)
        {
            for (int i = 0; i < range.Count; i++)
            {
                Add(range[i]);
            }
        }

        public void AddRange(IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                Add(item);
            }
        }

        public int IndexOf(T entry)
        {
            for (int i = 0; i < _count; i++)
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
            for (int i = 0; i < _count; i++)
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
            for (int i = 0; i < _count; i++)
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
            int index = IndexOf(predicate);
            if (index < 0)
            {
                return default;
            }
            return this[index];
        }

        public T Find<T1>(Func<T, T1, bool> predicate, T1 param)
        {
            int index = IndexOf(predicate, param);
            if (index < 0)
            {
                return default;
            }
            return this[index];
        }

        public void ForEach(Action<T> doIt)
        {
            for (int i = 0; i < _count; i++)
            {
                doIt(_array[i]);
            }
        }

        public bool Exists(Predicate<T> predicate)
        {
            for (int i = 0; i < _count; i++)
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
            for (int i = 0; i < _count; i++)
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
            for (int i = 0; i < _count;)
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
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_array[i]))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void Remove<T1>(Func<T, T1, bool> predicate, T1 param)
        {
            for (int i = 0; i < _count; i++)
            {
                if (predicate(_array[i], param))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            for (int i = 0; i < _count; i++)
            {
                for (int j = i + 1; j < _count; j++)
                {
                    if (comparer.Compare(_array[i], _array[j]) >= 0)
                    {
                        continue;
                    }
                    T info = _array[i];
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
            for(int i = 0; i < _count; i++)
            {
                _array[i] = defaultValue;
            }
            _count = 0;
        }

        public void RemoveAt(int index)
        {
            if (index >= _count)
            {
                return;
            }
            for (int i = index; i + 1 < _count; i++)
            {
                _array[i] = _array[i + 1];
            }
            _count--;
        }

        public bool Remove(T entry)
        {
            for (int i = 0; i < _count; i++)
            {
                if (Equals(_array[i], entry))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Function experimental, avoid use it in production.
        /// </summary>
        /// <param name="entry">removes all values equals to this value</param>
        public void RemoveAll(T entry)
        {
            for (int i = 0; i < _count; i++)
            {
                if (Equals(_array[i], entry))
                {
                    RemoveAt(i);
                }
            }
        }

        public bool Any(Predicate<T> predicate)
        {
            for (int i = 0; i < _count; i++)
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
            for (int i = 0; i < _count / 2; i++)
            {
                T info = _array[i];
                _array[i] = _array[_count - i - 1];
                _array[_count - i - 1] = info;
            }
        }

        public int Sum(Func<T, int> function)
        {
            int value = 0;
            for (int i = 0; i < _count; i++)
            {
                value += function(_array[i]);
            }
            return value;
        }

        public void Do(Action<T> action)
        {
            for (int i = 0; i < _count; i++)
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
                for (int i = index + 1; i < _count; i++)
                {
                    _array[i] = _array[i - 1];
                }
            }

            _array[index] = item;
        }

        public T Random()
        {
            if (_count <= 0) return default;
            return _array[UnityEngine.Random.Range(0, _array.Length)];
        }

        public void CopyTo(T[] array, int index)
        {
            int j = index;
            for (int i = 0; i < _count; i++)
            {
                array.SetValue(_array[i], j);
                j++;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _array[i];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _array[i];
            }
        }
    }
}
