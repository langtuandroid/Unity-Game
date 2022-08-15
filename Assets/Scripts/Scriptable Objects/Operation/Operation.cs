using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operation : ScriptableObject
{
    // To be overriden
    public abstract void Operate();
}
