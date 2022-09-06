using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : State
{
    private AIController controller;
    public float wanderTime;
    public float idleTime;

    private float wanderTimeCounter;
    private float idleTimeCounter;

    private WanderInternalState wanderState;

    private enum WanderInternalState { 
        Idle,
        PathFinding,
        Wandering
    }


    public override void InitializeFields(GameObject obj)
    {
        controller = obj.GetComponent<AIController>();
    }

    public override void OnEnter()
    {
        wanderState = WanderInternalState.Idle;
        wanderTimeCounter = 0;
        idleTimeCounter = 0;
    }

    public override void OnExit()
    {
        
    }

    public override Type Tick()
    {
        if (controller.TargetInSignt()) {
            return typeof(ChaseState);
        }
        switch (wanderState) {
            case WanderInternalState.Idle:
                idleTimeCounter += Time.deltaTime;
                if (idleTimeCounter >= idleTime) {
                    idleTimeCounter = 0;
                    wanderState = WanderInternalState.PathFinding;
                }
                break;
            case WanderInternalState.PathFinding:
                controller.Wander();
                wanderState = WanderInternalState.Wandering;
                break;
            case WanderInternalState.Wandering:
                wanderTimeCounter += Time.deltaTime;
                if (controller.ReachedDestination() || wanderTimeCounter >= wanderTime) {
                    wanderTimeCounter = 0;
                    wanderState = WanderInternalState.Idle;
                }
                break;
        }
        return null;
    }
}
