using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition<T> : DescriptionBaseSO
{
    public abstract bool Eval(T obj);
}
