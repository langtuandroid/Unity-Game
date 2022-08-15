using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefString
{
    [SerializeField] private string value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private VarString sharedValue;

    public RefString(string value = "", bool useSharedValue = false, VarString sharedValue = null) { 
        this.value = value;
        this.useSharedValue = useSharedValue;
        this.sharedValue = sharedValue;
    }

    public string Value
    {
        get
        {
            if (useSharedValue)
            {
                if (sharedValue == null)
                {
                    return default;
                }
                return sharedValue.Value;
            }
            return value;
        }

        set
        {
            if (useSharedValue)
            {
                sharedValue.Value = value;
            }
            else
            {
                this.value = value;
            }
        }
    }
}
