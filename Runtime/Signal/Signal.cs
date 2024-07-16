using System;

namespace GameWorkstore.Patterns
{
    public class Signal : ISignal
    {
        public event Action Action;

        public void Empty()
        {
            Action = null;
        }

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

    public class Signal<T> : ISignal<T>
    {
        public event Action<T> Action;

        public void Empty()
        {
            Action = null;
        }

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

    public class Signal<T, U> : ISignal<T, U>
    {
        public event Action<T, U> Action;

        public void Empty()
        {
            Action = null;
        }

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

    public class Signal<T, U, V> : ISignal<T, U, V>
    {
        public event Action<T, U, V> Action;

        public void Empty()
        {
            Action = null;
        }

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