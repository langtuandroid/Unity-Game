using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoroutineOperation : ScriptableObject
{
    public void Operate() {
        GameManager.BeginCoroutine(Execute());
    }

    // To be overriden
    protected abstract IEnumerator Execute();
}
