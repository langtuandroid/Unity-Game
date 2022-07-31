using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RefSO<T>: ScriptableObject where T : ScriptableObject
{
    [SerializeField] private T value;
    [SerializeField] private bool useSharedValue;
    [SerializeField] private T sharedValue;

    public T Value
    {
        get
        {
            if (useSharedValue)
            {
                if (sharedValue == null)
                {
                    return default;
                }
                return sharedValue;
            }
            return value;
        }

        set
        {
            if (useSharedValue)
            {
                sharedValue = value;
            }
            else
            {
                this.value = value;
            }
        }
    }
}
