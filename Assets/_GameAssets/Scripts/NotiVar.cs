using System;

[Serializable]
public class NotiVar<T>
{
    public NotiVar()
    {
    }
    public NotiVar(T baseValue)
    {
        Value = baseValue;
    }
    private T _value;
    
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            onValueChanged?.Invoke();
        }
    }
    public Action onValueChanged;
    public override string ToString()
    {
        return Value.ToString();
    }
}
