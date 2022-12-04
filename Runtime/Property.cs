using System;

namespace GameWorkstore.Patterns
{
    public class Property<T>
    {
        private bool _initialized;
        public T Value { get; private set; }
        private readonly Signal<T> _onChange = new Signal<T>();

        public Property()
        {
        }

        public Property(T initialValue)
        {
            Value = initialValue;
        }

        /// <summary>
        /// Updates value if 
        /// </summary>
        /// <param name="valueT"></param>
        public void Set(T valueT)
        {
            if (valueT.Equals(Value) && _initialized) return;

            _initialized = true;
            Value = valueT;
            _onChange.Invoke(valueT);
        }

        /// <summary>
        /// Register and Invokes callback with current value.
        /// </summary>
        /// <param name="e">Callback</param>
        public void Register(Action<T> e)
        {
            _onChange.Register(e);
            e?.Invoke(Value);
        }

        /// <summary>
        /// Remove callback
        /// </summary>
        /// <param name="e">Callback</param>
        public void Unregister(Action<T> e)
        {
            _onChange.Unregister(e);
        }

        /// <summary>
        /// Clear all callbacks
        /// </summary>
        /// <param name="e">Callback</param>
        public void Empty()
        {
            _onChange.Empty();
        }

        public override bool Equals(object obj)
        {
            return Value.Equals((T)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator T(Property<T> property)
        {
            return property.Value;
        }

        public static bool operator==(Property<T> a, Property<T> b)
        {
            return a.Value.Equals(b.Value);
        }

        public static bool operator!=(Property<T> a, Property<T> b)
        {
            return !(a == b);
        }
    }
}