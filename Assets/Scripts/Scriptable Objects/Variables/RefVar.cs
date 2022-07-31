using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RefVar<T> : ScriptableObject
{
    [SerializeField] private T value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private Variable<T> sharedValue;

    public T Value
    {
        get
        {
            if (useSharedValue) {
                if (sharedValue == null) {
                    return default;
                }
                return sharedValue.Value; 
            }
            return value;
        }

        set {
            if (useSharedValue)
            {
                sharedValue.Value = value;
            }
            else { 
                this.value = value;
            }
        }
    }
}