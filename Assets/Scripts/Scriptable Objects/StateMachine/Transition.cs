using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Transition
{
    [SerializeField] private StateSO targetState;
    [SerializeField] private List<Condition> transitionConditions;

    public StateSO Eval()
    {
        foreach (Condition condition in transitionConditions)
        {
            if (condition.Eval())
            {
                return targetState;
            }
        }
        return null;
    }

    public StateSO TargetState {
        get { return targetState; }
    } 
}
