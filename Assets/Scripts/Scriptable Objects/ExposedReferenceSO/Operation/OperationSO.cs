using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OperationSO : ScriptableObject
{
    // To be overriden
    public abstract void Operate();
}
