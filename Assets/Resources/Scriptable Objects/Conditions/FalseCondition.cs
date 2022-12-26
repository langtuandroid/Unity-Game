using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseCondition : Condition
{
    public override bool Eval()
    {
        return false;
    }
}
