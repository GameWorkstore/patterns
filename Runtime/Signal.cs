using System;

namespace Patterns
{
    public struct Signal
    {
        private event Action Action;

        public void Invoke()
        {
            Action?.Invoke();
        }

        public void Register(Action e)
        {
            Action += e;
        }

        public void Unregister(Action e)
        {
            Action -= e;
        }
    }

    public struct Signal<T>
    {
        private event Action<T> Action;

        public void Invoke(T evt)
        {
            Action?.Invoke(evt);
        }

        public void Register(Action<T> e)
        {
            Action += e;
        }

        public void Unregister(Action<T> e)
        {
            Action -= e;
        }
    }

    public struct Signal<T,U>
    {
        private event Action<T, U> Action;

        public void Invoke(T evt, U evu)
        {
            Action?.Invoke(evt, evu);
        }

        public void Register(Action<T, U> e)
        {
            Action += e;
        }

        public void Unregister(Action<T, U> e)
        {
            Action -= e;
        }
    }

    public struct Signal<T, U, V>
    {
        private event Action<T, U, V> Action;

        public void Invoke(T evt, U evu, V evv)
        {
            Action?.Invoke(evt, evu, evv);
        }

        public void Register(Action<T, U, V> e)
        {
            Action += e;
        }

        public void Unregister(Action<T, U, V> e)
        {
            Action -= e;
        }
    }
}