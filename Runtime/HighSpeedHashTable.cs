/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;*/

namespace GameWorkstore.Patterns
{
    /*[Serializable]
    public class HighSpeedHashTable<T, U>
    {
        private T[] _keyArray;
        private U[] _valueArray;

        public T[] KeysArray { get { return _keyArray; } }
        public U[] ValuesArray { get { return _valueArray; } }
        public ICollection<T> Keys { get { return _keyArray; } }
        public ICollection<U> Values { get { return _valueArray; } }
        public int Count { get; private set; }
        public int Capacity { get { return _keyArray.Length; } }

        public U this[T key]
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

        public HighSpeedHashTable(int capacity)
        {
            SetCapacity(capacity);
        }

        public void SetCapacity(int capacity)
        {
            _keyArray = new T[capacity];
            _valueArray = new U[capacity];
            Count = 0;
        }

        public void Add(T key, U value)
        {
            if (Count >= Capacity)
            {
                Debug.LogWarning("Error: HighSpeedArray reached limit for " + typeof(T) + ". Resizing from " + Capacity + " to " + 2 * Capacity);
                int ncapacity = 2 * Capacity;
                Array.Resize(ref _keyArray, ncapacity);
                Array.Resize(ref _valueArray, ncapacity);
            }
            _keyArray[Count] = key;
            _valueArray[Count] = value;
            Count++;
        }

        public void Add(KeyValuePair<T,U> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public bool ContainsKey(T key)
        {
            for(int i = 0; i < Count; i++)
            {
                if (_keyArray[i].Equals(key)) return true;
            }
            return false;
        }

        public bool Contains(KeyValuePair<T, U> pair)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_keyArray[i].Equals(pair.Key) && _valueArray[i].Equals(pair.Value)) return true;
            }
            return false;
        }

        public void AddRange(HighSpeedHashTable<T, U> range)
        {
            for (int i = 0; i < range.Count; i++)
            {
                Add(range.KeysArray[i], range.ValuesArray[i]);
            }
        }

        public bool TryGetValue(T key, out U value)
        {
            value = default(U);
            for (int i = 0; i < Count; i++)
            {
                if (_keyArray[i].Equals(key))
                {
                    value = _valueArray[i];
                    return true;
                }
            }
            return false;
        }

        public bool Remove(KeyValuePair<T, U> pair)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_keyArray[i].Equals(pair.Key) && _valueArray[i].Equals(pair.Value))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<T, U>[] pair, int amount)
        {
            for(int i=0;i<Count && i < amount; i++)
            {
                pair[i] = new KeyValuePair<T, U>(_keyArray[i], _valueArray[i]);
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
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; i++)
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
                return default(T);
            }
            return this[index];
        }

        public T Find<T1>(Func<T, T1, bool> predicate, T1 param)
        {
            int index = IndexOf(predicate, param);
            if (index < 0)
            {
                return default(T);
            }
            return this[index];
        }

        public void ForEach(Action<T> just_do_it)
        {
            for (int i = 0; i < Count; i++)
            {
                just_do_it(_array[i]);
            }
        }

        public bool Exists(Predicate<T> predicate)
        {
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; )
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
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; i++)
            {
                for (int j = i + 1; j < Count; j++)
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
            Count = 0;
        }

        public void RemoveAt(int index)
        {
            if (index >= Count)
            {
                return;
            }
            for (int i = index; i + 1 < Count; i++)
            {
                _keyArray[i] = _keyArray[i + 1];
                _valueArray[i] = _valueArray[i + 1];
            }
            Count--;
        }

        public bool Remove(T entry)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Equals(_array[i], entry))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAll(T entry)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Equals(_array[i], entry))
                {
                    RemoveAt(i);
                }
            }
        }

        public bool Any(Predicate<T> predicate)
        {
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count / 2; i++)
            {
                T info = _array[i];
                _array[i] = _array[Count - i - 1];
                _array[Count - i - 1] = info;
            }
        }

        public int Sum(Func<T, int> function)
        {
            int value = 0;
            for (int i = 0; i < Count; i++)
            {
                value += function(_array[i]);
            }
            return value;
        }

        public void Do(Action<T> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(_array[i]);
            }
        }

        public void Insert(int index, T item)
        {
            if (index > Count)
            {
                return;
            }

            if (Count > 0)
            {
                Add(_array[Count - 1]);
                for (int i = index + 1; i < Count; i++)
                {
                    _array[i] = _array[i - 1];
                }
            }

            _array[index] = item;
        }

        public void CopyTo(T[] array, int index)
        {
            int j = index;
            for (int i = 0; i < Count; i++)
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
            return _array.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _array[i];
            }
        }
    }*/
}
