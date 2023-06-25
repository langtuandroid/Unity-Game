using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefAbilityPriority
{
    [SerializeField] private ActionPriority value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private ActionPrioritySO sharedValue;

    public RefAbilityPriority(ActionPriority value = default, bool useSharedValue = false, ActionPrioritySO sharedValue = null)
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
