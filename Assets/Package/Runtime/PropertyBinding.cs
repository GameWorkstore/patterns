using System;

namespace GameWorkstore.Patterns
{
    public struct PropertyBinding<T>
    {
        private bool _setup;
        public T Value { get; private set; }
        private Signal<T> _onChange;

        public void Set(T valueT)
        {
            if (valueT.Equals(Value) && _setup) return;

            _setup = true;
            Value = valueT;
            _onChange.Invoke(valueT);
        }

        public void Register(Action<T> e)
        {
            _onChange.Register(e);
            e?.Invoke(Value);
        }

        public void Unregister(Action<T> e)
        {
            _onChange.Unregister(e);
        }

        public void Empty()
        {
            _onChange.Empty();
        }
    }
}