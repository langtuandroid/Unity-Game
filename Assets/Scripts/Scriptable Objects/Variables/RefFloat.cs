using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefFloat : DescriptionBaseSO
{
    public float value;
    public bool useConstant;
    [SerializeField] private VarFloat constant;

    public float Value
    {
        get
        {
            if (useConstant) { return constant.value; }
            return value;
        }
    }
}
