using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManagedSet<T>
{
    private OrderedSet<T> items = new();
    private int pointer = 0;

    private int Count
    {
        get { return items.Count; }
    }

    private int Pointer
    {
        get
        {
            if (items.Count == 0)
            {
                return -1;
            }
            if (pointer >= items.Count || pointer < 0)
            {
                pointer = items.Count - 1;
            }

            return pointer;
        }
    }
    public T CurrentItem
    {
        get
        {
            if (Pointer == -1)
            {
                return default;
            }
            return items.ElementAt(pointer);
        }
    }

    public void Advance()
    {
        pointer++;
        if (pointer >= items.Count) { 
            pointer = 0;
        }
    }

    public bool Add(T i)
    {
        return items.Add(i);
    }

    public bool Remove(T i)
    {
        return items.Remove(i);
    }
}
