using GameWorkstore.Patterns;

public abstract class PropertyBinding<T>
{
    private bool _setup = false;
    public T Value = default(T);
    public Signal<T> OnChange;

    public void Test(T valueT)
    {
        if (valueT.Equals(Value) && _setup) return;

        _setup = true;
        Value = valueT;
        OnChange.Invoke(valueT);
    }
}

public abstract class PropertyBinding<T, U>
{
    private bool _setup = false;
    public T ValueT = default(T);
    public U ValueU = default(U);
    public Signal<T,U> OnChange;

    public void Test(T valueT, U valueU)
    {
        if (ValueT.Equals(valueT) && ValueU.Equals(valueU) && _setup) return;

        _setup = true;
        ValueT = valueT;
        ValueU = valueU;
        OnChange.Invoke(valueT, valueU);
    }
}