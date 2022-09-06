using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/States/AI/FleeState")]
public class FleeStateSO : StateSO
{
    public override State ResolveState()
    {
        return new FleeState();
    }

    public override Type GetStateType()
    {
        return typeof(FleeState);
    }
}
