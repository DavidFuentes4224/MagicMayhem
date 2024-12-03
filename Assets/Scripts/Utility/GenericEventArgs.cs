using System;

public class GenericEventArgs<T> : EventArgs
{
    public GenericEventArgs(T t)
    {
        value = t;
    }

    public T value { get; private set; }
}
