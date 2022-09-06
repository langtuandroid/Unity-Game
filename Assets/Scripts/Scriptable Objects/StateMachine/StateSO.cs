using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class StateSO : DescriptionBaseSO
{
    public abstract State ResolveState();

    public abstract System.Type GetStateType();
}
