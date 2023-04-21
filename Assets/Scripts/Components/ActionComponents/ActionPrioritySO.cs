using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/ActionPrioritySO")]

public class ActionPrioritySO : ScriptableObject {
    [SerializeField] private RefInt executionPriority;
    [SerializeField] private RefInt enqueuePriority;
    private ActionPriority data;
    private bool isSet = false;

    public int ExecutionPriority
    {
        get { return executionPriority.Value; }
    }

    public int EnqueuePriority
    {
        get { return enqueuePriority.Value; }
    }

    public ActionPriority Extract() {
        if (!isSet) { 
            data.executionPriority = executionPriority.Value;
            data.executionPriority = enqueuePriority.Value;
        }
        return data; 
    }
}

[System.Serializable]
public struct ActionPriority
{
    public int executionPriority;
    public int enqueuePriority;
}
