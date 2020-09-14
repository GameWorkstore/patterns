using System.Collections.Generic;

namespace GameWorkstore.Patterns
{
    public class ThreadQueue<T>
    {
        /// <summary>Used as a lock target to ensure thread safety.</summary>
        private readonly object _Locker = new object();

        private readonly Queue<T> _Queue = new Queue<T>();

        /// <summary></summary>
        public void Enqueue(T item)
        {
            lock (_Locker)
            {
                _Queue.Enqueue(item);
            }
        }

        /// <summary>Enqueues a collection of items into this queue.</summary>
        public virtual void EnqueueRange(IEnumerable<T> items)
        {
            lock (_Locker)
            {
                if (items == null)
                {
                    return;
                }

                foreach (T item in items)
                {
                    _Queue.Enqueue(item);
                }
            }
        }

        /// <summary></summary>
        public T Dequeue()
        {
            lock (_Locker)
            {
                return _Queue.Dequeue();
            }
        }

        /// <summary></summary>
        public void Clear()
        {
            lock (_Locker)
            {
                _Queue.Clear();
            }
        }

        /// <summary></summary>
        public int Count
        {
            get
            {
                lock (_Locker)
                {
                    return _Queue.Count;
                }
            }
        }

        /// <summary></summary>
        public bool TryDequeue(out T item)
        {
            lock (_Locker)
            {
                if (_Queue.Count > 0)
                {
                    item = _Queue.Dequeue();
                    return true;
                }
                else
                {
                    item = default(T);
                    return false;
                }
            }
        }
    }
}