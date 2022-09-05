using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/States/AI/WanderState")]
public class WanderState : State
{
    private AIController controller;

    public override void InitializeFields(GameObject obj)
    {
        controller = obj.GetComponent<AIController>();
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
        controller.StopWander();
    }

    public override Type Tick()
    {
        if (controller.TargetInSignt()) {
            return typeof(ChaseState);
        }
        controller.Wander();
        return null;
    }
}
