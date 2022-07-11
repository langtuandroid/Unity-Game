using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefString : DescriptionBaseSO
{
    public string value;
    public bool useConstant;
    [SerializeField] private VarString constant;

    public string Value
    {
        get
        {
            if (useConstant) { return constant.value; }
            return value;
        }
    }
}
