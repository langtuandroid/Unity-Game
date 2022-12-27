using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SortableItem<T> : IComparable<SortableItem<T>>
{
    public int priority;
    public T item;

    public int CompareTo(SortableItem<T> other)
    {
        return priority - other.priority;
    }
}
