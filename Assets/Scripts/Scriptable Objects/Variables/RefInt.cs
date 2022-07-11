using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefInt : DescriptionBaseSO
{
    public int value;
    public bool useConstant;
    [SerializeField] private VarInt constant;

    public int Value
    {
        get
        {
            if (useConstant) { return constant.value; }
            return value;
        }
    }
}
