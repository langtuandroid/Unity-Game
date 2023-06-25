using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable<T> : ScriptableObject
{
    [SerializeField] private T value;

    public T Value
    {
        get { return value; }
        set { this.value = value; }
    }
}
