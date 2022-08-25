using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoroutineOperationSO : ScriptableObject
{
    public void Operate() {
        GameManager.BeginCoroutine(Execute());
    }

    // To be overriden
    protected abstract IEnumerator Execute();
}
