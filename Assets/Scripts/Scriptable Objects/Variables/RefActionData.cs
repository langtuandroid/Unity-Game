using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefActionData
{
    [SerializeField] private ActionData value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private ActionDataSO sharedValue;

    public RefActionData(ActionData value = default, bool useSharedValue = false, ActionDataSO sharedValue = null)
    {
        this.value = value;
        this.useSharedValue = useSharedValue;
        this.sharedValue = sharedValue;
    }
    public ActionData Value
    {
        get { if (useSharedValue) { return sharedValue.Data; } return value; }
    }
}
