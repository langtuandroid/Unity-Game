using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefBool : DescriptionBaseSO {
    public bool value;
    public bool useConstant;
    [SerializeField] private VarBool constant;

    public bool Value {
        get { if (useConstant) { return constant.value; }
            return value;
        }
    }
}
