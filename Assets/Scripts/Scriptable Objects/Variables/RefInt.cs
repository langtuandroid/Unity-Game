using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RefInt
{
    [SerializeField] private int value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private VarInt sharedValue;

    public RefInt(int value = 0, bool useSharedValue = false, VarInt sharedValue = null)
    {
        this.value = value;
        this.useSharedValue = useSharedValue;
        this.sharedValue = sharedValue;
    }

    public int Value
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
