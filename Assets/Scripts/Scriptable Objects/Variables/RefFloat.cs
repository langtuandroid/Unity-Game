using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefFloat
{
    [SerializeField] private float value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private VarFloat sharedValue;

    public RefFloat(float value = 0, bool useSharedValue = false, VarFloat sharedValue = null) { 
        this.value = value;
        this.useSharedValue = useSharedValue;
        this.sharedValue = sharedValue;
    }

    public float Value
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
