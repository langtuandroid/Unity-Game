using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefActionPriority
{
    [SerializeField] private ActionPriority value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private ActionPrioritySO sharedValue;

    public RefActionPriority(ActionPriority value = default, bool useSharedValue = false, ActionPrioritySO sharedValue = null)
    {
        this.value = value;
        this.useSharedValue = useSharedValue;
        this.sharedValue = sharedValue;
    }
    public ActionPriority Value
    {
        get { if (useSharedValue) { return sharedValue.Extract(); } return value; }
    }
}
