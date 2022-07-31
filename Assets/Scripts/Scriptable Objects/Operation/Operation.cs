using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operation : DescriptionBaseSO
{
    [Tooltip("Condition that determines when this operation should end.")]
    [SerializeField] private Condition terminalCondition;
    [Tooltip("Should be marked as true if the operation lasts longer than 1 frame.")]
    [SerializeField] private bool useCoroutine;
    public virtual bool Start() {
        Execute();
        if (!useCoroutine) {
            return false;
        }
        return terminalCondition.Eval();
    }

    public bool CanOperate() {
        if (!useCoroutine) {
            return false;
        }
        return terminalCondition.Eval();
    }

    // To be overriden
    protected abstract void Execute();
}
