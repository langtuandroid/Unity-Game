using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefBool{
    [SerializeField] private bool value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private VarBool sharedValue;

    public RefBool(bool value = default, bool useSharedValue = false, VarBool sharedValue = null)
    {
        this.value = value;
        this.useSharedValue = useSharedValue;
        this.sharedValue = sharedValue;
    }

    public bool Value
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
