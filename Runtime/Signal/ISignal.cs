using System;

namespace GameWorkstore.Patterns
{
    public interface ISignal
    {
        void Empty();
        void Invoke();
        void Register(Action e);
        void Unregister(Action e);
    }

    public interface ISignal<T>
    {
        void Empty();
        void Invoke(T evt);
        void Register(Action<T> e);
        void Unregister(Action<T> e);
    }

    public interface ISignal<T, U>
    {
        void Empty();
        void Invoke(T evt, U evu);
        void Register(Action<T,U> e);
        void Unregister(Action<T,U> e);
    }

    public interface ISignal<T, U, V>
    {
        void Empty();
        void Invoke(T evt, U evu, V evv);
        void Register(Action<T, U, V> e);
        void Unregister(Action<T, U, V> e);
    }
}