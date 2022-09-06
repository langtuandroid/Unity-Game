using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/States/AI/WanderState")]
public class WanderStateSO : StateSO
{
    [SerializeField] private RefFloat wanderTime;
    [SerializeField] private RefFloat idleTime;

    public override State ResolveState()
    {
        WanderState state = new WanderState();
        state.wanderTime = wanderTime.Value;
        state.idleTime = idleTime.Value;
        return state;
    }

    public override Type GetStateType()
    {
        return typeof(WanderState);
    }
}
