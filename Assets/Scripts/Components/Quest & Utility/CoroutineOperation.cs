using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoroutineOperation : Operation
{
    private Coroutine coroutine;

    public sealed override void Begin()
    {
        if (coroutine != null) {
            return;
        }
        coroutine = GameManager.BeginCoroutine(Operate());
    }

    private IEnumerator Operate() {
        yield return Coroutine();
        coroutine = null;
    }

    public void End() {
        if (coroutine == null) {
            return;
        }
        GameManager.EndCoroutine(coroutine);
        coroutine = null;
    }

    // Override this to implement custom behavior
    protected abstract IEnumerator Coroutine();
}
