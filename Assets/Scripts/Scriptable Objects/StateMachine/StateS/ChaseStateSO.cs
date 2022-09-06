using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/States/AI/ChaseState")]
public class ChaseStateSO : StateSO
{
    public override State ResolveState()
    {
        return new ChaseState();
    }

    public override Type GetStateType()
    {
        return typeof(ChaseState);
    }
}
